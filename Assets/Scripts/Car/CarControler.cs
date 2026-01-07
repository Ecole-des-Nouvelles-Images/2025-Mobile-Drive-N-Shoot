using System.Collections;
using System.Collections.Generic;
using __Workspaces.Alex.Scripts;
using Core;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Utils.Game;

namespace Car
{
    public class CarControler : MonoBehaviour
    {
        [Header("Car Controls")]
        public float motorTorque = 2000f;
        public float brakeTorque = 2000f;
        public float maxSpeed = 10f;
        public float baseMaxSpeed = 10f;
        public float steeringRange = 30f;
        public float steeringRangeAtMaxSpeed = 10f;
        public float centreOfGravityOffset = -1f;
    
        [Header("Visual")]
        [SerializeField] private List<MeshRenderer> _pickupMeshRenderers;
        [SerializeField] private List<MeshRenderer> _turretMeshRenderers = new();

        [Header("Boost")]
        public float boostCooldown = 10f;         // cooldown between boosts (seconds)
        public float shakeThreshold = 2.0f;       // threshold for detecting a shake (high-frequency component)
        public float boostDeltaV = 5f;            // desired instantaneous delta-V in m/s (VelocityChange)
        public float lowPassFilterFactor = 0.1f;  // smoothing factor for low-pass filter on accelerometer
        [SerializeField] private CarHealth _carHealth;   // Reference to CarHealth component
        [SerializeField] private float _shieldDuration = 2f; // Duration of shield after boost
        [SerializeField] private Image _imageSpeedEffect;

        [Header("Damage behavior")]
        public float damagedSpeedFactor = 0.8f;

        [Header("Input Thresholds")]
        public float reverseThreshold = -0.4f;  // Joystick must go below this to trigger reverse
    
        [Header("Cinemachine Camera")]
        public GameObject _camera;

        [Header("Wheel VFX")] 
        public float particleMinSpeed = 0.5f; // Minimal speed to activate wheel particles
        public float particleMaxEmission = 30f; // Max 

        [Header("Boost VFX")]
        [SerializeField] private ParticleSystem _boostVFX;
        
        [Header("SFX")] 
        [SerializeField] private EventReference _engineSound;
        [SerializeField] private EventReference _boostSound;

        private EventInstance _engineInstance;
    
    
        private WheelControl[] _wheels;
        private Rigidbody _rigidBody;
        private CarInputActions _carControls;

        // Cooldown state
        private float _nextBoostTime;

        // Low-pass filtered acceleration for shake detection
        private Vector3 _lowPassAcceleration = Vector3.zero;

        // Saved velocity for pause system
        private Vector3 _savedVelocity;
        private Vector3 _savedAngularVelocity;
        private bool _isPaused = false;
    
        // Damage state
        private bool _isDamaged = false;
        
        // Coroutine handle for shield timeout
        private Coroutine _shieldCoroutine;
    
    
        private void Awake()
        {
            GameManager.Instance.Player = gameObject;
            _carControls = new CarInputActions();
        }

        private void OnEnable()
        {
            _carControls.Enable();
            EventBus.OnGameOver += HandleGamePause;
            EventBus.OnGamePause += HandleGamePause;
            EventBus.OnGameResume += HandleGameResume;
            EventBus.OnPlayerAtHalfHealth += DamageVehicle;
            EventBus.OnPlayerRecoveredFromHalf += RestoreVehicle;
        }

        private void OnDisable()
        {
            _carControls.Disable();
            EventBus.OnGameOver -= HandleGamePause;
            EventBus.OnGamePause -= HandleGamePause;
            EventBus.OnGameResume -= HandleGameResume;
            EventBus.OnPlayerAtHalfHealth -= DamageVehicle;
            EventBus.OnPlayerRecoveredFromHalf -= RestoreVehicle;
        }

        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();

            // Adjust center of mass to improve stability and prevent rolling
            Vector3 centerOfMass = _rigidBody.centerOfMass;
            centerOfMass.y += centreOfGravityOffset;
            _rigidBody.centerOfMass = centerOfMass;

            // Get all wheel components attached to the car
            _wheels = GetComponentsInChildren<WheelControl>();

            // Initialize accelerometer filter and cooldown so boost can be used immediately
            _lowPassAcceleration = Input.acceleration;
            // _nextBoostTime = TimeManager.Instance.Time;
        
            // Skin
            foreach (var meshRenderers in _pickupMeshRenderers)
            {
                meshRenderers.materials = GameManager.Instance.CurrentCarMaterials;
            }
            foreach (var meshRenderers in _turretMeshRenderers)
            {
                meshRenderers.materials = GameManager.Instance.CurrentTurretMaterials;
            }
            
            // Start engine sound
            if (_engineInstance.isValid())
            {
                _engineInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _engineInstance.release();
            }

            _engineInstance = AudioManager.Instance.Play(_engineSound, loop: false, follow: gameObject);
        }

        void Update()
        {
            // Read raw acceleration and apply a simple low-pass filter to remove gravity/slow changes.
            Vector3 currentAccel = Input.acceleration;
            _lowPassAcceleration = Vector3.Lerp(_lowPassAcceleration, currentAccel, lowPassFilterFactor);

            // High-frequency component (shake) = raw - low-pass
            Vector3 highFreq = currentAccel - _lowPassAcceleration;

            // If the high-frequency magnitude exceeds the threshold and cooldown has passed, trigger boost immediately
            // float now = TimeManager.Instance.Time;
            if (_nextBoostTime <= boostCooldown)
            {
                _nextBoostTime += TimeManager.Instance.DeltaTime;
                _nextBoostTime = Mathf.Clamp(_nextBoostTime, 0f, boostCooldown);
                EventBus.OnPlayerBoostCooldown?.Invoke(_nextBoostTime, boostCooldown);
            }
            if (highFreq.magnitude > shakeThreshold && _nextBoostTime >= boostCooldown)
            {
                ApplyVelocityChangeBoost();
            }
            
            // Sound gestion
            // Normalize speed to 0–1 for FMOD
            float speed01 = Mathf.Clamp01(_rigidBody.linearVelocity.magnitude / 20f);

            if (_engineInstance.isValid())
                _engineInstance.setParameterByName("RPM", speed01);

#if UNITY_EDITOR || UNITY_STANDALONE
            // Editor / standalone shortcut for testing: press B to trigger boost
            if (Input.GetKeyDown(KeyCode.B) && _nextBoostTime >= boostCooldown)
            {
                ApplyVelocityChangeBoost();
            }
#endif
        }

        void FixedUpdate()
        {
            if (_isPaused) return;
            // --- Input ---
            Vector2 inputVector = _carControls.CarControls.Move.ReadValue<Vector2>();
            float vInput = inputVector.y;
            float hInput = inputVector.x;

            // --- Reverse threshold handling ---
            float effectiveVInput = vInput;

            // Only activate reverse if joystick pushed enough
            if (vInput < 0 && vInput > reverseThreshold)
            {
                effectiveVInput = 0f; // allow turning but no reverse motion
            }

            // --- Speed & steering logic ---
            float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

            float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
            float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

            bool isAccelerating = Mathf.Sign(effectiveVInput) == Mathf.Sign(forwardSpeed);

            foreach (var wheel in _wheels)
            {
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
                }

                if (isAccelerating)
                {
                    if (wheel.motorized)
                    {
                        wheel.WheelCollider.motorTorque = effectiveVInput * currentMotorTorque;
                    }

                    wheel.WheelCollider.brakeTorque = 0f;
                }
                else
                {
                    wheel.WheelCollider.motorTorque = 0f;
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(effectiveVInput) * brakeTorque;
                }
            }

            // --- Speed decay after boost ---
            float currentSpeed = _rigidBody.linearVelocity.magnitude;

            if (currentSpeed > maxSpeed)
            {
                float newSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 0.01f);
                _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * newSpeed;
            }
        
            // --- Wheel particle effects ---
            foreach (var wheel in _wheels)
            {
                if (wheel.wheelParticles != null)
                {
                    var emission = wheel.wheelParticles.emission;

                    if (currentSpeed > particleMinSpeed)
                    {
                        // Enable emission proportionally to speed
                        emission.rateOverTime = Mathf.Lerp(0, particleMaxEmission, currentSpeed / maxSpeed);
                        if(!wheel.wheelParticles.isPlaying) wheel.wheelParticles.Play();
                    }
                    else
                    {
                        // Disable emission when slow or stopped
                        emission.rateOverTime = 0f;
                        if (wheel.wheelParticles.isPlaying) wheel.wheelParticles.Stop();
                    }
                }
            }
        }

 
    
        // Apply an immediate velocity change (VelocityChange) and set the cooldown
        private void ApplyVelocityChangeBoost()
        {
            // Play boost sound
            AudioManager.Instance.Play(_boostSound, follow: gameObject);
            // Play boost VFX
            if (_boostVFX) _boostVFX.Play();
            
            
            _nextBoostTime = 0f;

            // Apply an immediate velocity change in the forward direction (mass-independent)
            _rigidBody.AddForce(transform.forward * boostDeltaV, ForceMode.VelocityChange);


            _carHealth.IsShieldActive = true;
            if (_shieldCoroutine != null)
            {
                StopCoroutine(_shieldCoroutine);
            }
            _shieldCoroutine = StartCoroutine(ShieldTimeOutCoroutine());


            // Change Material
            float targetValue = 0.3f;
            DOTween.To(
                () => 0f,
                value =>
                {
                    GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_ResistanceProgress", value);
                    GameManager.Instance.CurrentCarMaterials[0].SetFloat("_ResistanceProgress", value);
                    GameManager.Instance.CurrentIemExhaustPipeMaterials[0].SetFloat("_ResistanceProgress", value);
                },
                targetValue,
                0.5f
            );
            
            // Active Speed Effect
            _imageSpeedEffect.DOFade(1f, 0.5f);
        }

        private IEnumerator ShieldTimeOutCoroutine()
        {
            float elapsed = 0f;
            while (elapsed < _shieldDuration)
            {
                elapsed += TimeManager.Instance.DeltaTime;
                yield return null;
            }
            _carHealth.IsShieldActive = false;
            _shieldCoroutine = null;
            
            // Change Color
            float targetValue = 0f;
            DOTween.To(
                () => 0.6f,
                value =>
                {
                    GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_ResistanceProgress", value);
                    GameManager.Instance.CurrentCarMaterials[0].SetFloat("_ResistanceProgress", value);
                    GameManager.Instance.CurrentIemExhaustPipeMaterials[0].SetFloat("_ResistanceProgress", value);
                },
                targetValue,
                0.75f
            );
            
            // Disable Speed Effect
            _imageSpeedEffect.DOFade(0f, 0.2f);
        }

        private void HandleGamePause()
        {
            // Save Velocity
            _savedVelocity = _rigidBody.linearVelocity;
            _savedAngularVelocity = _rigidBody.angularVelocity;
        
            // Stop instantly
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        
            // Disable wheel motion
            foreach (var wheel in _wheels)
            {
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = brakeTorque;
            }
        
            // Disable Camera to avoid its movement
            _camera.SetActive(false);
        
            _isPaused = true;
        }

        private void HandleGameResume()
        {
            // Restore previous velocity
            _rigidBody.linearVelocity = _savedVelocity;
            _rigidBody.angularVelocity = _savedAngularVelocity;
        
            // Release brakes
            foreach (var wheel in _wheels)
            {
                wheel.WheelCollider.brakeTorque = 0f;
            }
            // Active camera
            _camera.SetActive(true);
            _isPaused = false;
        }

        private void DamageVehicle()
        {
            if (_isDamaged) return;
            _isDamaged = true;
            // Reduce maxSpeed
            maxSpeed = baseMaxSpeed * damagedSpeedFactor;
        }

        private void RestoreVehicle()
        {
            if (!_isDamaged) return;
            _isDamaged = false;
            // Restore maxSpeed
            maxSpeed = baseMaxSpeed;
        }
        private void OnDestroy()
        {
            if (_engineInstance.isValid())
            {
                _engineInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _engineInstance.release();
                _engineInstance.clearHandle();
            }
        }
    }
}
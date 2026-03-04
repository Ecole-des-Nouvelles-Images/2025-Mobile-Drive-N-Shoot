using System.Collections;
using System.Collections.Generic;
using __Workspaces.Alex.Scripts;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Core;
using Utils.Game;

namespace Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarControler : MonoBehaviour
    {
        [Header("Car Controls")]
        public float motorTorque = 2000f;
        public float brakeTorque = 2000f;
        public float engineBrakeTorque = 500f;
        public float maxSpeed = 10f;
        public float baseMaxSpeed = 10f;
        public float steeringRange = 30f;
        public float steeringRangeAtMaxSpeed = 10f;
        public float centreOfGravityOffset = -1f;

        [Header("Visual & VFX")]
        [SerializeField] private List<MeshRenderer> _pickupMeshRenderers;
        [SerializeField] private List<MeshRenderer> _turretMeshRenderers = new();
        [SerializeField] private ParticleSystem _boostVFX;
        [SerializeField] private Image _imageSpeedEffect;
        public float particleMinSpeed = 0.5f;
        public float particleMaxEmission = 30f;

        [Header("Boost Settings")]
        public float boostCooldown = 10f;
        public float shakeThreshold = 2.0f;
        public float boostDeltaV = 5f;
        public float lowPassFilterFactor = 0.1f;
        [SerializeField] private CarHealth _carHealth;
        [SerializeField] private float _shieldDuration = 2f;

        [Header("Damage & Input")]
        public float damagedSpeedFactor = 0.8f;
        public float reverseThreshold = -0.4f;
        private const float IDLE_THRESHOLD = 0.05f; // Seuil pour considérer le joystick au repos

        [Header("References")]
        public GameObject _camera;
        [SerializeField] private EventReference _engineSound;
        [SerializeField] private EventReference _boostSound;

        // Internal State
        private Rigidbody _rigidBody;
        private WheelControl[] _wheels;
        private CarInputActions _carControls;
        private EventInstance _engineInstance;
        private Coroutine _shieldCoroutine;
        
        private Vector3 _lowPassAcceleration = Vector3.zero;
        private Vector3 _savedVelocity, _savedAngularVelocity;
        private float _nextBoostTime;
        private bool _isPaused, _isDamaged;

        #region Lifecycle

        private void Awake()
        {
            GameManager.Instance.Player = gameObject;
            _carControls = new CarInputActions();
            _rigidBody = GetComponent<Rigidbody>();
            _wheels = GetComponentsInChildren<WheelControl>();
        }

        private void OnEnable()
        {
            _carControls.Enable();
            ToggleEvents(true);
        }

        private void OnDisable()
        {
            _carControls.Disable();
            ToggleEvents(false);
        }

        private void Start()
        {
            _rigidBody.centerOfMass += new Vector3(0, centreOfGravityOffset, 0);
            _lowPassAcceleration = Input.acceleration;
            
            InitializeSkins();
            InitializeAudio();
        }

        private void Update()
        {
            if (_isPaused) return;

            HandleShakeDetection();
            UpdateEngineAudio();
            HandleEditorInputs();
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;

            HandlePhysics();
            ApplySpeedLimits();
            UpdateWheelVFX();
        }

        #endregion

        #region Movement Logic

        private void HandlePhysics()
        {
            Vector2 input = _carControls.CarControls.Move.ReadValue<Vector2>();
            float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

            // Calcul du couple et de la direction
            float currentMotor = Mathf.Lerp(motorTorque, 0, speedFactor);
            float currentSteer = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

            // Détermination de l'état (Accélération, Frein manuel ou Frein moteur)
            bool isIdle = Mathf.Abs(input.y) < IDLE_THRESHOLD;
            bool isBraking = !isIdle && (Mathf.Sign(input.y) != Mathf.Sign(forwardSpeed) && Mathf.Abs(forwardSpeed) > 0.1f);

            foreach (var wheel in _wheels)
            {
                if (wheel.steerable)
                    wheel.WheelCollider.steerAngle = input.x * currentSteer;

                if (isIdle) 
                {
                    // --- FREIN MOTEUR ---
                    wheel.WheelCollider.motorTorque = 0;
                    wheel.WheelCollider.brakeTorque = engineBrakeTorque;
                }
                else if (isBraking)
                {
                    // Freinage actif
                    wheel.WheelCollider.motorTorque = 0;
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(input.y) * brakeTorque;
                }
                else
                {
                    // Propulsion
                    float effectiveInput = (input.y < 0 && input.y > reverseThreshold) ? 0 : input.y;
                    if (wheel.motorized) wheel.WheelCollider.motorTorque = effectiveInput * currentMotor;
                    wheel.WheelCollider.brakeTorque = 0;
                }
            }
        }

        private void ApplySpeedLimits()
        {
            float currentSpeed = _rigidBody.linearVelocity.magnitude;
            if (currentSpeed > maxSpeed)
            {
                _rigidBody.linearVelocity = Vector3.Lerp(_rigidBody.linearVelocity, _rigidBody.linearVelocity.normalized * maxSpeed, 0.05f);
            }
        }

        #endregion

        #region Boost & Shake Detection

        private void HandleShakeDetection()
        {
            if (_nextBoostTime < boostCooldown)
            {
                _nextBoostTime = Mathf.Clamp(_nextBoostTime + TimeManager.Instance.DeltaTime, 0f, boostCooldown);
                EventBus.OnPlayerBoostCooldown?.Invoke(_nextBoostTime, boostCooldown);
            }

            Vector3 currentAccel = Input.acceleration;
            _lowPassAcceleration = Vector3.Lerp(_lowPassAcceleration, currentAccel, lowPassFilterFactor);
            
            if ((currentAccel - _lowPassAcceleration).magnitude > shakeThreshold && _nextBoostTime >= boostCooldown)
            {
                ApplyVelocityChangeBoost();
            }
        }

        private void ApplyVelocityChangeBoost()
        {
            AudioManager.Instance.Play(_boostSound, follow: gameObject);
            if (_boostVFX) _boostVFX.Play();
            
            _nextBoostTime = 0f;
            _rigidBody.AddForce(transform.forward * boostDeltaV, ForceMode.VelocityChange);

            _carHealth.IsShieldActive = true;
            if (_shieldCoroutine != null) StopCoroutine(_shieldCoroutine);
            _shieldCoroutine = StartCoroutine(ShieldRoutine());

            SetMaterialsProgress(0.3f, 0.5f);
            _imageSpeedEffect.DOFade(0.5f, 0.5f);
        }

        private IEnumerator ShieldRoutine()
        {
            yield return new WaitForSeconds(_shieldDuration);
            _carHealth.IsShieldActive = false;
            SetMaterialsProgress(0f, 0.75f);
            _imageSpeedEffect.DOFade(0f, 0.2f);
            _shieldCoroutine = null;
        }

        #endregion

        #region Visuals & Audio

        private void InitializeSkins()
        {
            foreach (var r in _pickupMeshRenderers) r.materials = GameManager.Instance.CurrentCarMaterials;
            foreach (var r in _turretMeshRenderers) r.materials = GameManager.Instance.CurrentTurretMaterials;
        }

        private void InitializeAudio()
        {
            if (_engineInstance.isValid()) { _engineInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); _engineInstance.release(); }
            _engineInstance = AudioManager.Instance.Play(_engineSound, loop: false, follow: gameObject);
        }

        private void UpdateEngineAudio()
        {
            if (_engineInstance.isValid())
            {
                float rpm = Mathf.Clamp01(_rigidBody.linearVelocity.magnitude / 20f);
                _engineInstance.setParameterByName("RPM", rpm);
            }
        }

        private void UpdateWheelVFX()
        {
            float speed = _rigidBody.linearVelocity.magnitude;
            foreach (var wheel in _wheels)
            {
                if (wheel.wheelParticles == null) continue;
                var emission = wheel.wheelParticles.emission;

                if (speed > particleMinSpeed)
                {
                    emission.rateOverTime = Mathf.Lerp(0, particleMaxEmission, speed / maxSpeed);
                    if (!wheel.wheelParticles.isPlaying) wheel.wheelParticles.Play();
                }
                else if (wheel.wheelParticles.isPlaying) wheel.wheelParticles.Stop();
            }
        }

        private void SetMaterialsProgress(float target, float duration)
        {
            DOTween.To(() => GameManager.Instance.CurrentCarMaterials[0].GetFloat("_ResistanceProgress"),
                value => {
                    GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_ResistanceProgress", value);
                    GameManager.Instance.CurrentCarMaterials[0].SetFloat("_ResistanceProgress", value);
                    GameManager.Instance.CurrentIemExhaustPipeMaterials[0].SetFloat("_ResistanceProgress", value);
                }, target, duration);
        }

        #endregion

        #region Events & Pause

        private void ToggleEvents(bool subscribe)
        {
            if (subscribe) {
                EventBus.OnGameOver += HandleGamePause; EventBus.OnGamePause += HandleGamePause;
                EventBus.OnGameResume += HandleGameResume; EventBus.OnPlayerAtHalfHealth += DamageVehicle;
                EventBus.OnPlayerRecoveredFromHalf += RestoreVehicle;
            } else {
                EventBus.OnGameOver -= HandleGamePause; EventBus.OnGamePause -= HandleGamePause;
                EventBus.OnGameResume -= HandleGameResume; EventBus.OnPlayerAtHalfHealth -= DamageVehicle;
                EventBus.OnPlayerRecoveredFromHalf -= RestoreVehicle;
            }
        }

        private void HandleGamePause()
        {
            _savedVelocity = _rigidBody.linearVelocity;
            _savedAngularVelocity = _rigidBody.angularVelocity;
            _rigidBody.linearVelocity = _rigidBody.angularVelocity = Vector3.zero;
            foreach (var w in _wheels) w.WheelCollider.brakeTorque = brakeTorque;
            _camera.SetActive(false);
            _isPaused = true;
        }

        private void HandleGameResume()
        {
            _rigidBody.linearVelocity = _savedVelocity;
            _rigidBody.angularVelocity = _savedAngularVelocity;
            foreach (var w in _wheels) w.WheelCollider.brakeTorque = 0f;
            _camera.SetActive(true);
            _isPaused = false;
        }

        private void DamageVehicle() { if (_isDamaged) return; _isDamaged = true; maxSpeed = baseMaxSpeed * damagedSpeedFactor; }
        private void RestoreVehicle() { if (!_isDamaged) return; _isDamaged = false; maxSpeed = baseMaxSpeed; }

        private void HandleEditorInputs()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.B) && _nextBoostTime >= boostCooldown) ApplyVelocityChangeBoost();
#endif
        }

        private void OnDestroy()
        {
            if (_engineInstance.isValid()) { _engineInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); _engineInstance.release(); }
        }

        #endregion
    }
}
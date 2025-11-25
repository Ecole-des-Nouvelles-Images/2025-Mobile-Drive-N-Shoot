using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Game;

public class CarControler : MonoBehaviour
{
    [Header("Car Controls")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;
    
    [Header("Visual")]
    [SerializeField] private MeshRenderer _pickupMeshRenderer;
    [SerializeField] private List<MeshRenderer> _turretMeshRenderers = new();

    [Header("Boost")]
    public float boostCooldown = 10f;         // cooldown between boosts (seconds)
    public float shakeThreshold = 2.0f;       // threshold for detecting a shake (high-frequency component)
    public float boostDeltaV = 5f;            // desired instantaneous delta-V in m/s (VelocityChange)
    public float lowPassFilterFactor = 0.1f;  // smoothing factor for low-pass filter on accelerometer

    [Header("Input Thresholds")]
    public float reverseThreshold = -0.4f;  // Joystick must go below this to trigger reverse

    private WheelControl[] _wheels;
    private Rigidbody _rigidBody;
    private CarInputActions _carControls;

    // Cooldown state
    private float _nextBoostTime;

    // Low-pass filtered acceleration for shake detection
    private Vector3 _lowPassAcceleration = Vector3.zero;

    private void Awake()
    {
        _carControls = new CarInputActions();
        
        // Skin
        _pickupMeshRenderer.materials = GameManager.Instance.CurrentCarMaterials;
        foreach (var meshRenderers in _turretMeshRenderers)
        {
            meshRenderers.materials = GameManager.Instance.CurrentTurretMaterials;
        }
    }

    private void OnEnable()
    {
        _carControls.Enable();
    }

    private void OnDisable()
    {
        _carControls.Disable();
    }

    void Start()
    {
        // EventBus.OnGameStart(); // TODO: delete this line if not needed
        _rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = _rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        _rigidBody.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        _wheels = GetComponentsInChildren<WheelControl>();

        // Initialize accelerometer filter and cooldown so boost can be used immediately
        _lowPassAcceleration = Input.acceleration;
        _nextBoostTime = TimeManager.Instance.Time;
    }

    void Update()
    {
        // Read raw acceleration and apply a simple low-pass filter to remove gravity/slow changes.
        Vector3 currentAccel = Input.acceleration;
        _lowPassAcceleration = Vector3.Lerp(_lowPassAcceleration, currentAccel, lowPassFilterFactor);

        // High-frequency component (shake) = raw - low-pass
        Vector3 highFreq = currentAccel - _lowPassAcceleration;

        // If the high-frequency magnitude exceeds the threshold and cooldown has passed, trigger boost immediately
        float now = TimeManager.Instance.Time;
        if (highFreq.magnitude > shakeThreshold && now >= _nextBoostTime)
        {
            ApplyVelocityChangeBoost(now);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // Editor / standalone shortcut for testing: press B to trigger boost
        if (Input.GetKeyDown(KeyCode.B) && now >= _nextBoostTime)
        {
            ApplyVelocityChangeBoost(now);
        }
#endif
    }

    void FixedUpdate()
    {
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
            float newSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 0.05f);
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * newSpeed;
        }
    }

    // Apply an immediate velocity change (VelocityChange) and set the cooldown
    private void ApplyVelocityChangeBoost(float now)
    {
        _nextBoostTime = now + boostCooldown;

        // Apply an immediate velocity change in the forward direction (mass-independent)
        _rigidBody.AddForce(transform.forward * boostDeltaV, ForceMode.VelocityChange);
        Debug.Log($"Boost applied: +{boostDeltaV} m/s (VelocityChange). Next boost at {_nextBoostTime:F2}");
    }
}
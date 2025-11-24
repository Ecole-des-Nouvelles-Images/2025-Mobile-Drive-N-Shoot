using System;
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

    [Header("Boost")]
    public float boostCooldown = 10f;         // cooldown between boosts (seconds)
    public float shakeThreshold = 2.0f;       // threshold for detecting a shake (high-frequency component)
    public float boostDeltaV = 5f;            // desired instantaneous delta-V in m/s (VelocityChange)
    public float lowPassFilterFactor = 0.1f;  // smoothing factor for low-pass filter on accelerometer

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
        EventBus.OnGameStart(); // TODO: delete this line if not needed
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
        // Read the Vector2 input from the new Input System
        Vector2 inputVector = _carControls.CarControls.Move.ReadValue<Vector2>();

        // Get player input for acceleration and steering
        float vInput = inputVector.y; // Forward/backward input
        float hInput = inputVector.x; // Steering input

        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Determine if the player is accelerating or trying to reverse
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in _wheels)
        {
            // Apply steering to wheels that support steering
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to motorized wheels
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }

                // Release brakes when accelerating
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when reversing direction or no throttle
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
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
using System;
using System.Collections.Generic;
using __Workspaces.Hugoi.Scripts.GameLoop;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarControler : MonoBehaviour
{
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;
    
    [Header("Visual")]
    [SerializeField] private MeshRenderer _pickupMeshRenderer;
    [SerializeField] private List<MeshRenderer> _turretMeshRenderers = new();

    private WheelControl[] _wheels;
    private Rigidbody _rigidBody;
    private CarInputActions _carControls;

    private void Awake()
    {
        _carControls = new CarInputActions();
        
        // Skin
        _pickupMeshRenderer.materials = GameManager.Instance.CarMaterial;
        foreach (var meshRenderers in _turretMeshRenderers)
        {
            meshRenderers.materials = GameManager.Instance.TurretMaterial;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = _rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        _rigidBody.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        _wheels = GetComponentsInChildren<WheelControl>();

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
                // Apply brakes when reversing direction
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }
}
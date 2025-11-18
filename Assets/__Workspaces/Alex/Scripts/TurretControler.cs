using System;
using UnityEngine;

public class TurretControler : MonoBehaviour
{
    [SerializeField] private Transform _turret;
    [SerializeField] private Transform _vehicle;
    [SerializeField] private float _rotationSpeed = 5f;
    private CarInputActions _carInputActions;

    private void Awake()
    {
        _carInputActions = new CarInputActions();
    }

    private void OnEnable()
    {
        _carInputActions.Enable();
    }

    private void OnDisable()
    {
        _carInputActions.Disable();
    }
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vehicleYaw = _vehicle.eulerAngles.y;
        Vector2 input = _carInputActions.CarControls.Aim.ReadValue<Vector2>();
        float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle + vehicleYaw, 0);
        _turret.rotation = Quaternion.Slerp(_turret.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}

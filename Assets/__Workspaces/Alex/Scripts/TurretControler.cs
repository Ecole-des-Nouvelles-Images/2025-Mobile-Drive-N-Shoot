using System;
using UnityEngine;

public class TurretControler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _turret;
    [SerializeField] private Transform _vehicle;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ParticleSystem _muzzleFlash;
    
    [Header("Aiming")]
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("Overheat")]
    [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _heatPerSecond = 10f;
    [SerializeField] private float _coolPerSecond = 10f;
    [SerializeField] private float _overheatCooldownPerSecond = 20f;
    
    private CarInputActions _carInputActions;

    private float _currentHeat = 0f;
    private bool _isOverHeated = false;
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
        Vector2 input = _carInputActions.CarControls.Aim.ReadValue<Vector2>();
        bool isAiming = input.sqrMagnitude > 0;
        if (_isOverHeated)
        {
            _currentHeat -= _overheatCooldownPerSecond * Time.deltaTime;
            if (_currentHeat <= 0f)
            {
                _isOverHeated = false;
            }
        }
        else
        {
            _currentHeat -= _coolPerSecond * Time.deltaTime;
        }
        _currentHeat = Mathf.Clamp(_currentHeat, 0f, _maxHeat);
        if (!_isOverHeated && isAiming)
        {
            float vehicleYaw = _vehicle.eulerAngles.y;
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle + vehicleYaw, 0);
            _turret.rotation = Quaternion.Slerp(_turret.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            Fire();
        }
    }

    private void Fire()
    {
        _currentHeat += _heatPerSecond * Time.deltaTime;
        if (_currentHeat >= _maxHeat)
        {
            _currentHeat = _maxHeat;
            _isOverHeated = true;
            _muzzleFlash.Stop();
            return;
        }
        _muzzleFlash.Play();
        Vector3 origin = _firePoint.position;
        Vector3 direction = _firePoint.forward;
        if (Physics.Raycast(origin, direction, out RaycastHit hit))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
        }
    }
}

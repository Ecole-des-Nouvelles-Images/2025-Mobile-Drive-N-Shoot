using System;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;

public class TurretControler : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform _turret;

    [SerializeField] private Transform _turretBarrel;
    [SerializeField] private Transform _vehicle;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ParticleSystem _muzzleFlash;

    [Header("Aiming")]
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private TurretAimDetector _turretAimDetector;

    [Header("Firing")] 
    [SerializeField] private float _fireRate = 0.1f;

    [SerializeField] private float damage = 10f;
    

    [Header("Overheat")] [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _heatPerSecond = 10f;
    [SerializeField] private float _coolPerSecond = 10f;
    [SerializeField] private float _overheatCooldownPerSecond = 20f;

    private CarInputActions _carInputActions;

    private float _currentHeat = 0f;
    private bool _isOverHeated = false;
    private float _nextFireTime = 0f;
    private bool _isFiring = false;
    private Transform _currentTarget;

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

    void Update()
    {
        // Read aim input from controls
        Vector2 input = _carInputActions.CarControls.Aim.ReadValue<Vector2>();
        bool isAiming = input.sqrMagnitude > 0f;
        float dt = TimeManager.Instance.DeltaTime;

        // Cooling / overheat recovery
        if (_isOverHeated)
        {
            // When overheated we cool faster using the overheat cooldown rate
            _currentHeat -= _overheatCooldownPerSecond * dt;
            if (_currentHeat <= 0f)
            {
                _currentHeat = 0f;
                _isOverHeated = false;
            }
        }
        else
        {
            // Do not cool while aiming/holding fire.
            if (!isAiming)
            {
                _currentHeat -= _coolPerSecond * dt;
            }
            // If isAiming == true => no cooling (heat only accumulates)
        }

        // Clamp heat value between 0 and maxHeat
        _currentHeat = Mathf.Clamp(_currentHeat, 0f, _maxHeat);

        // If not overheated and the player is aiming, rotate turret and handle firing/heat
        if (!_isOverHeated && isAiming)
        {
            // 1. Horizontal rotation controlled by the joystick
            float vehicleYaw = _vehicle.eulerAngles.y;
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle + vehicleYaw, 0);
            _turret.rotation = Quaternion.Slerp(_turret.rotation, targetRotation, _rotationSpeed * dt);

            // 2. Detect an enemy in the aimed direction
            _currentTarget = DetectEnemyInAimDirection(input);

            // 3. Vertical rotation for the barrel (auto-adjust if enemy detected)
            if (_currentTarget != null)
            {
                Vector3 directionToEnemy = _currentTarget.position - _firePoint.position;
                float pitchAngle = Mathf.Atan2(directionToEnemy.y, new Vector2(directionToEnemy.x, directionToEnemy.z).magnitude) * Mathf.Rad2Deg;
                pitchAngle = Mathf.Clamp(pitchAngle, -30f, 60f); // Limit vertical angle
                _turretBarrel.localRotation = Quaternion.Slerp(
                    _turretBarrel.localRotation,
                    Quaternion.Euler(-pitchAngle, 0, 0),
                    _rotationSpeed * dt
                );
            }
            else
            {
                // Reset barrel vertical rotation 
                _turretBarrel.localRotation = Quaternion.Slerp(
                    _turretBarrel.localRotation,
                    Quaternion.Euler(0, 0, 0),
                    _rotationSpeed * dt
                );
            }

            // --- HEAT: accumulate while aiming (no cooling while aiming) ---
            _currentHeat += _heatPerSecond * dt;
            if (_currentHeat >= _maxHeat)
            {
                // Enter overheat state and stop muzzle effect immediately
                _currentHeat = _maxHeat;
                _isOverHeated = true;
                StopMuzzleImmediate();
                _isFiring = false;
            }
            else
            {
                // Firing logic respecting the fire rate
                if (Time.time >= _nextFireTime)
                {
                    _nextFireTime = Time.time + _fireRate;
                    Fire(); // perform a single shot (raycast/damage logic)
                }

                // Start the muzzle effect if not already playing
                if (!_isFiring)
                {
                    StartMuzzle();
                    _isFiring = true;
                }
            }
        }
        else
        {
            // Not allowed to fire (either overheated or not aiming) -> ensure muzzle is stopped
            if (_isFiring)
            {
                StopMuzzle();
                _isFiring = false;
            }
        }
    }

    private Transform DetectEnemyInAimDirection(Vector2 aimInput)
    {
        Transform enemy = _turretAimDetector.GetClosestEnemy(_firePoint.position);
        return enemy;
    }

    // Performs a single shot: raycast and debug line. Keep this method for per-shot effects/damage.
    private void Fire()
    {
        Vector3 origin = _firePoint.position;
        Vector3 direction = _firePoint.forward;
        RaycastHit hit;
        //If enemy detected
        if (_currentTarget)
        {
            direction = (_currentTarget.position - origin).normalized;
            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
            {
                //Inflict damages to enemy
                _currentTarget.GetComponent<IDamageable>().TakeDamage(damage);
                ImpactPool.Instance.PlayImpact(hit.point, hit.normal, ImpactType.Enemy);
            }
        }
        else
        {
            direction = _firePoint.forward;
            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
            {
                ImpactPool.Instance.PlayImpact(hit.point, hit.normal, ImpactType.Default);
            }
        }
    }

    private void StartMuzzle()
    {
        if (!_muzzleFlash) return;
        var emission = _muzzleFlash.emission;
        emission.enabled = true;
        _muzzleFlash.Play();
    }

    private void StopMuzzle()
    {
        if (!_muzzleFlash) return;
        var emission = _muzzleFlash.emission;
        emission.enabled = false;
        // Stop emitting new particles but let existing ones finish
        _muzzleFlash.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }

    private void StopMuzzleImmediate()
    {
        if (!_muzzleFlash) return;
        var emission = _muzzleFlash.emission;
        emission.enabled = false;
        // Immediately stop and clear all particles (used on overheat)
        _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
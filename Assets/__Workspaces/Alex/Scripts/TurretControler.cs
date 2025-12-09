using FMODUnity;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;

namespace __Workspaces.Alex.Scripts
{
    public class TurretControler : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform _rangeColliderTransform;
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _turretBarrel;
        [SerializeField] private Transform _vehicle;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private LineRenderer _lineRenderer; // Added line renderer

        [Header("Aiming")]
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private Transform _defaultAimTransform;
        [SerializeField] private TurretAimDetector _turretAimDetector;

        [Header("Firing")] 
        [SerializeField] private float _fireRate = 0.1f;
        [SerializeField] private float damage = 10f;

        [Header("Overheat")] 
        [SerializeField] private float _maxHeat = 100f;
        [SerializeField] private float _heatPerSecond = 10f;
        [SerializeField] private float _coolPerSecond = 10f;
        [SerializeField] private float _overheatCooldownPerSecond = 20f;
        
        [Header("Sounds")]
        [SerializeField] private EventReference _fireSound;
        [SerializeField] private EventReference _overheatSound;

        private CarInputActions _carInputActions;

        private float _currentHeat = 0f;
        private bool _isOverHeated = false;
        private float _nextFireTime = 0f;
        private bool _isFiring = false;
        private Transform _currentTarget;
        private Vector3 _currentAimPosition;

        private bool _noOverheatActive = false;
        private float _noOverheatTimer = 0f;

        private void Awake()
        {
            _carInputActions = new CarInputActions();
        }

        private void Start()
        {
            // Ensure laser is off at start
            if (_lineRenderer)
                _lineRenderer.enabled = false;
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
            Vector2 input = _carInputActions.CarControls.Aim.ReadValue<Vector2>();
            bool isAiming = input.sqrMagnitude > 0f;
            float dt = TimeManager.Instance.DeltaTime;

            // No-overheat item active
            if (_noOverheatActive)
            {
                _noOverheatTimer -= dt;
                _currentHeat = 0f;
                _isOverHeated = false;

                if (_noOverheatTimer <= 0f)
                    _noOverheatActive = false;
            }

            // Cooling logic
            if (_isOverHeated)
            {
                _currentHeat -= _overheatCooldownPerSecond * dt;
                if (_currentHeat <= 0f)
                {
                    _currentHeat = 0f;
                    _isOverHeated = false;
                }
            }
            else if (!isAiming)
            {
                _currentHeat -= _coolPerSecond * dt;
            }

            _currentHeat = Mathf.Clamp(_currentHeat, 0, _maxHeat);

            RotateRangeCollider(input, dt);
            // --- AIMING & FIRING LOGIC ---
            if (!_isOverHeated && isAiming)
            {
                HandleTurretRotation(input, dt);
                AutoPitchBarrel(dt);
                HandleHeat(dt);
                HandleFiring();
            }
            else
            {
                if (_isFiring)
                {
                    StopMuzzle();
                    _isFiring = false;
                }

                DisableLaser();
            }
        }

        // -------------------------
        // ROTATION & TARGETING
        // -------------------------

        /// <summary>
        /// Used for rotate collider with inputs
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dt"></param>
        private void RotateRangeCollider(Vector2 input, float dt)
        {
            float vehicleYaw = _vehicle.eulerAngles.y;
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle + vehicleYaw, 0);
            _rangeColliderTransform.rotation = Quaternion.Slerp(_rangeColliderTransform.rotation, targetRotation, _rotationSpeed * dt);
            _currentAimPosition = _defaultAimTransform.position;
        
            Transform closestEnemy = _turretAimDetector.GetClosestEnemy(_firePoint.position);
            _currentTarget = closestEnemy;
        
            if (closestEnemy) {
                IEnemy enemy = closestEnemy.GetComponent<IEnemy>();
                _currentAimPosition = enemy.GetAimPosition;
            }
        }
    
        private void HandleTurretRotation(Vector2 input, float dt)
        {
            float vehicleYaw = _vehicle.eulerAngles.y;
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        
            if (_currentTarget == null)
            {
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle + vehicleYaw, 0);
                _turret.rotation = Quaternion.Slerp(_turret.rotation, targetRotation, _rotationSpeed * dt);
                _currentAimPosition = _defaultAimTransform.position;
            }
            else
            { 
                Vector3 dirToEnemy = _currentTarget.position - _turret.position;
                dirToEnemy.y = 0f; // keep turret horizontal rotation only

                Quaternion targetRotation = Quaternion.LookRotation(dirToEnemy);
                _turret.rotation = Quaternion.Slerp(
                    _turret.rotation,
                    targetRotation,
                    _rotationSpeed * dt
                );
            }
        }

        private void AutoPitchBarrel(float dt)
        {
            Vector3 dir = _currentAimPosition - _firePoint.position;
            float pitch = Mathf.Atan2(dir.y, new Vector2(dir.x, dir.z).magnitude) * Mathf.Rad2Deg;
            pitch = Mathf.Clamp(pitch, -30f, 60f);
        
            _turretBarrel.localRotation = Quaternion.Slerp(
                _turretBarrel.localRotation,
                Quaternion.Euler(-pitch, 0, 0),
                _rotationSpeed * dt
            );
        }

        // -------------------------
        // FIRING
        // -------------------------

        private void HandleHeat(float dt)
        {
            _currentHeat += _heatPerSecond * dt;
            if (_currentHeat >= _maxHeat)
            {
                _currentHeat = _maxHeat;
                _isOverHeated = true;
                AudioManager.Instance.PlayAtPosition(_overheatSound, _firePoint.position);
                StopMuzzleImmediate();
                DisableLaser();
                _isFiring = false;
            }
        }

        private void HandleFiring()
        {
            if (TimeManager.Instance.Time >= _nextFireTime)
            {
                _nextFireTime = TimeManager.Instance.Time + _fireRate;
                Fire();
            }

            if (!_isFiring)
            {
                StartMuzzle();
                _isFiring = true;
            }

            DrawLaser();
        }

        private void Fire()
        {
            // Play fire sound
            AudioManager.Instance.PlayAtPosition(_fireSound, _firePoint.position);
            
            Vector3 origin = _firePoint.position;
            Vector3 direction;
            
            // In note cause not aiming the right direction :
            // if (_currentAimPosition != Vector3.zero)
            //     direction = (_currentAimPosition - origin).normalized;
            // else
                direction = _firePoint.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (_currentTarget)
                    _currentTarget.GetComponent<IDamageable>()?.TakeDamage(damage);
                Debug.DrawRay(origin, direction * hit.distance, Color.yellow, 3f);

                ImpactPool.Instance.PlayImpact(hit.point, hit.normal, ImpactType.Default);
            }
        }

        // -------------------------
        // LASER
        // -------------------------

        private void DrawLaser()
        {
            if (!_lineRenderer) return;

            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, _firePoint.position + _firePoint.forward * 100f);
        
            // Vector3 dir = _currentAimPosition != Vector3.zero
            //     ? (_currentAimPosition - _firePoint.position).normalized
            //     : _firePoint.forward;
            //
            // if (Physics.Raycast(_firePoint.position, dir, out RaycastHit hit))
            //     _lineRenderer.SetPosition(1, hit.point);
            // else
            //     _lineRenderer.SetPosition(1, _firePoint.position + dir * 100f);
        }

        private void DisableLaser()
        {
            if (_lineRenderer)
                _lineRenderer.enabled = false;
        }

        // -------------------------
        // MUZZLE FLASH
        // -------------------------

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
            _muzzleFlash.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        private void StopMuzzleImmediate()
        {
            if (!_muzzleFlash) return;
            var emission = _muzzleFlash.emission;
            emission.enabled = false;
            _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // -------------------------
        // ITEM EFFECT
        // -------------------------

        public void ActivateNoOverheat(float duration)
        {
            _noOverheatActive = true;
            _noOverheatTimer = duration;
            _currentHeat = 0f;
            _isOverHeated = false;
        }
    }
}

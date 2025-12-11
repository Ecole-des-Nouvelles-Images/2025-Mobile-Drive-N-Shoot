using __Workspaces.Alex.Scripts;
using FMODUnity;
using Core;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;

namespace Car
{
    public class TurretController : MonoBehaviour
    {
        [Header("Shooting")]
        [SerializeField] private float _shootingSpeed;
        [SerializeField] private float _damage;
        
        [Header("Overheating")]
        [SerializeField] private float _maxOverheatValue;
        [SerializeField] private float _overheatSpeed;
        [SerializeField] private float _cooldownSpeed;
        
        [Header("Aiming")]
        [SerializeField] private float _turretRotationSpeed;
        [SerializeField] private Transform _turretFireStartTransform;
        [SerializeField] private Transform _turretDefaultAimTransform;
        [SerializeField] private TurretAimDetector _turretAimDetector;
        [SerializeField] private LineRenderer _lineRenderer;
        
        [Header("References")]
        [SerializeField] private Transform _turretSupport;
        [SerializeField] private Transform _turretGun;

        [Header("SFX")] 
        [SerializeField] private EventReference _shootSFX;
        [SerializeField] private EventReference _overheatSFX;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _shootVFX;
        [SerializeField] private ParticleSystem _overheatVFX;
        
        private CarInputActions _carInputActions;
        
        [Header("DEBUG")]
        // States
        [SerializeField] private bool _isAiming;
        [SerializeField] private bool _isOverheating;
        
        // Aiming
        [SerializeField] private bool _lineRendererIsActive;
        [SerializeField] private Vector3 _targetTransform;
        
        // Shooting
        [SerializeField] private float _shootTimerCooldown;
        
        // Overheating
        [SerializeField] private float _currentOverheatValue;
        
        private void Awake()
        {
            _carInputActions = new CarInputActions();
            
            // Debug
            TimeManager.Instance.Resume();
        }

        private void OnEnable()
        {
            _carInputActions.Enable();
        }

        private void OnDisable()
        {
            _carInputActions.Disable();
        }

        private void Update()
        {
            Vector2 input = _carInputActions.CarControls.Aim.ReadValue<Vector2>();
            _isAiming = input.sqrMagnitude > 0f;
            
            if (_shootVFX) _shootVFX.Stop();


            if (_isAiming)
            {
                // Rotate on y the turret support
                Vector3 dir = new Vector3(input.x, 0f, input.y);
                Quaternion supportTargetRot = Quaternion.LookRotation(transform.TransformDirection(dir));
                _turretSupport.rotation = Quaternion.Slerp(
                    _turretSupport.rotation, 
                    supportTargetRot, 
                    TimeManager.Instance.DeltaTime * _turretRotationSpeed
                );
                
                // Rotate on x and z the turret gun
                Transform closestEnemyTransform = _turretAimDetector.GetClosestEnemy(transform.position);
                
                if (closestEnemyTransform)
                {
                    _targetTransform = closestEnemyTransform.GetComponent<IEnemy>().GetAimPosition;
                }
                else
                {
                    _targetTransform = _turretDefaultAimTransform.position;
                }
                Quaternion gunTargetRot = Quaternion.LookRotation(_targetTransform - transform.position);
                
                _turretGun.rotation = Quaternion.Slerp(
                    _turretGun.rotation,
                    gunTargetRot,
                    TimeManager.Instance.DeltaTime * _turretRotationSpeed
                );
                
                if (!_isOverheating)
                {
                    DisplayLaser(true, _targetTransform);
                    
                    // DONE DAMAGE
                    _shootTimerCooldown += TimeManager.Instance.DeltaTime;
                    if (_shootTimerCooldown >= _shootingSpeed)
                    {
                        if (closestEnemyTransform)
                        {
                            closestEnemyTransform.gameObject.GetComponent<IDamageable>().TakeDamage(_damage);
                        }
                        _shootTimerCooldown = 0f;
                        Debug.Log("Turret Shoot");
                        // SFX
                        AudioManager.Instance.PlayAtPosition(_shootSFX, transform.position);
                        // VFX
                        if (_shootVFX) _shootVFX.Play();
                    }
                    
                    // OVERHEATING
                    _currentOverheatValue += _overheatSpeed * TimeManager.Instance.DeltaTime;
                    if (_currentOverheatValue >= _maxOverheatValue)
                    {
                        _isOverheating = true;
                        DisplayLaser(false, _targetTransform);
                        // SFX
                        AudioManager.Instance.PlayAtPosition(_overheatSFX, transform.position);
                        // VFX
                        if (_shootVFX) _shootVFX.Stop();
                        if (_overheatVFX) _overheatVFX.Play();
                    }
                    
                    // VISUAL MATERIAL
                    GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_HitProgress", _currentOverheatValue / _maxOverheatValue / 2f);
                }
            }
            else
            {
                if (_lineRendererIsActive) DisplayLaser(false, _targetTransform);
            }
            
            
            // COOLING DOWN
            if (_isOverheating || !_isAiming)
            {
                if (_currentOverheatValue <= 0f) return;
                
                _currentOverheatValue -= _cooldownSpeed * TimeManager.Instance.DeltaTime;
                if (_currentOverheatValue <= 0f)
                {
                    _isOverheating = false;
                    //VFX
                    if (_overheatVFX) _overheatVFX.Stop();
                }
                
                // VISUAL MATERIAL
                GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_HitProgress", _currentOverheatValue / _maxOverheatValue / 2f);
            }
        }
        
        private void DisplayLaser(bool isActive, Vector3 targetPos)
        {
            if (isActive)
            {
                _lineRenderer.enabled = true;
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, _turretFireStartTransform.position);
                _lineRenderer.SetPosition(1, targetPos);
                _lineRendererIsActive = true;
            }
            else
            {
                _lineRenderer.enabled = false;
                _lineRendererIsActive = false;
            }
        }
    }
}
using __Workspaces.Alex.Scripts;
using __Workspaces.Hugoi.Scripts;
using FMODUnity;
using Core;
using UnityEngine;
using UnityEngine.UI;
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
        
        [Header("No Overheat")]
        [SerializeField] private bool _noOverheatActive;
        [SerializeField] private float _noOverheatTimer;
        
        [Header("Aiming")]
        [SerializeField] private float _turretRotationSpeed;
        [SerializeField] private Transform _turretFireStartTransform;
        [SerializeField] private Transform _turretDefaultAimTransform;
        [SerializeField] private Transform _turretDefaultPosTransform;
        [SerializeField] private TurretAimDetector _turretAimDetector;
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("References")]
        [SerializeField] private Transform _turretSupport;
        [SerializeField] private Transform _turretGun;
        [SerializeField] private Image _imageOverheatFill;
        [SerializeField] private Image _imageButtonAim;
        [SerializeField] private Button _buttonAim;

        [Header("SFX")]
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private EventReference _shootSFX;
        [SerializeField] private EventReference _overheatSFX;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _shootVFX;
        [SerializeField] private ParticleSystem _overheatVFX;
        [SerializeField] private ParticleSystem _bulletVFX;
        
        // private CarInputActions _carInputActions;
        
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
            // _carInputActions = new CarInputActions();
            
            // Debug
            TimeManager.Instance.Resume();
        }

        // private void OnEnable()
        // {
        //     _carInputActions.Enable();
        // }
        //
        // private void OnDisable()
        // {
        //     _carInputActions.Disable();
        // }

        private void Update()
        {
            // float input = _carInputActions.CarControls.Aim.ReadValue<float>();
            // _isAiming = input > 0f;
            
            // NO OVERHEAT TIMER
            if (_noOverheatActive)
            {
                _noOverheatTimer -= TimeManager.Instance.DeltaTime;

                _currentOverheatValue = 0f;
                _isOverheating = false;

                if (_noOverheatTimer <= 0f)
                {
                    _noOverheatActive = false;
                    GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_NoOverheat", 0f);
                }
            }

            // Stop shoot VFX if not aiming
            if (_shootVFX) _shootVFX.Stop();


            if (_isAiming)
            {
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

                Vector3 dir = _targetTransform - _turretGun.position;
                Quaternion targetRot = Quaternion.LookRotation(dir);

                _turretSupport.rotation = Quaternion.Slerp(
                    _turretSupport.rotation, 
                    targetRot, 
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
                        
                        // RAYCAST
                        Vector3 start = _turretGun.position;
                        Vector3 end = _targetTransform;
                        Vector3 direction = (end - start).normalized;
                        float distance = Vector3.Distance(start, end);
                        
            
                        if (Physics.Raycast(start, direction, out RaycastHit hit, distance, _layerMask))
                        {
                            // Detect if it's an enemy
                            ImpactType type = hit.transform.gameObject.CompareTag("Enemy") 
                                ? ImpactType.Enemy 
                                : ImpactType.Default;
            
                            // Call the pool to play the impact
                            ImpactPool.Instance.PlayImpact(
                                hit.point,
                                hit.normal,
                                type
                            );
                        }
                        
                        // SFX
                        AudioManager.Instance.PlayAtPosition(_shootSFX, transform.position);
                        // VFX
                        if (_shootVFX) _shootVFX.Play();
                        if (_bulletVFX) _bulletVFX.Emit(1);
                    }
                    
                    // OVERHEATING
                    if (!_noOverheatActive)
                    {
                        _currentOverheatValue += _overheatSpeed * TimeManager.Instance.DeltaTime;
                    }
                    if (_currentOverheatValue >= _maxOverheatValue && !_noOverheatActive)
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
                _targetTransform = _turretDefaultPosTransform.position;
                
                Vector3 dir = _targetTransform - _turretGun.position;
                Quaternion targetRot = Quaternion.LookRotation(dir);

                _turretSupport.rotation = Quaternion.Slerp(
                    _turretSupport.rotation, 
                    targetRot, 
                    TimeManager.Instance.DeltaTime * _turretRotationSpeed
                );
                
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
            
            _imageOverheatFill.fillAmount = _currentOverheatValue / _maxOverheatValue;
            _imageButtonAim.color = _isOverheating ? Color.red : Color.white;
            _buttonAim.interactable = !_isOverheating;
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
        public void ActivateNoOverheat(float duration)
        {
            _noOverheatActive = true;
            _noOverheatTimer = duration;
            _currentOverheatValue = 0f;
            _isOverheating = false;
            GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_NoOverheat", 1f);
        }
    }
}
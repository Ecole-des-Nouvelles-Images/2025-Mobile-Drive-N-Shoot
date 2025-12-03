using Core;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;
using Vector3 = UnityEngine.Vector3;

namespace Enemy.Drone
{
    public class DroneController : EnemyData, IDamageable
    {
        [Header("Turret")]
        [SerializeField] private Transform _turretTransform;
        [SerializeField] private float _rotateSpeed;
        
        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField] private Transform _startingPos;

        private bool _laserEnabled;
        private Vector3 _targetPos;
        private Vector3 _targetOffset = Vector3.zero;

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }
        
        private void Start()
        {
            TargetTransform = GameManager.Instance.Player.transform;
            TargetHealth = TargetTransform.GetComponent<CarHealth>();
            
            _targetPos = TargetTransform.position;
        }
        
        private void OnEnable()
        {
            EventBus.OnGameResume += OnGameResume;
            EventBus.OnGamePause += OnGamePause;
            EventBus.OnGameOver += OnGamePause;
        }
        
        private void OnDisable()
        {
            EventBus.OnGameResume -= OnGameResume;
            EventBus.OnGamePause -= OnGamePause;
            EventBus.OnGameOver -= OnGamePause;
        }

        private void Update()
        {
            if (IsDying && !IsDead)
            {
                Die();
                return;
            }

            if (CanAttack)
            {
                _targetPos = TargetTransform.position;
                if (_targetOffset == Vector3.zero) _targetOffset = transform.position - TargetTransform.position;
                _targetPos += _targetOffset;
                
                DroneTurretLookAt(TargetTransform);
                DisplayLaser(true);
            }
            else
            {
                _targetPos = TargetTransform.position;
                _targetOffset = Vector3.zero;

                ResetTurretRotation();
                if (_laserEnabled)
                {
                    DisplayLaser(false);
                }
            }
            
            if (TargetTransform && NavMeshAgent.isOnNavMesh)
            {
                IsMoving = true;
                NavMeshAgent.SetDestination(_targetPos);
            }
        }

        private void DisplayLaser(bool isActive)
        {
            if (isActive)
            {
                _laserLine.enabled = true;
                _laserLine.positionCount = 2;
                _laserLine.SetPosition(0, _startingPos.position);
                _laserLine.SetPosition(1, TargetTransform.position + Vector3.up * 0.8f);
                _laserEnabled = true;
            }
            else
            {
                _laserLine.enabled = false;
                _laserEnabled = false;
            }
        }

        private void DroneTurretLookAt(Transform target)
        {
            Vector3 dir = target.position - _turretTransform.position;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            _turretTransform.rotation = Quaternion.Slerp(
                _turretTransform.rotation,
                targetRot,
                TimeManager.Instance.DeltaTime * _rotateSpeed
            );
        }

        private void ResetTurretRotation()
        {
            Quaternion targetRot = _turretTransform.parent.rotation;
            
            _turretTransform.rotation = Quaternion.Slerp(
                _turretTransform.rotation,
                targetRot,
                TimeManager.Instance.DeltaTime * _rotateSpeed
            );
        }
        
        private void Die()
        {
            NavMeshAgent.ResetPath();
            // VFX, SFX
            
            IsDead = true;
            Destroy(gameObject, 3f);
        }
        
        private void OnGameResume()
        {
            if (CanAttack) AttackCoroutine = StartCoroutine(CoroutineAttack());
        }

        private void OnGamePause()
        {
            if (CanAttack) StopCoroutine(AttackCoroutine);
        }
    }
}
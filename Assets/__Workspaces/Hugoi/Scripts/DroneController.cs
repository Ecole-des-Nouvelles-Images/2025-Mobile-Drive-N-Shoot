using Core;
using UnityEngine;
using Utils.Interfaces;
using Vector3 = UnityEngine.Vector3;

namespace __Workspaces.Hugoi.Scripts
{
    public class DroneController : EnemyData, IDamageable
    {
        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField] private Transform _startingPos;
        [SerializeField] private float test;
        
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
                if (_targetOffset == Vector3.zero)
                {
                    _targetOffset = transform.position - TargetTransform.position;
                }

                _targetPos += _targetOffset;
                DisplayLaser(true);
            }
            else
            {
                _targetPos = TargetTransform.position;
                _targetOffset = Vector3.zero;
                
                DisplayLaser(false);
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
                _laserLine.SetPosition(1, TargetTransform.position);
            }
            else
            {
                _laserLine.enabled = false;
            }
        }
        
        private void Die()
        {
            // VFX, SFX
            
            IsDead = true;
            Destroy(gameObject, 3f);
        }
    }
}
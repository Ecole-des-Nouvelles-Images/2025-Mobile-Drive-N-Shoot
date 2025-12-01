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
        
        private Vector3 _targetPosOffset;

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        private void Update()
        {
            if (IsDead)
            {
                // Lance l'annimation de mort / VFX puis se détruit
                Debug.Log("Drone Dead");
                return;
            }

            if (CanAttack)
            {
                _targetPosOffset = transform.position - TargetTransform.position;
                DisplayLaser(true);
            }
            else
            {
                _targetPosOffset = Vector3.zero;
                DisplayLaser(false);
            }
            
            if (TargetTransform && NavMeshAgent.isOnNavMesh)
            {
                IsMoving = true;
                NavMeshAgent.SetDestination(TargetTransform.position + _targetPosOffset);
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
    }
}
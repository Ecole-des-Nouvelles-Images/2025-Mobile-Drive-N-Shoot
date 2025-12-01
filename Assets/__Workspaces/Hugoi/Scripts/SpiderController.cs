using UnityEngine;
using Utils.Interfaces;

namespace __Workspaces.Hugoi.Scripts
{
    public class SpiderController : EnemyData, IDamageable
    {
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        private void Update()
        {
            if (IsDead)
            {
                // Lance l'annimation de mort / VFX puis se détruit
                Debug.Log("Spider Dead");
                return;
            }

            if (CanAttack && IsMoving)
            {
                IsMoving = false;
                NavMeshAgent.ResetPath();
            }
            
            if (TargetTransform && NavMeshAgent.isOnNavMesh && !IsAttacking)
            {
                IsMoving = true;
                NavMeshAgent.SetDestination(TargetTransform.position);
            }
        }
    }
}
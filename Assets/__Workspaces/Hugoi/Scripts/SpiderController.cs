using UnityEngine;
using Utils.Interfaces;

namespace __Workspaces.Hugoi.Scripts
{
    public class SpiderController : SpiderData, IDamageable
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

            if (CanAttack())
            {
                IsAttacking = true;
                StartCoroutine(Attack());
                return;
            }
            
            if (TargetTransform && NavMeshAgent.isOnNavMesh && !IsAttacking)
            {
                NavMeshAgent.SetDestination(TargetTransform.position);
            }
        }
    }
}
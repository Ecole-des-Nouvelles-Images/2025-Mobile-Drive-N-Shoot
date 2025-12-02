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
            if (IsDying && !IsDead)
            {
                Die();
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
            
            Animator.SetBool("IsMoving", IsMoving);
            Animator.SetBool("IsAttacking", IsAttacking);
        }

        private void Die()
        {
            Animator.SetBool("IsDead", IsDying);
            
            Collider[] cols = Physics.OverlapSphere(transform.position, DeathExplosionRange);

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player"))
                {
                    TargetHealth.TakeDamage(DeathExplosionDamage);
                    break;
                }
            }
            
            IsDead = true;
            Destroy(gameObject, 3f);
        }
    }
}
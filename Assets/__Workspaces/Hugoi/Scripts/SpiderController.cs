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
            if (IsDead) return;

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
    }
}
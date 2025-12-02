using Core;
using Utils.Interfaces;

namespace __Workspaces.Hugoi.Scripts
{
    public class SpiderController : EnemyData, IDamageable
    {
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }
        
        private void Start()
        {
            TargetTransform = GameManager.Instance.Player.transform;
            TargetHealth = TargetTransform.GetComponent<CarHealth>();
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
            
            // VFX, SFX
            
            IsDead = true;
            Destroy(gameObject, 3f);
        }
    }
}
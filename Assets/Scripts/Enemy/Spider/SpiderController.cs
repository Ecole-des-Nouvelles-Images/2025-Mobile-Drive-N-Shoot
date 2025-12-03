using __Workspaces.Alex.Scripts;
using Core;
using Utils.Game;
using Utils.Interfaces;

namespace Enemy.Spider
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
            if (IsDying)
            {
                if (!IsDead)
                {
                    Die();
                }
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
            NavMeshAgent.ResetPath();
            Collider.enabled = false;
            Animator.SetBool("IsDead", IsDying);
            
            // VFX, SFX
            
            IsDead = true;
            Destroy(gameObject, 3f);
        }
        
        private void OnGameResume()
        {
            Animator.SetFloat("AttackSpeed", AttackSpeed);
        }

        private void OnGamePause()
        {
            Animator.SetFloat("AttackSpeed", 0f);
        }
    }
}
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

            if (SeeTarget)
            {
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
            
            Animator.SetBool("IsMoving", IsMoving);
            Animator.SetBool("IsAttacking", IsAttacking);
        }

        private void Die()
        {
            IsMoving = false;
            NavMeshAgent.ResetPath();
            Collider.enabled = false;
            Animator.SetBool("IsDead", IsDying);
            
            // VFX, SFX
            
            IsDead = true;
            Destroy(gameObject, 3f);
        }
        
        #region Subscriptions
        
        private void OnEnable()
        {
            EventBus.OnGameResume += OnGameResume;
            EventBus.OnGamePause += OnGamePause;
            EventBus.OnGameOver += OnGamePause;
        }
        
        private void OnGameResume()
        {
            IsMoving = true;
            NavMeshAgent.SetDestination(TargetTransform.position);
            Animator.SetFloat("AttackSpeed", AttackSpeed);
        }

        private void OnGamePause()
        {
            IsMoving = false;
            NavMeshAgent.ResetPath();
            Animator.SetFloat("AttackSpeed", 0f);
        }
        
        private void OnDisable()
        {
            EventBus.OnGameResume -= OnGameResume;
            EventBus.OnGamePause -= OnGamePause;
            EventBus.OnGameOver -= OnGamePause;
        }
        
        #endregion
    }
}
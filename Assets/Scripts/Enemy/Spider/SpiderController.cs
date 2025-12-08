using __Workspaces.Alex.Scripts;
using Core;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;

namespace Enemy.Spider
{
    public class SpiderController : EnemyData, IDamageable
    {
        [Header("Explosion Settings")]
        [SerializeField] private float _explosionRange;
        [SerializeField] private float _explosionDamage;
        
        [Header("External Components")]
        [SerializeField] private Animator _animator;
        
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
            if (IsDying || CanAttack)
            {
                IsDying = true;
                if (!IsDead)
                {
                    AutoDestructionExplosion();
                }
                return;
            }

            if (SeeTarget && TargetTransform && NavMeshAgent.isOnNavMesh)
            {
                IsMoving = true;
                NavMeshAgent.SetDestination(TargetTransform.position);
            }
            
            _animator.SetBool("IsMoving", IsMoving);
        }

        private void AutoDestructionExplosion()
        {
            IsMoving = false;
            NavMeshAgent.ResetPath();
            Collider.enabled = false;
            _animator.SetBool("IsDead", IsDying);
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject.CompareTag("Player"))
                {
                    TargetHealth.TakeDamage(_explosionDamage);
                }
            }
            
            // VFX, SFX
            
            IsDead = true;
            Destroy(gameObject, 1f);
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
        }

        private void OnGamePause()
        {
            IsMoving = false;
            NavMeshAgent.ResetPath();
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
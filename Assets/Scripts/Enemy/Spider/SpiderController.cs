using __Workspaces.Alex.Scripts;
using Core;
using DG.Tweening;
using FMODUnity;
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

        [Header("VFX")]
        [SerializeField] private ParticleSystem _explosionVFX;
        
        [Header("SFX")] 
        [SerializeField] private EventReference _explosionSFX;
        
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            
            // Change material
            float targetValue = 0.5f;
            DOTween.To(
                () => 0f,
                value =>
                {
                    foreach (var material in Materials)
                    {
                        material.SetFloat("_HitProgress", value);
                    }
                },
                targetValue,
                0.1f
            ).SetLoops(2, LoopType.Yoyo);
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
            Visual.SetActive(false);
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject.CompareTag("Player"))
                {
                    TargetHealth.TakeDamage(_explosionDamage);
                }
            }
            
            // VFX, SFX
            // AudioManager.Instance.PlayAtPosition(_deathSFX, transform.position);
            if (_explosionVFX) _explosionVFX.Play();
            
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
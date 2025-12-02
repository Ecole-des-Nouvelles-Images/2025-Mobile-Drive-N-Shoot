using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utils.Interfaces;

namespace Enemy
{
    [Serializable]
    public class EnemyData : MonoBehaviour, IEnemy
    {
        [Header("Data")]
        public bool HaveAnimation;
        [Header("Stats")]
        public float MaxHealth;
        public float Damage;
        public float AttackSpeed;
        public float AttackRange;
        private float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

                if (_currentHealth <= 0)
                {
                    IsDying = true;
                }
            }
        }

        [Header("States")]
        public bool IsDying;
        public bool IsDead;
        public bool IsAttacking;
        public bool IsMoving;
        private bool _canAttack;
        public bool CanAttack
        {
            get => _canAttack;
            set
            {
                _canAttack = value;
                
                if (!IsAttacking && _canAttack)
                {
                    IsAttacking = true;
                    if (!HaveAnimation) AttackCoroutine = StartCoroutine(CoroutineAttack());
                }
                else
                {
                    IsAttacking = false;
                    if (!HaveAnimation && AttackCoroutine != null) StopCoroutine(AttackCoroutine);
                }
            }
        }
        
        [Header("IEnemy Interface")]
        [SerializeField] private Transform aimTransform;
        public Vector3 GetAimPosition => aimTransform.position;
        
        [Header("Target")]
        public Transform TargetTransform;
        public CarHealth TargetHealth;
        
        [Header("Internal Components")]
        public NavMeshAgent NavMeshAgent;

        [Header("External Components")]
        public SphereCollider AttackRangeCollider;
        public Animator Animator;

        [Header("Coroutines")]
        public Coroutine AttackCoroutine;
        
        public void Attack()
        {
            TargetHealth.TakeDamage(Damage);
        }
        
        public IEnumerator CoroutineAttack()
        {
            while (IsAttacking)
            {
                yield return new WaitForSeconds(AttackSpeed);
                TargetHealth.TakeDamage(Damage);
            }
        }

        private void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            AttackRangeCollider.radius = AttackRange;
            CurrentHealth = MaxHealth;
            if (HaveAnimation) Animator.SetFloat("AttackSpeed", AttackSpeed);
        }
    }
}

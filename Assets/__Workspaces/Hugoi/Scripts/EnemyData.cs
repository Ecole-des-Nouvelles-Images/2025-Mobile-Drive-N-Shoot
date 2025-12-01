using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace __Workspaces.Hugoi.Scripts
{
    [Serializable]
    public class EnemyData : MonoBehaviour
    {
        [Header("Data")]
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
                    IsDead = true;
                }
            }
        }

        [Header("States")]
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
                    AttackCoroutine = StartCoroutine(Attack());
                }
                else
                {
                    IsAttacking = false;
                    if (AttackCoroutine != null) StopCoroutine(AttackCoroutine);
                }
            }
        }
        
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

        private void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            
            AttackRangeCollider.radius = AttackRange;
            
            CurrentHealth = MaxHealth;
            
            TargetHealth = TargetTransform.GetComponent<CarHealth>();
        }
        
        public IEnumerator Attack()
        {
            while (IsAttacking)
            {
                yield return new WaitForSeconds(AttackSpeed);
                TargetHealth.TakeDamage(Damage);
                Debug.Log("Attack");
            }
        }
    }
}

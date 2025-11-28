using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace __Workspaces.Hugoi.Scripts
{
    [Serializable]
    public class SpiderData : MonoBehaviour
    {
        [Header("Data")]
        [Header("Currents")]
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
        
        [Header("Stats")]
        public float MaxHealth;
        public float Damage;
        public float AttackSpeed;
        public float AttackRange;

        [Header("States")]
        public bool IsDead;
        public bool IsAttacking;
        
        [Header("Target")]
        public Transform TargetTransform;
        
        [Header("Internal Components")]
        public NavMeshAgent NavMeshAgent;
        
        [Header("External Components")]
        public Animator Animator;

        private void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            
            CurrentHealth = MaxHealth;
        }
        
        public bool CanAttack()
        {
            RaycastHit hit;
            
            if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, AttackRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public IEnumerator Attack()
        {
            while (CanAttack())
            {
                yield return new WaitForSeconds(AttackSpeed);
                // Deal Damage
                
            }
            IsAttacking = false;
        }
    }
}

using System;
using System.Collections.Generic;
using __Workspaces.Alex.Scripts;
using UnityEngine;
using UnityEngine.AI;
using Utils.Interfaces;

namespace Enemy
{
    [Serializable]
    public class EnemyData : MonoBehaviour, IEnemy
    {
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
        public bool SeeTarget;
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
                }
                else
                {
                    IsAttacking = false;
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
        public Collider Collider;
        public Renderer[] Renderers;

        [Header("External Components")]
        public SphereCollider AttackRangeCollider;
        
        [Header("Visual")]
        public GameObject Visual;
        public List<Material> Materials;

        private void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Collider = GetComponent<Collider>();
            AttackRangeCollider.radius = AttackRange;
            CurrentHealth = MaxHealth;

            foreach (var renderer in Renderers)
            {
                Materials.Add(renderer.material);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using __Workspaces.Alex.Scripts;
using Core;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;
using Vector3 = UnityEngine.Vector3;

namespace Enemy.Drone
{
    public class DroneController : EnemyData, IDamageable
    {
        [Header("Turret")]
        [SerializeField] private Transform _turretTransform;
        [SerializeField] private float _rotateSpeed;
        
        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField] private Transform _startingPos;
        
        [Header("SFX")]
        [SerializeField] private EventReference _laserSFX;
        [SerializeField] private EventReference _deathSFX;

        [Header("VFX")] 
        [SerializeField] private ParticleSystem _deathVFX;
        
        [Header("Visual")]
        [SerializeField] private GameObject _visual;
        
        private Coroutine _attackCoroutine;
        private bool _laserEnabled;
        private Vector3 _targetPos;
        private Vector3 _targetOffset = Vector3.zero;
        
        private EventInstance _laserInstance = default;


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
            
            _targetPos = TargetTransform.position;
            IsMoving = true;
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

            if (!SeeTarget) return;

            if (CanAttack && IsMoving)
            {
                if (_attackCoroutine == null)
                {
                    _attackCoroutine = StartCoroutine(CoroutineAttack());
                }
                
                _targetPos = TargetTransform.position;
                if (_targetOffset == Vector3.zero) _targetOffset = transform.position - TargetTransform.position;
                _targetPos += _targetOffset;
                
                DroneTurretLookAt(TargetTransform);
                DisplayLaser(true);
                // play laser sound only once and keep the instance
                if (EqualityComparer<EventInstance>.Default.Equals(_laserInstance, default(EventInstance)))
                {
                    _laserInstance = AudioManager.Instance.PlayAtPosition(_laserSFX, transform.position, true);
                }
                else
                {
                    // update 3D position of the instance so sound follows the drone
                    _laserInstance.set3DAttributes(transform.position.To3DAttributes());
                }
            }
            else
            {
                if (_attackCoroutine != null)
                {
                    StopCoroutine(CoroutineAttack());
                }
                
                _targetPos = TargetTransform.position;
                _targetOffset = Vector3.zero;

                ResetTurretRotation();
                if (_laserEnabled)
                {
                    DisplayLaser(false);
                }
                // stop and release the laser sound if it was playing
                if (!EqualityComparer<EventInstance>.Default.Equals(_laserInstance, default(EventInstance)))
                {
                    AudioManager.Instance.Stop(_laserInstance);
                    _laserInstance = default;
                }
            }
            
            if (SeeTarget && TargetTransform && NavMeshAgent.isOnNavMesh && IsMoving)
            {
                NavMeshAgent.SetDestination(_targetPos);
            }
        }
        
        private IEnumerator CoroutineAttack()
        {
            while (IsAttacking)
            {
                yield return new WaitForSeconds(AttackSpeed);
                TargetHealth.TakeDamage(Damage);
                Debug.Log("Drone Done Damage");
            }
        }
        
        private void Die()
        {
            IsMoving = false;
            CanAttack = false;
            IsAttacking = false;
            NavMeshAgent.ResetPath();
            StopCoroutine(CoroutineAttack());
            DisplayLaser(false);
            Collider.enabled = false;
            Visual.SetActive(false);
            
            
            // VFX, SFX
            if (_deathVFX) _deathVFX.Play();
            _visual.SetActive(false);
            AudioManager.Instance.PlayAtPosition(_deathSFX, transform.position);
            IsDead = true;
            Destroy(gameObject, 3f);
        }

        private void DisplayLaser(bool isActive)
        {
            if (isActive)
            {
                _laserLine.enabled = true;
                _laserLine.positionCount = 2;
                _laserLine.SetPosition(0, _startingPos.position);
                _laserLine.SetPosition(1, TargetTransform.position + Vector3.up * 0.8f);
                _laserEnabled = true;
            }
            else
            {
                _laserLine.enabled = false;
                _laserEnabled = false;
            }
        }

        private void DroneTurretLookAt(Transform target)
        {
            Vector3 dir = target.position - _turretTransform.position;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            _turretTransform.rotation = Quaternion.Slerp(
                _turretTransform.rotation,
                targetRot,
                TimeManager.Instance.DeltaTime * _rotateSpeed
            );
        }

        private void ResetTurretRotation()
        {
            Quaternion targetRot = _turretTransform.parent.rotation;
            
            _turretTransform.rotation = Quaternion.Slerp(
                _turretTransform.rotation,
                targetRot,
                TimeManager.Instance.DeltaTime * _rotateSpeed
            );
        }
        
        private void OnEnable()
        {
            EventBus.OnGameResume += OnGameResume;
            EventBus.OnGamePause += OnGamePause;
            EventBus.OnGameOver += OnGamePause;
        }
        
        private void OnGameResume()
        {
            IsMoving = true;
            // NavMeshAgent.SetDestination(TargetTransform.position);
            // if (CanAttack) _attackCoroutine = StartCoroutine(CoroutineAttack());
        }

        private void OnGamePause()
        {
            IsMoving = false;
            NavMeshAgent.ResetPath();
            if (CanAttack)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
        
        private void OnDisable()
        {
            EventBus.OnGameResume -= OnGameResume;
            EventBus.OnGamePause -= OnGamePause;
            EventBus.OnGameOver -= OnGamePause;
        }
    }
}
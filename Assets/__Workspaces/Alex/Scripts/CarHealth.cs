using Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Game;
using Utils.Interfaces;

namespace __Workspaces.Alex.Scripts
{
    public class CarHealth : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        public float maxHealth = 100f;

        [SerializeField] private float _currentHealth;
        
        [Header("VFX")]
        [SerializeField] private ParticleSystem _healVFX;
    
        // internal state to avoid triggering the half repeatedly
        private bool _hasTriggeredHalf = false;
        
        // Is shield active
        public bool IsShieldActive = false;


        private void Start()
        {
            _currentHealth = maxHealth;
            EventBus.OnPlayerHealthChange?.Invoke(_currentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (IsShieldActive)
                return;
            _currentHealth -= damage;
            EventBus.OnPlayerHealthChange?.Invoke(_currentHealth, maxHealth);
            
            if (!_hasTriggeredHalf && _currentHealth <= maxHealth / 2)
            {
                EventBus.OnPlayerAtHalfHealth?.Invoke();
                _hasTriggeredHalf = true;
                // TODO: smoke VFX to show the car is starting to be broken
            }

            if (_currentHealth <= 0)
            {
                // TODO: explosion VFX and car destruction
                EventBus.OnGameOver?.Invoke();
            }
            
            // Change material
            float targetValue;
            if (damage <= 10f)
            {
                targetValue = 0.5f;
            }
            else
            {
                targetValue = 1f;
            }
            DOTween.To(
                () => 0f,
                value =>
                {
                    GameManager.Instance.CurrentTurretMaterials[0].SetFloat("_HitProgress", value);
                    GameManager.Instance.CurrentCarMaterials[0].SetFloat("_HitProgress", value);
                    GameManager.Instance.CurrentIemExhaustPipeMaterials[0].SetFloat("_HitProgress", value);
                },
                targetValue,
                0.1f
            ).SetLoops(2, LoopType.Yoyo);
        }

        public void Heal()
        {
            _currentHealth = maxHealth;
            _hasTriggeredHalf = false;
            EventBus.OnPlayerRecoveredFromHalf?.Invoke();
            EventBus.OnPlayerHealthChange?.Invoke(_currentHealth, maxHealth);
            // VFX
            if (_healVFX) _healVFX.Play();
        }
    }
}

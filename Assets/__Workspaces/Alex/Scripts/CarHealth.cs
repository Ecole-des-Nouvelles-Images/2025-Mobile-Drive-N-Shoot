using System;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;

public class CarHealth : MonoBehaviour, IDamageable
{
    [Header("Health")] 
    public float maxHealth = 100f;

    private float CurrentHealth;
    
    // internal state to avoid triggering the half repeatedly
    private bool _hasTriggeredHalf = false;


    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (!_hasTriggeredHalf && CurrentHealth <= maxHealth / 2)
        {
            EventBus.OnPlayerAtHalfHealth?.Invoke();
            _hasTriggeredHalf = true;
            // TODO: smoke VFX to show the car is starting to be broken
        }

        if (CurrentHealth <= 0)
        {
            EventBus.OnGameOver?.Invoke();
        }
    }

    public void Heal()
    {
        CurrentHealth = maxHealth;
        _hasTriggeredHalf = false;
        EventBus.OnPlayerRecoveredFromHalf?.Invoke();
    }
}

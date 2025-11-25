using System;
using UnityEngine;
using Utils.Game;
using Utils.Interfaces;

public class CarHealth : MonoBehaviour, IDamageable
{
    [Header("Health")] 
    public float maxHealth = 100f;

    private float CurrentHealth;


    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= maxHealth / 2)
        {
            EventBus.OnPlayerAtHalfHealth?.Invoke();
            // TODO: smoke VFX to show the car is starting to be broken
        }

        if (CurrentHealth <= 0)
        {
            EventBus.OnGameOver?.Invoke();
        }
    }
}

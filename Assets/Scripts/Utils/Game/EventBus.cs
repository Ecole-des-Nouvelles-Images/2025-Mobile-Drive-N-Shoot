using System;
using UnityEngine;

namespace Utils.Game
{
    public static class EventBus
    {
        // Game
        public static Action OnGameStart;
        public static Action OnGamePause;
        public static Action OnGameResume;
        public static Action OnGameOver;
        
        // Timer
        public static Action<float> OnTimerChange;
        public static Action OnStartTimer;
        public static Action OnStopTimer;
        public static Action OnResetTimer;

        // Player
        public static Action OnPlayerSpawn;
        public static Action OnPlayerDie;
        public static Action<float> OnPlayerTakeDamage;
        public static Action OnPlayerDealCritDamage;
        /// <summary>
        /// Max Health, Current Health
        /// </summary>
        public static Action<float, float> OnPlayerHealthChange;

        // Enemy
        public static Action OnEnemySpawn;
        public static Action OnSpiderEnemySpawn;
        public static Action OnDroneEnemySpawn;
        public static Action<GameObject> OnEnemyDie;
        public static Action<float> OnEnemyTakeDamage;
        
        // Terrain
        public static Action<Terrain> GenerationTerrainFinished;
    }
}

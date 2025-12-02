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
        public static Action<int> OnLoadScene;
        
        // Time
        public static Action<float> OnTimeChange;
        public static Action OnStartTime;
        public static Action OnStopTime;
        public static Action OnResetTime;

        // Player
        public static Action OnPlayerSpawn;
        public static Action OnPlayerDie;
        public static Action<float> OnPlayerTakeDamage;
        public static Action OnPlayerDealCritDamage;
        /// <summary>
        /// Current Health, Max Health
        /// </summary>
        public static Action<float, float> OnPlayerHealthChange;

        public static Action OnPlayerAtHalfHealth;
        public static Action OnPlayerRecoveredFromHalf;

        // Enemy
        public static Action OnEnemySpawn;
        public static Action OnSpiderEnemySpawn;
        public static Action OnDroneEnemySpawn;
        public static Action<GameObject> OnEnemyDie;
        public static Action<float> OnEnemyTakeDamage;
        
        // Terrain
        public static Action<Terrain> GenerationTerrainFinished;
        
        // Timer
        public static Action OnAddTimeToTimer;
        
        // Items
        public static Action<Item> OnCollectedItem;
        public static Action<ItemType> OnUsingItem;
    }
}
using System;
using __Workspaces.Alex.Scripts;
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
        /// <summary>
        /// Timer, Cooldown
        /// </summary>
        public static Action<float, float> OnPlayerBoostCooldown;

        public static Action OnPlayerAtHalfHealth;
        public static Action OnPlayerRecoveredFromHalf;

        // Enemy
        public static Action OnEnemySpawn;
        public static Action OnSpiderEnemySpawn;
        public static Action OnDroneEnemySpawn;
        public static Action<GameObject> OnEnemyDie;
        public static Action<float> OnEnemyTakeDamage;
        
        // Module
        public static Action OnModuleFinishedGeneration;
        
        // Timer
        public static Action OnAddTimeToTimer;
        
        // Items
        public static Action<Item> OnCollectedItem;
        public static Action<ItemType> OnUsingItem;
        
        // Cinematic
        public static Action OnCinematicEnd;
        
        // Camera
        public static Action OnBigExplosion;
        public static Action OnSmallExplosion;

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            OnGameStart = null;
            OnGamePause = null;
            OnGameResume = null;
            OnGameOver = null;
            OnLoadScene = null;
            OnTimeChange = null;
            OnStartTime = null;
            OnStopTime = null;
            OnResetTime = null;
            OnPlayerSpawn = null;
            OnPlayerDie = null;
            OnPlayerTakeDamage = null;
            OnPlayerDealCritDamage = null;
            OnPlayerHealthChange = null;
            OnPlayerBoostCooldown = null;
            OnPlayerAtHalfHealth = null;
            OnPlayerRecoveredFromHalf = null;
            OnEnemySpawn = null;
            OnSpiderEnemySpawn = null;
            OnDroneEnemySpawn = null;
            OnEnemyDie = null; 
            OnEnemyTakeDamage = null;
            OnModuleFinishedGeneration = null;
            OnAddTimeToTimer = null;
            OnCollectedItem = null; 
            OnUsingItem = null;
            OnCinematicEnd = null;
            OnBigExplosion = null;
            OnSmallExplosion = null;
        }
    }
}
using UnityEngine;
using Utils.Game;

namespace InGameHandlers
{
    public class TimerHandler : MonoBehaviour
    {
        public float Timer {get; private set;}
        
        [Header("Settings")]
        [SerializeField] private float _startTimerValue;
        [SerializeField] private float _timeToAdd;
        
        private bool _isActive;

        private void Awake()
        {
            Timer = _startTimerValue;
            Invoke(nameof(GameStart), 0.2f);
        }

        private void OnEnable()
        {
            EventBus.OnCinematicEnd += OnCinematicEnd;
            EventBus.OnAddTimeToTimer += AddTimeToTimer;
        }

        private void OnDisable()
        {
            EventBus.OnCinematicEnd -= OnCinematicEnd;
            EventBus.OnAddTimeToTimer -= AddTimeToTimer;
        }

        private void Update()
        {
            if (!_isActive) return;
            
            Timer -= TimeManager.Instance.DeltaTime;

            if (Timer <= 0)
            {
                EventBus.OnGameOver?.Invoke();
                _isActive = false;
            }
        }
        
        private void OnCinematicEnd()
        {
            _isActive = true;
        }
        
        private void AddTimeToTimer()
        {
            Timer += _timeToAdd;
        }

        private void GameStart()
        {
            EventBus.OnGameStart?.Invoke();
        }
    }
}
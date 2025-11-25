using UnityEngine;
using Utils.Game;

namespace InGameHandlers
{
    public class TimerHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _startTimerValue;
        [SerializeField] private float _timeToAdd;
        
        private float _timer;
        private bool _isActive = true;
        
        public float GetTimerValue()
        {
            return _timer;
        }

        private void Awake()
        {
            _timer = _startTimerValue;
            Invoke(nameof(GameStart), 0.2f);
        }

        private void OnEnable()
        {
            EventBus.OnAddTimeToTimer += AddTimeToTimer;
        }

        private void OnDisable()
        {
            EventBus.OnAddTimeToTimer -= AddTimeToTimer;
        }

        private void Update()
        {
            if (!_isActive) return;
            
            _timer -= TimeManager.Instance.DeltaTime;

            if (_timer <= 0)
            {
                EventBus.OnGameOver?.Invoke();
                _isActive = false;
            }
        }
        
        private void AddTimeToTimer()
        {
            _timer += _timeToAdd;
        }

        private void GameStart()
        {
            EventBus.OnGameStart?.Invoke();
        }
    }
}
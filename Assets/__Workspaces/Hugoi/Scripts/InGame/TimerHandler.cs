using UnityEngine;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts.InGame
{
    public class TimerHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _startTimerValue;
        [SerializeField] private float _timeToAdd;
        
        
        [SerializeField] private float _timer;

        private void Awake()
        {
            _timer = _startTimerValue;
            EventBus.OnGameStart?.Invoke();
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
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                EventBus.OnGameOver?.Invoke();
            }
        }
        
        private void AddTimeToTimer()
        {
            _timer += _timeToAdd;
        }
    }
}
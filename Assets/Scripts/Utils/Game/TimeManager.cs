using System;
using System.Collections;
using System.Collections.Generic;
using Utils.Singletons;

namespace Utils.Game
{
    public class TimeManager : MonoBehaviourSingletonDontDestroyOnLoad<TimeManager>
    {
        private float _time;
        private float _fixedDeltaTime;
        private float _deltaTime;
        private float _timeScale;
        private float _previousTimeScale;
        private Dictionary<string, float> _timers = new();
    
        public float Time => _time;
        public float FixedDeltaTime => _fixedDeltaTime;
        public float DeltaTime => _deltaTime;
    
        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = value;
        }
    
        /*
         * API
         */
    
        public void Pause()
        {
            TimeScale = 0;
        }

        public void Resume()
        {
            TimeScale = 1;
        }

        /// <summary>
        /// Start a timer updated using TimeManager scale
        /// </summary>
        /// <param name="timerName">the key of the timer</param>
        public void StartTimer(string timerName)
        {
            _timers.Add(timerName, 0);
        }

        /// <summary>
        /// Retrieve the timer current value
        /// </summary>
        /// <param name="timerName">the key of the timer</param>
        /// <returns>the current time</returns>
        public float GetTimer(string timerName)
        {
            _timers.TryGetValue(timerName, out float value);
            return value;
        }
    
        /// <summary>
        /// Use this to remove time from dictionary of timer
        /// </summary>
        /// <param name="timerName">name of the of dictionary</param>
        /// <returns></returns>
        public float RemoveTimer(string timerName)
        {
            _timers.Remove(timerName, out float value);
            return value;
        }
    
        /// <summary>
        /// Use this to trigger the callback after waiting time value
        /// </summary>
        /// <param name="time">the duration of the wait</param>
        /// <param name="callback">the callback to launch after the wait is over</param>
        public void StartWait(float time, Action callback = null)
        {
            StartCoroutine(nameof(Wait), (time, callback));
        }
    
        private IEnumerator Wait((float, Action) args)
        {
            float pauseTime = Time;
            while (Time < pauseTime + args.Item1)
            {
                yield return null;
            }
            if (args.Item2 != null) args.Item2.Invoke();
        }
    
        /*
         * Privates methods
         */
    
        private void Update()
        {
            _time += UnityEngine.Time.deltaTime * _timeScale;
            _fixedDeltaTime = UnityEngine.Time.fixedDeltaTime * _timeScale;
            _deltaTime = UnityEngine.Time.deltaTime * _timeScale;
        
            // Timer increment
            foreach (KeyValuePair<string, float> valuePair in _timers)
            {
                _timers[valuePair.Key] = valuePair.Value + _deltaTime;
            }
        }
    
        private void OnEnable()
        {
            EventBus.OnGameStart += OnGameStart;
            EventBus.OnGameResume += OnGameResume;
            EventBus.OnGamePause += OnGamePause;
            EventBus.OnPlayerDie += OnPlayerDead;
            EventBus.OnGameOver += OnGameOver;
        }

        private void OnGameStart()
        {
            TimeScale = 1;
        }
    
        private void OnGameResume()
        {
            Resume();
        }
    
        private void OnGamePause()
        {
            Pause();
        }

        private void OnPlayerDead()
        {
            Pause();
        }
    
        private void OnGameOver()
        {
            Pause();
        }

        private void OnDisable()
        {
            EventBus.OnGameStart -= OnGameStart;
            EventBus.OnGameResume -= OnGameResume;
            EventBus.OnGamePause -= OnGamePause;
            EventBus.OnPlayerDie -= OnPlayerDead;
            EventBus.OnGameOver -= OnGameOver;
        }
    }
}
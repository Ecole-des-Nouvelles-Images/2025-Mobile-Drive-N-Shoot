using Car;
using UnityEngine;
using Utils.Game;

namespace InGameHandlers
{
    public class PauseHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel;
        
        public void Pause()
        {
            EventBus.OnGamePause?.Invoke();
        }

        public void Resume()
        {
            EventBus.OnGameResume?.Invoke();
        }

        #region ===== EVENTS =====

        private void OnEnable()
        {
            PlayerInputHandler.OnStarting += OnStarting;
        }
        
        private void OnDisable()
        {
            PlayerInputHandler.OnStarting -= OnStarting;
        }

        private void OnStarting(float value)
        {
            if (value > 0)
            {
                Pause();
                _pausePanel.SetActive(true);
            }
        }

        #endregion
    }
}
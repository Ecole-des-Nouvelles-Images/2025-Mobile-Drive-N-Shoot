using UnityEngine;
using Utils.Game;

namespace InGameHandlers
{
    public class PauseHandler : MonoBehaviour
    {
        public void Pause()
        {
            EventBus.OnGamePause?.Invoke();
        }

        public void Resume()
        {
            EventBus.OnGameResume?.Invoke();
        }
    }
}
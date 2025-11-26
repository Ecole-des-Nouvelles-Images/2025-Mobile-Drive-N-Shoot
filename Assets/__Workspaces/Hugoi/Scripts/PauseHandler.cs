using UnityEngine;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts
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
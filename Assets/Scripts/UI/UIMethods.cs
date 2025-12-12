using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIMethods : MonoBehaviour
    {
        public void LoadScene(int id)
        {
            SceneManager.LoadScene(id);
        }

        // public void GameStart()
        // {
        //     EventBus.OnGameStart?.Invoke();
        // }
        //
        // public void GamePause()
        // {
        //     EventBus.OnGamePause?.Invoke();
        // }
        //
        // public void GameResume()
        // {
        //     EventBus.OnGameResume?.Invoke();
        // }
        //
        // public void GameOver()
        // {
        //     EventBus.OnGameOver?.Invoke();
        // }
    }
}
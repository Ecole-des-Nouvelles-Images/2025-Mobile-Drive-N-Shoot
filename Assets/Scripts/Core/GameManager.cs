using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Game;
using Utils.Singletons;

namespace Core
{
    public class GameManager : MonoBehaviourSingletonDontDestroyOnLoad<GameManager>
    {
        [Header("Player Data")]
        public Material[] CurrentCarMaterials;
        public Material[] CurrentTurretMaterials;

        private void OnEnable()
        {
            EventBus.OnGameOver += OnGameOver;
        }
        
        private void OnDisable()
        {
            EventBus.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            SceneManager.LoadScene(0);
        }
    }
}
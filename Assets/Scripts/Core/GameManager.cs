using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Game;
using Utils.Singletons;

namespace Core
{
    public class GameManager : MonoBehaviourSingletonDontDestroyOnLoad<GameManager>
    {
        [Header("Player Data")]
        public GameObject Player;
        public Material[] CurrentCarMaterials;
        public Material[] CurrentTurretMaterials;
        public Material[] CurrentIemExhaustPipeMaterials;

        private void OnEnable()
        {
            EventBus.OnLoadScene += LoadScene;
        }

        private void OnDisable()
        {
            EventBus.OnLoadScene -= LoadScene;
        }

        private void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}
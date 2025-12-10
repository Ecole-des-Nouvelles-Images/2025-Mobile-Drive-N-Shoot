using __Workspaces.Alex.Scripts;
using FMODUnity;
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

        [Header("Sounds")] 
        [SerializeField] private EventReference _ambiantSound;

        [SerializeField] private EventReference _musicSound;
        

        private void OnEnable()
        {
            EventBus.OnLoadScene += LoadScene;
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
            // Play ambiant sound
            AudioManager.Instance.Play(_ambiantSound, true);
            // Play music sound
            AudioManager.Instance.Play(_musicSound, true);
        }
        
        private void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            CurrentCarMaterials[0].SetFloat("_ResistanceProgress", 0f);
            CurrentTurretMaterials[0].SetFloat("_ResistanceProgress", 0f);
            CurrentIemExhaustPipeMaterials[0].SetFloat("_ResistanceProgress", 0f);
            CurrentCarMaterials[0].SetFloat("_HitProgress", 0f);
            CurrentTurretMaterials[0].SetFloat("_HitProgress", 0f);
            CurrentIemExhaustPipeMaterials[0].SetFloat("_HitProgress", 0f);
        }

        private void OnDisable()
        {
            EventBus.OnLoadScene -= LoadScene;
            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
        }
    }
}
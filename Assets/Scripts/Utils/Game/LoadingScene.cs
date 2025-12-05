using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utils.Game
{
    public class LoadingScene : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _loadingBarMaxSpeed;
        [SerializeField] private float _pointsSpeed;
        
        [Header("References")]
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Image _loadingBarFill;
        [SerializeField] private TextMeshProUGUI _loadingText;

        public void LoadScene(int sceneId)
        {
            StartCoroutine(LoadSceneAsync(sceneId));
            StartCoroutine(TextAnimation());
        }
        
        private IEnumerator LoadSceneAsync(int sceneId)
        {
            _loadingScreen.SetActive(true);

            while (_loadingBarFill.fillAmount < 1f)
            {
                _loadingBarFill.fillAmount += Random.Range(0f, _loadingBarMaxSpeed) * Time.deltaTime;
                yield return null;
            }

            SceneManager.LoadScene(sceneId);
        }

        private IEnumerator TextAnimation()
        {
            string[] steps = { "", ".", ". .", ". . .", };
            int index = 0;

            while (_loadingBarFill.fillAmount < 1f)
            {
                _loadingText.text = steps[index];
                index = (index + 1) % steps.Length;

                yield return new WaitForSeconds(_pointsSpeed);
            }
        }

        // private IEnumerator LoadSceneAsync(int sceneId)
        // {
        //     AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        //     _loadingScreen.SetActive(true);
        //
        //     while (!operation.isDone)
        //     {
        //         float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
        //         _loadingBarFill.fillAmount = progressValue;
        //         yield return null;
        //     }
        // }
    }
}

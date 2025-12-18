using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Utils.Game
{
    public class LoadingScene : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _loadingBarMaxSpeed = 1f;
        [SerializeField] private float _pointsSpeed = 0.3f;
        
        [Header("References")]
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Image _loadingBarFill;
        [SerializeField] private TextMeshProUGUI _loadingText;

        private void Start()
        {
            StartCoroutine(LoadSceneAsync());
            StartCoroutine(TextAnimation());
        }
        
        private IEnumerator LoadSceneAsync()
        {
            while (_loadingBarFill.fillAmount < 1f)
            {
                _loadingBarFill.fillAmount += Random.Range(0f, _loadingBarMaxSpeed) * Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(0.5f);
            _loadingScreen.SetActive(false);
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
    }
}

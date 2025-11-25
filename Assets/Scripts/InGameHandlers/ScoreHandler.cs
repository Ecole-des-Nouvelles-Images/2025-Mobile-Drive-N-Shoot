using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventBus = Utils.Game.EventBus;

namespace InGameHandlers
{
    public class ScoreHandler : MonoBehaviour
    {
        [Header("References Display")]
        [SerializeField] private GameObject _panelScore;
        [SerializeField] private TextMeshProUGUI _tmpScore;
        
        [Header("References Data")]
        [SerializeField] private DistanceHandler _distanceHandler;
        
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

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
            _image.raycastTarget = true;
            _panelScore.SetActive(true);
            
            // Set le score
            _tmpScore.text = _distanceHandler.GetDistanceValue().ToString("F1");
        }
    }
}

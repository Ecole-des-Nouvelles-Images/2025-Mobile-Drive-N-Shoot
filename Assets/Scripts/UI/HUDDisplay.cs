using InGameHandlers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDDisplay : MonoBehaviour
    {
        [Header("References Display")]
        [SerializeField] private TextMeshProUGUI _tmpTimer;
        [SerializeField] private TextMeshProUGUI _tmpDistance;
        
        [Header("References Data")]
        [SerializeField] private TimerHandler _timerHandler;
        [SerializeField] private DistanceHandler _distanceHandler;
        
        private void Update()
        {
            _tmpTimer.text = _timerHandler.Timer.ToString("F1");
            _tmpDistance.text = _distanceHandler.Distance.ToString("F1");
        }
    }
}
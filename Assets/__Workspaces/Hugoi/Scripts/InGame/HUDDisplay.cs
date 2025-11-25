using TMPro;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts.InGame
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
            _tmpTimer.text = _timerHandler.GetTimerValue().ToString("F1");
            _tmpDistance.text = _distanceHandler.GetDistanceValue().ToString("F1");
        }
    }
}

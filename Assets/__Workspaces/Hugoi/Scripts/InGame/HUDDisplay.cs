using TMPro;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts.InGame
{
    public class HUDDisplay : MonoBehaviour
    {
        [Header("References Display")]
        [SerializeField] private TextMeshProUGUI _tmpTimer;
        
        [Header("References Logic")]
        [SerializeField] private TimerHandler _timerHandler;

        private void Update()
        {
            _tmpTimer.text = _timerHandler.GetTimerValue().ToString("F1");
        }
    }
}

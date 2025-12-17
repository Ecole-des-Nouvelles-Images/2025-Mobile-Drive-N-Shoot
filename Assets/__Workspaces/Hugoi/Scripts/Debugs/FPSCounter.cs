using TMPro;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts.Debugs
{
    public class FPSCounter : MonoBehaviour
    {
        private TextMeshProUGUI _fpsText;
        private float _timer;

        private void Awake()
        {
            _fpsText = GetComponent<TextMeshProUGUI>();
            
            // Application.targetFrameRate = 60;
            // QualitySettings.vSyncCount = 0;
        }

        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= 0.1f)
            {
                float fps = 1f / Time.deltaTime;
                _fpsText.text = $"FPS : {fps:F0}";
                _timer = 0f;
            }
        }
    }
}
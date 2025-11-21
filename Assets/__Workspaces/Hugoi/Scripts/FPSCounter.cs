using TMPro;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class FPSCounter : MonoBehaviour
    {
        public TextMeshProUGUI fpsText;
        private float timer = 0f;

        private void Awake()
        {
            Application.targetFrameRate = 90;
            QualitySettings.vSyncCount = 0;
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= 0.1f)
            {
                float fps = 1f / Time.deltaTime;
                fpsText.text = $"FPS : {fps:F0}";
                timer = 0f;
            }
        }
    }
}
using UnityEngine;

namespace UI
{
    [ExecuteAlways]
    public class FitCanvasToCamera : MonoBehaviour
    {
        public Camera targetCamera;
        public float distance = 5f;

        private void Update()
        {
            if (!targetCamera) return;

            transform.position = targetCamera.transform.position + targetCamera.transform.forward * distance;
            transform.rotation = targetCamera.transform.rotation;

            RectTransform rt = GetComponent<RectTransform>();

            float frustumHeight = 2.0f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * targetCamera.aspect;

            rt.sizeDelta = new Vector2(frustumWidth, frustumHeight);
        }
    }
}
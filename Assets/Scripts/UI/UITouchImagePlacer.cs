using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UITouchImagePlacer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _canvasObject;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject go = Instantiate(_canvasObject);
                go.GetComponent<UITouchImage>().Setup(Input.mousePosition);
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                GameObject go = Instantiate(_canvasObject);
                go.GetComponent<UITouchImage>().Setup(Input.GetTouch(0).position);
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}
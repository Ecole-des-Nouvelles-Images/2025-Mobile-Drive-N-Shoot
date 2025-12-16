using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UITouchImage : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _scaleEndValue = 2f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private AnimationCurve _animationCurve;
        
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Transform _image;
        
        public void Setup(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                screenPosition,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out Vector2 localPos
            );

            _image.localPosition = localPos;
            
            _image.DOScale(_scaleEndValue, _duration).SetEase(_animationCurve).SetLoops(2, LoopType.Yoyo);
            Destroy(gameObject, _duration * 2f);
        }
    }
}

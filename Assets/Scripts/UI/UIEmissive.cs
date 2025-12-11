using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIEmissive : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _durationFade = 1.2f;
        [SerializeField] private float _minAlpha = 0f;
        [SerializeField] private float _maxAlpha = 1f;
        
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            Color color = _image.color;
            color.a = _minAlpha;
            _image.color = color;
            
            _image.DOFade(_maxAlpha, _durationFade).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }
}

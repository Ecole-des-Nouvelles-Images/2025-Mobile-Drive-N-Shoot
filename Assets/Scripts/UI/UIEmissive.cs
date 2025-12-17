using System;
using DG.Tweening;
using TMPro;
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
        private TextMeshProUGUI _tmp;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _tmp = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (_image)
            {
                Color color = _image.color;
                color.a = _minAlpha;
                _image.color = color;
            
                _image.DOFade(_maxAlpha, _durationFade).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }

            if (_tmp)
            {
                Color color = _tmp.color;
                color.a = _minAlpha;
                _tmp.color = color;
            
                _tmp.DOFade(_maxAlpha, _durationFade).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}
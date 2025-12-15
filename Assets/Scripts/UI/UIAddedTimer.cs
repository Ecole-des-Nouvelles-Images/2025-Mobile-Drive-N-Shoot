using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIAddedTimer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _scaleEndValue;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private Vector3 _nextPosition;
        
        private RectTransform _rectTransform;
        private TextMeshProUGUI _text;

        public void Setup(Vector3 position, Vector3 nextPosition, int value)
        {
            GetComponent<RectTransform>().anchoredPosition = position;
            _nextPosition = nextPosition;
            GetComponent<TextMeshProUGUI>().text = value.ToString();
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            transform.DOScale(_scaleEndValue, _duration).SetEase(_animationCurve).SetLoops(2, LoopType.Yoyo);
            Invoke(nameof(ChangePosition), _duration);
        }

        private void ChangePosition()
        {
            _rectTransform.DOAnchorPos(_nextPosition, _duration).SetEase(_animationCurve);
            _text.DOFade(0f, _duration).SetEase(_animationCurve);
            Invoke(nameof(AnimationStop), _duration + 0.1f);
        }

        private void AnimationStop()
        {
            _rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
            var color = _text.color;
            color.a = 1f;
            _text.color = color;
            gameObject.SetActive(false);
        }
    }
}
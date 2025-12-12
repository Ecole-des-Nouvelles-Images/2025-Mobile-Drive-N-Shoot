using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAnimationOnClick : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Button _button;
        [SerializeField] private float _endScaleValue;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _animationCurve;
        
        private void OnEnable()
        {
            _button.onClick.AddListener(() => ButtonIsClicked());
        }

        private void ButtonIsClicked()
        {
            _button.GetComponent<Transform>().DOScale(_endScaleValue, _duration).SetEase(_animationCurve).SetLoops(2, LoopType.Yoyo);
        }
    }
}

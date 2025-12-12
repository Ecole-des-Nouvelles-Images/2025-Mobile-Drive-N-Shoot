using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILastSelectedButton : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite _spriteLeftSelected;
        [SerializeField] private Sprite _spriteLeftNotSelected;
        [SerializeField] private Sprite _spriteRightSelected;
        [SerializeField] private Sprite _spriteRightNotSelected;
        
        [Header("Text")]
        [SerializeField] private Sprite _spriteTextPickup;
        [SerializeField] private Sprite _spriteTextTurret;

        [Header("Button Animation")]
        [SerializeField] private float _endScaleValue;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _animationCurve;
        
        [Header("References")]
        [SerializeField] private Button _buttonLeft;
        [SerializeField] private Button _buttonRight;
        [SerializeField] private Image _imageButtonSelected;
        
        private void OnEnable()
        {
            _buttonLeft.onClick.AddListener(() => ButtonIsClicked(_buttonLeft));
            _buttonRight.onClick.AddListener(() => ButtonIsClicked(_buttonRight));
            
            ButtonIsClicked(_buttonLeft);
        }

        private void ButtonIsClicked(Button button)
        {
            if (button == _buttonLeft)
            {
                _buttonLeft.image.overrideSprite = _spriteLeftSelected;
                _buttonRight.image.overrideSprite = _spriteRightNotSelected;
                _imageButtonSelected.sprite = _spriteTextPickup;
                
                _buttonLeft.GetComponent<Transform>().DOScale(_endScaleValue, _duration).SetEase(_animationCurve);
                _buttonRight.GetComponent<Transform>().DOScale(1f, _duration).SetEase(_animationCurve);
            }
            else
            {
                _buttonRight.image.overrideSprite = _spriteRightSelected;
                _buttonLeft.image.overrideSprite = _spriteLeftNotSelected;
                _imageButtonSelected.sprite = _spriteTextTurret;
                
                _buttonRight.GetComponent<Transform>().DOScale(_endScaleValue, _duration).SetEase(_animationCurve);
                _buttonLeft.GetComponent<Transform>().DOScale(1f, _duration).SetEase(_animationCurve);
            }
        }
    }
}

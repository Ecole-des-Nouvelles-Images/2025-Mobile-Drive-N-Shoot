using Core;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts
{
    public class StickPanelHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector3 _leftPos;
        [SerializeField] private Vector3 _rightPos;
        
        [Header("References")]
        [SerializeField] public RectTransform _joystickMovement;
        [SerializeField] public RectTransform _buttonShoot;

        private void OnEnable()
        {
            if (GameManager.Instance.StickPos == 0)
            {
                _joystickMovement.position = _leftPos;
                _buttonShoot.position = _rightPos;
            }
            else
            {
                _joystickMovement.position = _rightPos;
                _buttonShoot.position = _leftPos;
            }
            
            EventBus.OnChangedStickPos += OnChangedStickPos;
        }
        
        private void OnDisable()
        {
            EventBus.OnChangedStickPos -= OnChangedStickPos;
        }

        private void OnChangedStickPos(int pos)
        {
            if (pos == 0)
            {
                _joystickMovement.position = _leftPos;
                _buttonShoot.position = _rightPos;
            }
            else
            {
                _joystickMovement.position = _rightPos;
                _buttonShoot.position = _leftPos;
            }

            Destroy(_joystickMovement.gameObject.GetComponent<OnScreenStick>());
            OnScreenStick stick = _joystickMovement.gameObject.AddComponent<OnScreenStick>();
            stick.controlPath = "<Gamepad>/leftStick";
        }
    }
}

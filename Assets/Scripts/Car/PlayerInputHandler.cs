using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utils.Game;

namespace Car
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Settings")]
        public bool InputAreEnable = true;
        [FormerlySerializedAs("_playerInput")] public PlayerInput PlayerInput;
        
        public static event Action<bool> OnInputDeviceChanged;
        
        // STATIC EVENT
        public static Action<Vector2> OnMove;
        public static Action<float> OnFire;
        public static Action<float> OnItemOne;
        public static Action<float> OnItemTwo;
        public static Action<float> OnItemThree;
        public static Action<float> OnAccelerate;
        public static Action<float> OnBackward;
        public static Action<float> OnBoost;
        public static Action<float> OnStarting;
        
        private CarControler _playerController;
        
        private bool _isControllerConnected;
        
        private Vector2 _joystickReadValue;
        private float _westButtonReadValue;
        private float _leftButtonReadValue;

        private void Awake()
        {
            _playerController = GetComponent<CarControler>();
        }

        private void OnEnable()
        {
            UnityEngine.InputSystem.InputSystem.onDeviceChange += OnDeviceChange;

            // Bind input actions
            PlayerInput.actions["Move"].performed += Move;
            PlayerInput.actions["Move"].canceled += Move;
            PlayerInput.actions["Fire"].performed += Fire;
            PlayerInput.actions["Fire"].canceled += Fire;
            PlayerInput.actions["ItemOne"].performed += ItemOne;
            PlayerInput.actions["ItemOne"].canceled += ItemOne;
            PlayerInput.actions["ItemTwo"].performed += ItemTwo;
            PlayerInput.actions["ItemTwo"].canceled += ItemTwo;
            PlayerInput.actions["ItemThree"].performed += ItemThree;
            PlayerInput.actions["ItemThree"].canceled += ItemThree;
            PlayerInput.actions["Accelerate"].performed += Accelerate;
            PlayerInput.actions["Accelerate"].canceled += Accelerate;
            PlayerInput.actions["Backward"].performed += Backward;
            PlayerInput.actions["Backward"].canceled += Backward;
            PlayerInput.actions["Boost"].performed += Boost;
            PlayerInput.actions["Boost"].canceled += Boost;
            PlayerInput.actions["Start"].performed += Starting;
            PlayerInput.actions["Start"].canceled += Starting;
            
            // Game
            EventBus.OnGamePause += GamePaused;
            EventBus.OnGameResume += GameResumed;
        }

        private void OnDisable()
        {
            UnityEngine.InputSystem.InputSystem.onDeviceChange -= OnDeviceChange;

            // Unbind input actions
            PlayerInput.actions["Move"].performed -= Move;
            PlayerInput.actions["Move"].canceled -= Move;
            PlayerInput.actions["Fire"].performed -= Fire;
            PlayerInput.actions["Fire"].canceled -= Fire;
            PlayerInput.actions["ItemOne"].performed -= ItemOne;
            PlayerInput.actions["ItemOne"].canceled -= ItemOne;
            PlayerInput.actions["ItemTwo"].performed -= ItemTwo;
            PlayerInput.actions["ItemTwo"].canceled -= ItemTwo;
            PlayerInput.actions["ItemThree"].performed -= ItemThree;
            PlayerInput.actions["ItemThree"].canceled -= ItemThree;
            PlayerInput.actions["Accelerate"].performed -= Accelerate;
            PlayerInput.actions["Accelerate"].canceled -= Accelerate;
            PlayerInput.actions["Backward"].performed -= Backward;
            PlayerInput.actions["Backward"].canceled -= Backward;
            PlayerInput.actions["Boost"].performed -= Boost;
            PlayerInput.actions["Boost"].canceled -= Boost;
            PlayerInput.actions["Start"].performed += Starting;
            PlayerInput.actions["Start"].canceled += Starting;
            
            // Game
            EventBus.OnGamePause -= GamePaused;
            EventBus.OnGameResume -= GameResumed;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed) DetectCurrentInputDevice();
        }

        private void DetectCurrentInputDevice()
        {
            _isControllerConnected = Gamepad.all.Count > 0;
            OnInputDeviceChanged?.Invoke(_isControllerConnected);

            Debug.Log(_isControllerConnected
                ? "Controller connected: Switching to Gamepad controls."
                : "No controller connected: Switching to Keyboard/Mouse controls.");
        }
        
        private void Move(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnMove?.Invoke(context.ReadValue<Vector2>());
                Debug.Log($"Move {context.ReadValue<Vector2>()}");
            }
        }
        
        private void Fire(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnFire?.Invoke(context.ReadValue<float>());
                Debug.Log($"Fire {context.ReadValue<float>()}");
            }
        }
        
        private void ItemOne(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnItemOne?.Invoke(context.ReadValue<float>());
                Debug.Log($"ItemOne {context.ReadValue<float>()}");
            }
        }
        
        private void ItemTwo(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnItemTwo?.Invoke(context.ReadValue<float>());
                Debug.Log($"ItemTwo {context.ReadValue<float>()}");
            }
        }

        private void ItemThree(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnItemThree?.Invoke(context.ReadValue<float>());
                Debug.Log($"ItemThree {context.ReadValue<float>()}");
            }
        }
        
        private void Accelerate(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnAccelerate?.Invoke(context.ReadValue<float>());
                Debug.Log($"Accelerate {context.ReadValue<float>()}");
            }
        }
        
        private void Backward(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnBackward?.Invoke(context.ReadValue<float>());
                Debug.Log($"Backward {context.ReadValue<float>()}");
            }
        }
        
        private void Boost(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnBoost?.Invoke(context.ReadValue<float>());
                Debug.Log($"Boost {context.ReadValue<float>()}");
            }
        }
        
        private void Starting(InputAction.CallbackContext context)
        {
            if (InputAreEnable)
            {
                OnStarting?.Invoke(context.ReadValue<float>());
                Debug.Log($"Starting {context.ReadValue<float>()}");
            }
        }
        
        private void GamePaused()
        {
            InputAreEnable = false;
        }
        
        private void GameResumed()
        {
            InputAreEnable = true;
        }
    }
}
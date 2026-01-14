using UnityEngine;
using Utils.Game;

namespace UI
{
    public class UILookCamera : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Transform _rotationTarget;
        
        private Transform _mainCameraTransform;

        void Start()
        {
            if (UnityEngine.Camera.main) _mainCameraTransform = UnityEngine.Camera.main.transform;
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
                    _mainCameraTransform.rotation * Vector3.up);
            
            _rotationTarget.transform.Rotate(0, 0, _rotationSpeed * TimeManager.Instance.DeltaTime);
        }
    }
}

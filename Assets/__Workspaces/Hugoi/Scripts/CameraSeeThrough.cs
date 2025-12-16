using Core;
using DG.Tweening;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class CameraSeeThrough : MonoBehaviour
    {
        [Header("Material")]
        [SerializeField] private Material _material;
        [SerializeField] private LayerMask _layerMask;
        
        private Transform _targetTransform;

        private bool _isActivated;

        private void Start()
        {
            _targetTransform = GameManager.Instance.Player.transform;
        }

        private void Update()
        {
            Vector3 direction = (_targetTransform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _targetTransform.position);
            
            bool hitSomething = Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, _layerMask);

            if (hitSomething && !_isActivated)
            {
                SetOpacity(0f, 0.95f, 0.2f);
                _isActivated = true;
            }
            else if (!hitSomething && _isActivated)
            {
                SetOpacity(0.95f, 0f, 0.2f);
                _isActivated = false;
            }
        }
        
        private void SetOpacity(float startValue, float endValue, float duration)
        {
            DOTween.To(
                () => startValue,
                value =>
                {
                    _material.SetFloat("_Opacity", value);
                },
                endValue,
                duration
            );
        }
    }
}
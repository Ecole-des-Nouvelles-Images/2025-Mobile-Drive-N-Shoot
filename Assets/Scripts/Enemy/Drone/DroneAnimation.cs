using UnityEngine;
using UnityEngine.AI;
using Utils.Game;

namespace Enemy.Drone
{
    public class DroneAnimation : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _rotationHelixSpeed;
        [SerializeField] private float _bodyRotationMultiplier;
        [SerializeField] private float _bodyRotationSpeed;
        
        [Header("References")]
        [SerializeField] private NavMeshAgent _navMeshAgent;
        
        [Header("Transforms")]
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _leftHelix;
        [SerializeField] private Transform _rightHelix;

        private Vector3 _direction;

        private void Update()
        {
            _leftHelix.transform.Rotate(0f, _rotationHelixSpeed * TimeManager.Instance.DeltaTime, 0f);
            _rightHelix.transform.Rotate(0f, _rotationHelixSpeed * TimeManager.Instance.DeltaTime, 0f);

            _direction = _navMeshAgent.velocity;
            _direction.y = 0;

            if (_direction.sqrMagnitude > 0.5f)
            {
                Vector3 tiltDir = _direction.normalized + Vector3.down * -_bodyRotationMultiplier;
                Quaternion targetRot = Quaternion.LookRotation(tiltDir);
                _body.rotation = Quaternion.Lerp(_body.rotation, targetRot, TimeManager.Instance.DeltaTime * _bodyRotationSpeed);
            }
            else
            {
                Quaternion targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
                _body.rotation = Quaternion.Lerp(_body.rotation, targetRot, TimeManager.Instance.DeltaTime * _bodyRotationSpeed / 2f);
            }
        }
    }
}
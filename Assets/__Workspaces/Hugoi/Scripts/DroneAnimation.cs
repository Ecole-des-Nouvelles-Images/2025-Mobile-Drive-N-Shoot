using UnityEngine;
using UnityEngine.AI;

namespace __Workspaces.Hugoi.Scripts
{
    public class DroneAnimation : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _rotationHelixSpeed;
        
        [Header("References")]
        [SerializeField] private NavMeshAgent _navMeshAgent;
        
        [Header("Transforms")]
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _leftHelix;
        [SerializeField] private Transform _rightHelix;

        private void Update()
        {
            _leftHelix.transform.Rotate(0f, _rotationHelixSpeed * Time.deltaTime, 0f);
            _rightHelix.transform.Rotate(0f, _rotationHelixSpeed * Time.deltaTime, 0f);
        }
    }
}

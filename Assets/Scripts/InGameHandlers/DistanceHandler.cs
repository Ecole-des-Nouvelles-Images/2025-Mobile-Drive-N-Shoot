using UnityEngine;

namespace InGameHandlers
{
    public class DistanceHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _player;

        private float _distance;
        
        private Vector2 _playerPosition;
        private Vector2 _lastPlayerPosition;

        private void Start()
        {
            _lastPlayerPosition = _player.position;
        }

        private void Update()
        {
            _playerPosition = new Vector2(_player.position.x, _player.position.z);

            if ((_lastPlayerPosition - _playerPosition).sqrMagnitude >= 1f && _playerPosition.y > _lastPlayerPosition.y)
            {
                _distance++;
                _lastPlayerPosition = _playerPosition;
            }
        }
        
        public float GetDistanceValue()
        {
            return _distance;
        }
    }
}
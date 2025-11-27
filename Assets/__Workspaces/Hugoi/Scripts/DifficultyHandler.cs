using InGameHandlers;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class DifficultyHandler : MonoBehaviour
    {
        public int Difficulty {get; private set;}
        
        [Header("Settings")]
        [SerializeField] private int _distanceDifficultyScaling;
            
        [Header("References Data")]
        [SerializeField] private DistanceHandler _distanceHandler;

        private int _lastDistance;

        private void Update()
        {
            int distance = (int)_distanceHandler.GetDistanceValue();

            if (_distanceDifficultyScaling <= distance - _lastDistance)
            {
                Difficulty++;
                _lastDistance = distance;
            }
        }
    }
}

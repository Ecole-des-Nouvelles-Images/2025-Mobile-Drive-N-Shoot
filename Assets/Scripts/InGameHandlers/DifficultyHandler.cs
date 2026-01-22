using UnityEngine;

namespace InGameHandlers
{
    public class DifficultyHandler : MonoBehaviour
    {
        public int Difficulty {get; private set;}
        
        [Header("Settings")]
        public int DistanceDifficultyScaling;
            
        [Header("References Data")]
        [SerializeField] private DistanceHandler _distanceHandler;

        private int _lastDistance;

        private void Update()
        {
            int distance = (int)_distanceHandler.Distance;

            if (DistanceDifficultyScaling <= distance - _lastDistance)
            {
                Difficulty++;
                _lastDistance = distance;
            }
        }
    }
}

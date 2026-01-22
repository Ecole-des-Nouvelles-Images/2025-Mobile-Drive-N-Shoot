using MapGeneration;
using UnityEngine;

namespace InGameHandlers
{
    public class EnemiesSpawnerHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _difficultyScaling;
        
        private DynamicSplineProps[] _splineEnemies;
        
        private void Awake()
        {
            _splineEnemies = GetComponentsInChildren<DynamicSplineProps>();
        }

        public void Setup(int difficulty, int distanceDifficultyScaling)
        {
            for (int i = 0; i < _splineEnemies.Length; i++)
            {
                // _splineEnemies[i].SetDensity((int)(difficulty * _difficultyScaling));
                
                int density = Mathf.RoundToInt(Mathf.Log(difficulty / (float)distanceDifficultyScaling) * _difficultyScaling);
                _splineEnemies[i].SetDensity(density);
            }
        }
    }
}
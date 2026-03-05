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

        public void Setup(int difficulty)
        {
            for (int i = 0; i < _splineEnemies.Length; i++)
            {
                int density = Mathf.RoundToInt(Mathf.Log10(difficulty) * _difficultyScaling);
                
                if (density < 1)
                {
                    density = 1;
                }
                
                _splineEnemies[i].SetDensity(density);
            }
        }
    }
}
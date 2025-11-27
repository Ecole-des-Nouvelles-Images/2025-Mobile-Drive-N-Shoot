using MapGeneration;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
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
                _splineEnemies[i].SetDensity((int)(difficulty * _difficultyScaling));
            }
        }
    }
}
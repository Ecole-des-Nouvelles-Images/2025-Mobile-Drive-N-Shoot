using UnityEngine;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;

namespace MapGeneration
{
    public class SplineKnotHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private uint _seed;
        [SerializeField] private int _knotCount;
        [SerializeField] private int _terrainSize;
        [SerializeField] private int _nextPosOffset;
        
        [Header("References")]
        [SerializeField] private SplineContainer _splineContainer;
        
        private Vector3 _lastPos;

        private Random _random;
        
        [ContextMenu("DebugBuildRoad")]
        public void DebugGenerateSpline()
        {
            GenerateSpline(_seed);
        }
        
        public void GenerateSpline(uint seed)
        {
            _seed = seed;
            _random = new Random(_seed);
            
            _splineContainer.Spline.Clear();
            
            float space = _terrainSize / (_knotCount - 1);
            for (float i = 0; i <= _terrainSize; i += space)
            {
                Vector3 newPos = new Vector3();
                if (i == 0 || i == _terrainSize)
                {
                    newPos = new Vector3(0, 0, i);
                }
                else
                {
                    int xOffset = _random.NextInt(-_nextPosOffset, _nextPosOffset);
                    newPos = new Vector3(_lastPos.x + xOffset, 0, i);
                    newPos.x = Mathf.Clamp(newPos.x, -40, 40);
                }
                
                Vector3 tangentIn = new Vector3(0, 0, -5);
                Vector3 tangentOut = new Vector3(0, 0, 5);
                
                BezierKnot newBezierKnot = new BezierKnot(newPos, tangentIn, tangentOut);
                _splineContainer.Spline.Add(newBezierKnot);
                
                _lastPos = newPos;
            }
        }
    }
}
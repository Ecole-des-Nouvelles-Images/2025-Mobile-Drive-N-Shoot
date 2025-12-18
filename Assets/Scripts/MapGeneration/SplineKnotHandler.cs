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

        private Random _random;
        private Vector3 _lastPos;

        private const float TANGENT_Z = 5f;
        private const float CLAMP_X = 40f;

        [ContextMenu("DebugBuildRoad")]
        public void DebugGenerateSpline()
        {
            GenerateSpline(_seed);
        }

        public void GenerateSpline(uint seed)
        {
            _seed = seed;
            _random = new Random(_seed);

            Spline spline = _splineContainer.Spline;
            spline.Clear();

            _lastPos = Vector3.zero;

            int knotCount = Mathf.Max(2, _knotCount);
            float stepZ = (float)_terrainSize / (knotCount - 1);

            Vector3 tangentIn = new Vector3(0f, 0f, -TANGENT_Z);
            Vector3 tangentOut = new Vector3(0f, 0f, TANGENT_Z);

            for (int i = 0; i < knotCount; i++)
            {
                float z = i * stepZ;
                Vector3 newPos;

                if (i == 0 || i == knotCount - 1)
                {
                    newPos = new Vector3(0f, 0f, z);
                }
                else
                {
                    int xOffset = _random.NextInt(-_nextPosOffset, _nextPosOffset);
                    float x = Mathf.Clamp(_lastPos.x + xOffset, -CLAMP_X, CLAMP_X);
                    newPos = new Vector3(x, 0f, z);
                }

                spline.Add(new BezierKnot(newPos, tangentIn, tangentOut));
                _lastPos = newPos;
            }
        }
    }
}
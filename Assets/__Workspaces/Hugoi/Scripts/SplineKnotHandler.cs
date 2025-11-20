using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace __Workspaces.Hugoi.Scripts
{
    [RequireComponent( typeof(SplineContainer))]
    public class SplineKnotHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _knotCount;
        [SerializeField] private int _terrainSize;
        [SerializeField] private int _nextPosOffset;
        
        private SplineContainer _splineContainer;
        private Vector3 _lastPos;
        
        private void Awake()
        {
            _splineContainer = GetComponent<SplineContainer>();
        }
        
        [ContextMenu("GenerateSpline")]
        public void GenerateSpline()
        {
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
                    int xOffset = Random.Range(-_nextPosOffset, _nextPosOffset);
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
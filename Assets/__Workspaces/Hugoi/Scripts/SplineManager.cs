using UnityEngine;
using UnityEngine.Splines;

namespace __Workspaces.Hugoi.Scripts
{
    [RequireComponent( typeof(SplineContainer))]
    public class SplineManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _knotCount;
        [SerializeField] private int _terrainSize;
        
        [Header("References")]
        [SerializeField] private SplineContainer _splineContainer;
        
        private void OnEnable()
        {
            _splineContainer = GetComponent<SplineContainer>();
            GenerateSpline();
        }
        
        [ContextMenu("GenerateSpline")]
        private void GenerateSpline()
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
                    newPos = new Vector3(Random.Range(-40, 40), 0, i);
                }
                
                Vector3 tangentIn = new Vector3(0, 0, -5);
                Vector3 tangentOut = new Vector3(0, 0, 5);
                
                BezierKnot newBezierKnot = new BezierKnot(newPos, tangentIn, tangentOut);
                _splineContainer.Spline.Add(newBezierKnot);
            }
        }
    }
}
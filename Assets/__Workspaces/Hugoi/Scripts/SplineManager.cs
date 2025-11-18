using UnityEngine;
using UnityEngine.Splines;

namespace __Workspaces.Hugoi.Scripts
{
    [RequireComponent( typeof(SplineContainer))]
    public class SplineManager : MonoBehaviour
    {
        public SplineContainer SplineContainer;

        public int KnotCount;
        public int TerrainSize;
        
        private void OnEnable()
        {
            SplineContainer = GetComponent<SplineContainer>();
        }

        [ContextMenu("GenerateSpline")]
        private void GenerateSpline()
        {
            SplineContainer.Spline.Clear();

            float space = TerrainSize / (KnotCount - 1);
            for (float i = 0; i <= TerrainSize; i += space)
            {
                Vector3 newPos = new Vector3();
                if (i == 0 || i == TerrainSize)
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
                SplineContainer.Spline.Add(newBezierKnot);
            }
        }
    }
}
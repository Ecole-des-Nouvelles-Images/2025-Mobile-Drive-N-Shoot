using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    [ExecuteAlways]
    public class DestroyIfOverlapping : MonoBehaviour
    {
        [SerializeField] private float _overlapRadius;
        [SerializeField] private LayerMask _overlapLayers;

        private void OnEnable()
        {
            Invoke(nameof(DestroyImmediate), 0.5f);
        }

        private void DestroyImmediate()
        {
            Collider[] overlaps = Physics.OverlapSphere(transform.position, _overlapRadius, _overlapLayers);

            foreach (Collider col in overlaps)
            {
                if (col.gameObject != gameObject)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _overlapRadius);
        }
    }
}

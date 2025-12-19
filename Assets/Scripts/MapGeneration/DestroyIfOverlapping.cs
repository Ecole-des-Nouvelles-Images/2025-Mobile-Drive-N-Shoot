using UnityEngine;
using Utils.Pooling;

namespace MapGeneration
{
    public class DestroyIfOverlapping : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _overlapRadius;
        [SerializeField] private LayerMask _overlapLayers;

        private void OnEnable()
        {
            CheckOverlap();
        }

        private void CheckOverlap()
        {
            Collider[] overlaps = Physics.OverlapSphere(transform.position, _overlapRadius, _overlapLayers);

            foreach (Collider col in overlaps)
            {
                if (col.gameObject != gameObject)
                {
                    ObjectPoolingManager.ReturnObjectToPool(col.gameObject);
                    return;
                }
            }
        }

        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(transform.position, _overlapRadius);
        // }
    }
}

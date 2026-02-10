using System.Collections.Generic;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class TriggerCollider : MonoBehaviour
    {
        [SerializeField] private List<Collider> _colliders = new List<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            _colliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _colliders.Remove(other);
            _colliders.RemoveAll(collider => collider == null);
            
            // _animalData.LastFoodsTargets.Remove(other.gameObject);
        }

        public GameObject GetNearestObject(string targetTag)
        {
            _colliders.RemoveAll(collider => collider == null);
            
            GameObject nearestObject = null;
            float nearestDistance = Mathf.Infinity;
            
            foreach (Collider col in _colliders)
            {
                if (col.CompareTag(targetTag))
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestObject = col.gameObject;
                    }
                }
            }
            
            return nearestObject;
        }
    }
}
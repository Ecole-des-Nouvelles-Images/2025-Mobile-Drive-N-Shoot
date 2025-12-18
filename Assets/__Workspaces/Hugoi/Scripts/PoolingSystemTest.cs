using UnityEngine;
using Utils.Pooling;

namespace __Workspaces.Hugoi.Scripts
{
    public class PoolingSystemTest : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _parent;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ObjectPoolingManager.SpawnObject(_prefab, _parent, new Vector3(1f, 0f, 0f), Quaternion.identity);
            }
        }
    }
}
using UnityEngine;
using Utils.Pooling;

namespace __Workspaces.Hugoi.Scripts
{
    public class DestroyAfterTime : MonoBehaviour
    {
        private void OnEnable()
        {
            Invoke(nameof(Destroy), 2f);
        }

        private void Destroy()
        {
            ObjectPoolingManager.ReturnObjectToPool(gameObject);
        }
    }
}
using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    public class TestSpawnEnemy : MonoBehaviour
    {
        public GameObject Enemy;
        private void Start()
        {
            Instantiate(Enemy, transform.position, Quaternion.identity);
        }
    }
}

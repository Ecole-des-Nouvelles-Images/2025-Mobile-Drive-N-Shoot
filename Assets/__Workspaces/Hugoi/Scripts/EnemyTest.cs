using UnityEngine;
using Utils.Interfaces;

namespace __Workspaces.Hugoi.Scripts
{
    public class EnemyTest : MonoBehaviour, IDamageable
    {
        public void TakeDamage(float damage)
        {
            Debug.Log("Damaged");
        }
    }
}

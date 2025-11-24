using UnityEngine;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts.InGame
{
    public class CheckPointHandler : MonoBehaviour
    {
        private bool _alreadyPass;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !_alreadyPass)
            {
                EventBus.OnAddTimeToTimer?.Invoke();
                _alreadyPass = true;
            }
        }
    }
}

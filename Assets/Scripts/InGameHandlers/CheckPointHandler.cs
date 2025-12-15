using UnityEngine;
using Utils.Game;

namespace InGameHandlers
{
    public class CheckPointHandler : MonoBehaviour
    {
        private bool _alreadyPass;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !_alreadyPass)
            {
                EventBus.OnAddTimeToTimer?.Invoke();
                EventBus.OnPlayerPassCheckpoint?.Invoke();
                _alreadyPass = true;
            }
        }
    }
}

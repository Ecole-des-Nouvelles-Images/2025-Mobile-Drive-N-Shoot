using __Workspaces.Alex.Scripts;
using FMODUnity;
using UnityEngine;

namespace MapGeneration
{
    public class PropsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _visual;
        [SerializeField] private ParticleSystem _psDestroy;
        [SerializeField] private EventReference _destroySFX;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_psDestroy) _psDestroy.Play();
                _visual.SetActive(false);
                AudioManager.Instance.PlayAtPosition(_destroySFX, transform.position);
                Destroy(gameObject, 2f);
            }
        }
    }
}
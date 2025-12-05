using UnityEngine;

namespace MapGeneration
{
    public class PropsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _visual;
        [SerializeField] private ParticleSystem _psDestroy;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_psDestroy) _psDestroy.Play();
                _visual.SetActive(false);
                Destroy(gameObject, 2f);
            }
        }
    }
}
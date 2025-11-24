using UnityEngine;

namespace SelectionSkins
{
    public class RotationCarVisual : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 150f;

        private float previousX = 0f;
        private bool isSliding = false;
        
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    rb.angularVelocity = Vector3.zero;
                    
                    previousX = touch.position.x;
                    isSliding = true;
                }
                else if (touch.phase == TouchPhase.Moved && isSliding)
                {
                    float deltaX = touch.position.x - previousX;
                    previousX = touch.position.x;

                    Rotate(deltaX);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isSliding = false;
                }
            }
        }

        private void Rotate(float deltaX)
        {
            float rotationAmount = -deltaX * rotationSpeed * Time.deltaTime;
            Vector3 rotation = new Vector3(0, rotationAmount, 0);

            rb.AddRelativeTorque(rotation, ForceMode.Impulse);
        }
    }
}
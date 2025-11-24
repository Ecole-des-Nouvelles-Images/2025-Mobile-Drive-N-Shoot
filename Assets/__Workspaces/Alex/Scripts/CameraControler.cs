using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public Transform Player;
    public Vector3 Offset;
    public float Speed;
    private Rigidbody _playerRigidBody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerRigidBody = Player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerForward = (_playerRigidBody.linearVelocity + Player.forward).normalized;
        transform.position = Vector3.Lerp(transform.position,
            Player.position + Player.transform.TransformVector(Offset) + playerForward * (-5f), Speed * Time.deltaTime);
        transform.LookAt(Player);
    }
}

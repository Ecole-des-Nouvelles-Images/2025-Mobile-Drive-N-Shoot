using UnityEngine;
using UnityEngine.AI;
using Utils.Game;
using Utils.Interfaces;

public class DroneControler : MonoBehaviour, IEnemy, IDamageable
{
    [Header("Target")] public Transform target;

    [Header("Health")] public float health = 50f;

    [Header("Movement")] public float speed = 15f;

    [Header("Weapon")] public Transform weaponPivot; // weapon pivot point
    public float weaponRotationSpeed = 10f;
    public Transform firePoint;

    [Header("Laser")] public LineRenderer lineRenderer;
    public float laserRange = 20f;
    public float damagePerSecond = 5f;
    public LayerMask hitMask = ~0;

    // Health
    private float _currentHealth;

    private NavMeshAgent agent;

    private Vector3 _targetPos;
    private Vector3 _targetOffset = Vector3.zero;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        _targetPos = target.transform.position;

        agent.updateRotation = true;
        agent.speed = speed;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (target == null) return;

        // Drone follows target
        agent.SetDestination(_targetPos);

        // Weapon aims target
        AimWeaponAtTarget();

        // Laser ON/OFF
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= laserRange)
            FireLaser();
        else
            StopLaser();
    }

    void AimWeaponAtTarget()
    {
        if (!weaponPivot || !target)
            return;

        Vector3 toTarget = target.position - weaponPivot.position;
        if (toTarget.sqrMagnitude < 0.0001f) return;

        // Rotation toward the target
        Quaternion desiredRot = Quaternion.LookRotation(toTarget);

        weaponPivot.rotation = Quaternion.Slerp(
            weaponPivot.rotation,
            desiredRot,
            weaponRotationSpeed * TimeManager.Instance.DeltaTime
        );
    }

    void FireLaser()
    {
        if (!firePoint || !lineRenderer) return;

        _targetPos = target.transform.position;
        if (_targetOffset == Vector3.zero)
        {
            _targetOffset = transform.position - target.position;
        }

        _targetPos += _targetOffset;


        lineRenderer.enabled = true;

        // Fire from the weapon forward direction
        Vector3 origin = firePoint.position;
        Vector3 dir = firePoint.forward;

        // Laser max length
        float rayLength = laserRange;
        Vector3 endPoint = origin + dir * rayLength;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, rayLength, hitMask))
        {
            endPoint = hit.point;

            // Damage only if the hit is the target
            if (hit.transform == target)
            {
                var health = hit.collider.GetComponent<CarHealth>();
                if (health != null)
                    health.TakeDamage(damagePerSecond * TimeManager.Instance.DeltaTime);
            }
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);
    }

    void StopLaser()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        _targetPos = target.transform.position;
        _targetOffset = Vector3.zero;
    }

    void OnDisable()
    {
        StopLaser();
    }

    public void SetupEnemy(Transform target)
    {
        this.target = target;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            // TODO : explosion VFX
            Destroy(this.gameObject);
        }
    }
}
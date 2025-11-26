using UnityEngine;
using UnityEngine.AI;
using Utils.Game;

public class DroneControler : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movement")]
    public float speed = 15f;

    [Header("Weapon")]
    public Transform weaponPivot;  // weapon pivot point
    public float weaponRotationSpeed = 10f;
    public Transform firePoint;

    [Header("Laser")]
    public LineRenderer lineRenderer;
    public float laserRange = 20f;
    public float damagePerSecond = 5f;
    public LayerMask hitMask = ~0;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        agent.updateRotation = true;
        agent.speed = speed;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (target == null) return;

        // Drone follows target
        agent.SetDestination(target.position);

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
        if (weaponPivot == null || target == null)
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
        if (firePoint == null || lineRenderer == null) return;

        lineRenderer.enabled = true;

        Vector3 origin = firePoint.position;
        Vector3 toTarget = target.position - origin;
        float targetDist = toTarget.magnitude;
        Vector3 dir = toTarget.normalized;
        float rayLength = Mathf.Min(laserRange, targetDist);

        RaycastHit hit;
        Vector3 endPoint = origin + dir * rayLength;

        if (Physics.Raycast(origin, dir, out hit, rayLength, hitMask))
        {
            endPoint = hit.point;

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
    }

    void OnDisable()
    {
        StopLaser();
    }
}

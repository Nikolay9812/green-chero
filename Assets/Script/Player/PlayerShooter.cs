using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerShooter : MonoBehaviour
{
    public Arrow arrowPrefab;

    public int damage = 25;
    public float fireRate = 2f;
    public float range = 15f;
    public float turnSpeed = 720f;
    public float aimTolerance = 10f;
    public Vector3 shootOffset = new Vector3(0f, -0.3f, 0.6f);

    public int frontArrows = 1;
    public float frontArrowSpacing = 0.4f;

    public Enemy CurrentTarget { get; private set; }

    private PlayerMovement movement;
    private float cooldown;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (movement.IsMoving)
        {
            CurrentTarget = null;
            return;
        }

        CurrentTarget = FindNearestEnemy();
        if (CurrentTarget == null)
        {
            return;
        }

        Vector3 toEnemy = CurrentTarget.transform.position - transform.position;
        toEnemy.y = 0f;
        if (toEnemy.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(toEnemy);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        bool isAimed = Quaternion.Angle(transform.rotation, targetRotation) <= aimTolerance;

        if (isAimed && cooldown <= 0f)
        {
            cooldown = 1f / fireRate;
            Shoot(toEnemy.normalized);
        }
    }

    Enemy FindNearestEnemy()
    {
        Enemy nearest = null;
        float nearestDistance = range * range;

        foreach (Enemy enemy in Enemy.All)
        {
            if (enemy.IsDead) continue;

            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    void Shoot(Vector3 direction)
    {
        Vector3 right = Vector3.Cross(Vector3.up, direction);
        for (int i = 0; i < frontArrows; i++)
        {
            float offset = (i - (frontArrows - 1) / 2f) * frontArrowSpacing;
            SpawnArrow(direction, right * offset);
        }
    }

    void SpawnArrow(Vector3 direction, Vector3 sideOffset)
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("PlayerShooter: Arrow Prefab is not set in Inspector");
            return;
        }
        
        Vector3 spawnPosition = transform.position + Vector3.up * shootOffset.y + direction * shootOffset.z + sideOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(direction);


        Arrow arrow = Instantiate(arrowPrefab, spawnPosition, spawnRotation);
        arrow.damage = damage;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

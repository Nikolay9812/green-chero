using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 25;
    public float lifetime = 3f;

    void Awake()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        foreach (Collider col in GetComponentsInChildren<Collider>()) col.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null || enemy.IsDead) return;

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}

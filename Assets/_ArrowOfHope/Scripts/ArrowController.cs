using UnityEngine;

public class ArrowController : MonoBehaviour
{

    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 2f;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.velocity = transform.up * -1 * speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject,0.05f);
        }
    }

}

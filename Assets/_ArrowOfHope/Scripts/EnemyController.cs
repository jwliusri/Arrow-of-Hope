using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Enemy enemyData;
    [SerializeField] private float healthpoint;

    private Rigidbody2D rb;
    private bool isMoveToTarget = true;
    private float attackCooldownTimer = 0;
    private bool isAttackOnCooldown => attackCooldownTimer > 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveToTarget)
        {
            MoveForward();
        }
    }

    private void FixedUpdate()
    {
        if (isAttackOnCooldown) attackCooldownTimer -= Time.fixedDeltaTime;
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.up, enemyData.AttackRange, 1 << 6); // Wall layer
        Debug.DrawRay(transform.position, transform.up * enemyData.AttackRange, Color.white);
        if (ray.collider != null)
        {
            Debug.DrawRay(transform.position, ray.point - (Vector2)transform.position, Color.red);
            if (isMoveToTarget )
            {
                StopMovement();
            }
            if (!isAttackOnCooldown)
            {
                attackCooldownTimer = enemyData.AttackCooldownSecond;
                var wall = ray.collider.GetComponent<WallController>();
                wall.TakeDamage(enemyData.Damage);
            }

        }
        else
        {
            isMoveToTarget = true;
        }
    }

    public void SetEnemyData(Enemy enemy)
    {
        enemyData = enemy;
        healthpoint = enemyData.Healthpoint;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemyData.Sprite;
        GetComponent<BoxCollider2D>().size = spriteRenderer.size;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} take {damage} damage");
        healthpoint -= damage;
        if (healthpoint <= 0)
        {
            Die();
        }
    }

    private void MoveForward()
    {
        rb.velocity = transform.up * enemyData.Speed;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        isMoveToTarget = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}

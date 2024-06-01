using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Enemy enemyData;
    [SerializeField] private float healthpoint;
    [SerializeField] private Color damageFlashColor = Color.white;
    [SerializeField] private float damageFlashTime = 0.25f;
    [SerializeField] private float staggerTime = 0.25f;

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D coll;
    private FlashEffect flashEffect;
    private AudioSource audioSource;

    private bool isMoveToTarget = true;
    private bool isDead = false;
    private float attackCooldownTimer = 0;
    private bool isAttackOnCooldown => attackCooldownTimer > 0;
    private float staggerTimer = 0;
    private bool isStagger => staggerTimer > 0;
    private bool isPathObstructed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        flashEffect = GetComponentInChildren<FlashEffect>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (isStagger)
        {
            staggerTimer -= Time.fixedDeltaTime;
            StopMovement();
            return;
        }
        if (isAttackOnCooldown) attackCooldownTimer -= Time.fixedDeltaTime;
        RaycastHit2D[] hit = new RaycastHit2D[1];
        coll.Raycast((Vector2)transform.up, hit, enemyData.AttackRange);
        var ray = hit[0];
        Debug.DrawRay(transform.position, transform.up * enemyData.AttackRange, Color.white);
        if (ray.collider != null)
        {
            if (ray.collider.CompareTag("Wall"))
            {
                Debug.DrawRay(transform.position, ray.point - (Vector2)transform.position, Color.red);
                if (isMoveToTarget)
                {
                    StopMovement();
                }
                if (!isAttackOnCooldown)
                {
                    attackCooldownTimer = enemyData.AttackCooldownSecond;
                    animator.SetTrigger("Attack");
                    var wall = ray.collider.GetComponent<WallController>();
                    wall.TakeDamage(enemyData.Damage);
                }
            }

            isPathObstructed = ray.collider.CompareTag("Enemy");

        }
        else
        {
            isMoveToTarget = true;
        }

        if (isMoveToTarget)
        {
            MoveForward();
        }
    }

    public void SetEnemyData(Enemy enemy)
    {
        enemyData = enemy;
        healthpoint = enemyData.Healthpoint;
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = enemyData.Sprite;
        //GetComponent<BoxCollider2D>().size = spriteRenderer.bounds.size;
        if (coll == null) coll = GetComponent<CapsuleCollider2D>();
        coll.size = spriteRenderer.bounds.size;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        Debug.Log($"{gameObject.name} take {damage} damage");
        healthpoint -= damage;
        staggerTimer = staggerTime;
        flashEffect.Trigger(damageFlashColor, damageFlashTime);
        audioSource.Play();
        if (healthpoint <= 0)
        {
            Die();
        }
    }

    private void MoveForward()
    {
        Vector3 dir;
        if (isPathObstructed)
        {
            bool slideRight = transform.position.x < 0;
            //dir = Vector3.Normalize(transform.up + (transform.right * (slideRight ? 1 : -1)));
            dir = transform.right * (slideRight ? 1 : -1);
            isPathObstructed = false;
        }
        else
        {
            dir = transform.up;
        }
        rb.velocity = dir * enemyData.Speed;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        isMoveToTarget = false;
    }

    private void Die()
    {
        isDead = true;
        coll.enabled = false;
        StopMovement();
        animator.SetTrigger("Die");
        var animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animLength);
    }

}

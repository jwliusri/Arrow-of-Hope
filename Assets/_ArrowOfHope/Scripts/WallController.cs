using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] private float healthpoint = 1f;

    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} take {damage} damage");
        healthpoint -= damage;
        if (healthpoint <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

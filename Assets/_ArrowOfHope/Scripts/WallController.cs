using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] private float maxHealthpoint = 1f;
    [SerializeField] private float healthpoint = 1f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        healthpoint = maxHealthpoint;
    }

    public void TakeDamage(float damage)
    {
        //Debug.Log($"{gameObject.name} take {damage} damage");
        healthpoint -= damage;
        GameController.Instance.uiController.UpdateWallHealthBar(healthpoint/maxHealthpoint);
        audioSource.Play();
        if (healthpoint <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        GameController.Instance.GameOver();
    }
}

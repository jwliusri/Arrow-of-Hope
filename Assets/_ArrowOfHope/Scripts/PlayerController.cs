using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float shootCooldownSecond = 1f;
    [SerializeField] private Transform bowHolder;
    [SerializeField] private Transform pointOfShoot;
    [SerializeField] private GameObject arrowPrefab;


    InputAction attackAction;

    private float shootCooldownTimer = 0f;
    private bool isShootOnCooldown => shootCooldownTimer > 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        if (isShootOnCooldown) shootCooldownTimer -= Time.deltaTime;

        LookAtMouse();

        if (attackAction.IsPressed() && !isShootOnCooldown)
        {
            Shoot();
        }
    }

    void LookAtMouse()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        bowHolder.rotation = rotation;
    }

    void Shoot()
    {
        shootCooldownTimer = shootCooldownSecond;
        GameObject arrow = Instantiate(arrowPrefab, pointOfShoot.position, pointOfShoot.rotation);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] private string id = "enemy-name";
    [SerializeField] private string displayName = "Enemy Name";
    [SerializeField] private float healthpoint = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldownSecond = 1f;
    [SerializeField] private float attackRange = .5f;
    [SerializeField] private Sprite sprite;

    public string Id => id;
    public string DisplayName => displayName;
    public float Healthpoint => healthpoint;
    public float Speed => speed;
    public float Damage => damage;
    public float AttackCooldownSecond => attackCooldownSecond;
    public float AttackRange => attackRange;
    public Sprite Sprite => sprite;
}

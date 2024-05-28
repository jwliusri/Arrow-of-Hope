using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemyList;

    public Enemy Find(string id)
    {
        return enemyList.Find(e => e.Id == id);
    }
}

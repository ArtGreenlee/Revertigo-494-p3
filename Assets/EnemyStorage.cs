using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStorage : MonoBehaviour
{
    public HashSet<GameObject> enemies;

    public void addEnemy(GameObject enemyIn)
    {
        enemies.Add(enemyIn);
    }

    public bool enemyIsAlive(GameObject checkEnemy)
    {
        return enemies.Contains(checkEnemy);
    }
}

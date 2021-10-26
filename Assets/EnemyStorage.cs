using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStorage : MonoBehaviour
{
    public HashSet<GameObject> enemies = new HashSet<GameObject>();
    public void addEnemy(GameObject enemyIn)
    {
        enemies.Add(enemyIn);
    }

    public void removeEnemy(GameObject enemyIn)
    {
        enemies.Remove(enemyIn);
    }

    public bool enemyIsAlive(GameObject checkEnemy)
    {
        return enemies.Contains(checkEnemy);
    }
}

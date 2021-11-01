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

    public bool validWallPosition(Vector3 checkVec)
    {
        foreach (Collider tempCollider in Physics.OverlapBox(checkVec, new Vector3(1f, 1f, 1f))) {
            if (enemies.Contains(tempCollider.gameObject))
            {
                return false;
            }
        }
        return true;
    }

    public GameObject getClosestEnemyToPointWithinRange(Vector3 checkVec, float range)
    {
        GameObject temp = null;
        float minDistance = range;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float curDistance = Vector3.Distance(enemy.transform.position, checkVec);
                if (curDistance < minDistance)
                {
                    temp = enemy;
                    minDistance = curDistance;
                }
            }
        }
        return temp;
    }

    public GameObject getStrongestEnemyWithinRange(Vector3 checkVec, float range)
    {
        GameObject temp = null;
        float maxHealth = float.MaxValue;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float curHealth = enemy.GetComponent<EnemyHealth>().currentHealth;
                if (curHealth < maxHealth)
                {
                    temp = enemy;
                    maxHealth = curHealth;
                }
            }
        }
        return temp;
    }

    public List<GameObject> getAllEnemiesWithinRange(Vector3 checkVec, float range)
    {
        List<GameObject> enemiesInRangeTemp = new List<GameObject>();
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && Vector3.Distance(checkVec, enemy.transform.position) <= range)
            {
                enemiesInRangeTemp.Add(enemy);
            }
        }
        return enemiesInRangeTemp;
    }
}

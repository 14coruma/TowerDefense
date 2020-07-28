using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemies : MonoBehaviour
{
    Transform start; // Start object transform
    Transform stop; // Stop object transform
    public Transform boxObstaclePrefab;

    // Start is called before the first frame update
    void Start()
    {
        start = GameObject.Find("Start").GetComponent<Transform>();
        stop = GameObject.Find("Stop").GetComponent<Transform>();
    }

    public void SendWave(int waveNum, Transform enemyPrefab, int number, float maxHealth,
            int prize, float waveDelay, float enemyDelay) {
        for (int i = 0; i < number; i++) {
            StartCoroutine(NewEnemy(enemyPrefab, maxHealth, prize, i*enemyDelay + waveNum*waveDelay));
        }
    }

    IEnumerator NewEnemy(Transform enemyPrefab, float maxHealth, int prize, float delay) {
        yield return new WaitForSeconds(delay);
        Transform newEnemy = Instantiate(enemyPrefab, start.position, Quaternion.identity);
        newEnemy.transform.parent = transform;
        newEnemy.GetComponent<EnemyAI>().SetStats(maxHealth, prize);
    }

    /// <summary>
    /// Find the enemy which is closet to its target while within the range of a turret
    /// </summary>
    /// <param name="turret">Vector2, the turret's position</param>
    /// <param name="range">float, the turret's range</param>
    /// <param name="typeId">string, the type of enemy</param>
    /// <returns>null or Transform, of the closest enemy</returns>
    public Transform ClosestEnemy(Vector2 turret, float range, string typeId = "Normal") {
        float min = float.MaxValue; // Default turret looking at "Start" object
        Transform minTransform = null;
        foreach (Transform enemy in transform) {
            // Turret => Enemy (which turrets attack which enemies)
            // Normal => Normal,Flying,Tank
            // Tank => Normal,Tank
            // Flying => Flying
            if (!enemy.name.Contains(typeId) && typeId != "Normal" && !(typeId == "Tank" && enemy.name.Contains("Normal"))) {
                continue;
            }
            EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
            float dist2end = enemyScript.distance;
            float dist2turret = Vector2.Distance(enemy.transform.position, turret);
            if (!enemyScript.reachedEndOfPath && dist2end < min && dist2turret < range) {
                min = dist2end;
                minTransform = enemy;
            }
        }
        return minTransform;
    }

    public bool UpdateGridGraph(Collider2D collider) {
        // Update graph with new obstacle
        AstarPath.active.Scan();
        // Check if path from start to target is possible
        GraphNode node1 = AstarPath.active.GetNearest(start.position, NNConstraint.Default).node;
        GraphNode node2 = AstarPath.active.GetNearest(stop.position, NNConstraint.Default).node;
        return PathUtilities.IsPathPossible(node1, node2);
    } 

    public bool NewObstaclePathPossible(Vector2 position) {
        Transform newObstacle = Instantiate(boxObstaclePrefab, position, Quaternion.identity);
        // Check if able to find path, then destroy temporary obstacle
        bool foundPath = UpdateGridGraph(newObstacle.GetComponent<Collider2D>());
        Destroy(newObstacle.gameObject);
        AstarPath.active.Scan();
        return foundPath;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
} 

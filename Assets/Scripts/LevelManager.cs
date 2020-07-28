using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float width = 17f;
    public float height = 7f;
    Enemies enemies;
    public List<string> enemyIds;
    Dictionary<string,Transform> enemyPrefabs;
    string wavesString;
    public float enemyDelay = 1f;
    public float waveDelay = 10f;

    void Start() {
        enemies = GameObject.Find("Enemies").GetComponent<Enemies>();
        wavesString = Resources.Load<TextAsset>("EnemyText/LevelEnemies0").ToString();
        enemyPrefabs = new Dictionary<string, Transform>();
        foreach (string enemyId in enemyIds) {
            enemyPrefabs.Add(enemyId, Resources.Load<GameObject>("Prefabs/Enemy" + enemyId).transform);
        }

        List<List<object>> waves = ParseWaveString();
        for (int i = 0; i < waves.Count; i++) {
            enemies.SendWave(
                i, enemyPrefabs[(string)waves[i][0]], (int)waves[i][1], (float)waves[i][2],
                (int)waves[i][3], waveDelay, enemyDelay
            );
        }
    }
    
    /// EX: """0,5,300,5"""
    /// Enemy0, 5 enemies, 300f health, 5 points each 
    List<List<object>> ParseWaveString() {
        List<List<object>> waves = new List<List<object>>(0);
        string[] wavesSplit = wavesString.Split(
            new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        foreach(string s in wavesSplit) {
            string[] sSplit = s.Split(',');
            List<object> wave = new List<object>(4);
            wave.Add(sSplit[0]);
            wave.Add(int.Parse(sSplit[1]));
            wave.Add(float.Parse(sSplit[2]));
            wave.Add(int.Parse(sSplit[3]));
            waves.Add(wave);
        }
        return waves;
    }

    // Checks if point is inside level boundaries, set as public vars,
    // centered about the LevelManager transform
    public bool PointInsideLevel(Vector2 point) {
        float x = transform.position.x;
        float y = transform.position.y;
        bool insideX = point.x < x + width/2 && point.x > x - width/2;
        bool insideY = point.y < y + height/2 && point.y > y - height/2;
        return insideX && insideY;
    }
}

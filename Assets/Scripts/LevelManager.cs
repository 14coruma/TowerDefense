using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float width = 17f;
    public float height = 7f;
    Enemies enemies;
    public Transform enemyPrefab;
    public string wavesString; // waveString is for easy level editing, parsed into waves
    public float enemyDelay = 1f;
    public float waveDelay = 10f;

    void Start() {
        enemies = GameObject.Find("Enemies").GetComponent<Enemies>();

        List<List<object>> waves = ParseWaveString();
        for (int i = 0; i < waves.Count; i++) {
            enemies.SendWave(
                i, enemyPrefab, (int)waves[i][0], (float)waves[i][1], (int)waves[i][2],
                waveDelay, enemyDelay
            );
        }
    }
    
    /// EX: """5,300,5;7,350,10"""
    /// Wave 0: 5 enemies, 300f health, 5 points each 
    /// Wave 1: 7 enemies, 350f health, 10points each
    List<List<object>> ParseWaveString() {
        List<List<object>> waves = new List<List<object>>(0);
        string[] wavesSplit = wavesString.Split(';');
        foreach(string s in wavesSplit) {
            string[] sSplit = s.Split(',');
            List<object> wave = new List<object>(3);
            wave.Add(int.Parse(sSplit[0]));
            wave.Add(float.Parse(sSplit[1]));
            wave.Add(int.Parse(sSplit[2]));
            waves.Add(wave);
        }
        return waves;
    }
    
    public bool PointInsideLevel(Vector2 point) {
        bool insideX = point.x < width/2 && point.x > 0-width/2;
        bool insideY = point.y < height/2 && point.y > 0-height/2;
        return insideX && insideY;
    }
}

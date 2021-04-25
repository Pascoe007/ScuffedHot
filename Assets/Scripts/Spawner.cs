using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WaveEnemy
{
    public int Amount;
    public GameObject Type;
}

[System.Serializable]
public class Wave
{
    public List<WaveEnemy> Enemy;
}

public class Spawner : MonoBehaviour
{
    public List<Wave> Waves;
    public int CurrentWave = 0;


    void CreateNextEnemyWave()
    {
        var wave = Waves.ElementAtOrDefault(CurrentWave);

        if (wave != null)
        {
            CreateEnemiesFor(wave);
            CurrentWave++;
        }
    }

    void CreateEnemiesFor(Wave wave)
    {
        if (!wave.Enemy.Any()) return;

        foreach (var enemy in wave.Enemy)
        {
            for (int i = 0; i < enemy.Amount; i++)
            {
                Instantiate(enemy.Type, Vector3.zero, Quaternion.identity);
            }
        }
    }

    // To test press spacebar
    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space)) CreateNextEnemyWave();
    }
}

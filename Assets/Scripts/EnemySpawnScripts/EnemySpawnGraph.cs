using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

[CreateAssetMenu]
public class EnemySpawnGraph : ScriptableObject
{
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private int WavesCount;
    [SerializeField]
    private float EnemyCountMultiplier, EnemySpawnInterval, WaveStartInterval, CurveReadDuration;
    [SerializeField]
    private int MaxNumEnemySimultaneously;
    [SerializeField]
    private SOVariableInt CurrentEnemyCount;

    float timeWaveInterval, timeEnemyInterval, timeFromStartSpawning;
    int currentWaveEnemyCount, currentWaveIndex, enemySpawnedCount;
    bool waveSpawning;

    void OnEnable()
    {
        enemySpawnedCount = 0;
        currentWaveEnemyCount = 0;
        currentWaveIndex = 0;
        timeWaveInterval = 0;
        timeEnemyInterval = 0;
        timeFromStartSpawning = 0;
        waveSpawning = true;
    }

    // Update is called once per frame
    public bool GetSpawn(out int numToSpawn, float DeltaTime)
    {
        numToSpawn = 0;

        if (WaveStartInterval != 0)
            return UpdateWaves(ref numToSpawn, DeltaTime);
        else
            return UpdateSpawn(ref numToSpawn, DeltaTime);
    }

    bool UpdateWaves(ref int numToSpawn, float time)
    {
        if (!waveSpawning)
        {
            //Start wave
            timeWaveInterval -= time;
            if (timeWaveInterval <= 0)
            {
                timeWaveInterval = WaveStartInterval;
                ReadCurveByWave();
                waveSpawning = true;
            }
        }
        else
        {
            if (CurrentEnemyCount.Value <= MaxNumEnemySimultaneously)
            {
                //Spawn enemies until num enemies equal to num enemies in current wave
                timeEnemyInterval -= time;
                if (timeEnemyInterval <= 0)
                {
                    timeEnemyInterval = EnemySpawnInterval;

                    if (enemySpawnedCount >= currentWaveEnemyCount)
                    {
                        if (CurrentEnemyCount.Value == 0)
                        {
                            waveSpawning = false;
                            enemySpawnedCount = 0;
                        }
                    }
                    else
                    {
                        enemySpawnedCount++;
                        numToSpawn = 1;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool UpdateSpawn(ref int numToSpawn, float time)
    {
        if (CurrentEnemyCount.Value <= MaxNumEnemySimultaneously)
        {
            timeFromStartSpawning += time;
            timeEnemyInterval -= time;

            if (timeEnemyInterval <= 0)
            {
                timeEnemyInterval = EnemySpawnInterval;

                numToSpawn = ReadCurveByEnemy(timeFromStartSpawning / CurveReadDuration);
                return true;
            }
        }
        return false;
    }

    void ReadCurveByWave()
    {
        currentWaveEnemyCount = (int)(curve.Evaluate(currentWaveIndex == 0 ? 0 : 1f / (float)WavesCount * (float)currentWaveIndex) * EnemyCountMultiplier);
        currentWaveIndex++;
    }

    int ReadCurveByEnemy(float time)
    {
        return (int)(curve.Evaluate(time) * EnemyCountMultiplier);
    }
}

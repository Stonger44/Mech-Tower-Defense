using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject spawnPoint;
    public GameObject endPoint;
    public GameObject standbyPoint;

    public int spawnCount = 10;
    public int wave = 1;

    public bool waveComplete = false;

    public override void Init()
    {
        spawnCount = spawnCount * wave;

        StartCoroutine(SpawnEnemyRoutine(3));
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (PoolManager.Instance.enemyPool.Count == 0)
        {
            waveComplete = true;
        }
    }

    private IEnumerator SpawnEnemyRoutine(int seconds)
    {
        while (!waveComplete)
        {
            yield return new WaitForSeconds(seconds);

            GameObject enemy = PoolManager.Instance.RequestEnemy();

            if (enemy != null)
            {
                //Mech2 (The bigger Mech) is slower, so give it more time before the next spawn
                if (enemy.tag == "Mech1")
                    seconds = 3;
                else
                    seconds = 5;
            }
        }
        
    }

}

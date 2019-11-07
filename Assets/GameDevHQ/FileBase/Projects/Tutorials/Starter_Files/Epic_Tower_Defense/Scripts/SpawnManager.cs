using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject spawnPoint;
    public GameObject endPoint;

    public int enemyCount = 10;
    public int wave = 1;

    public override void Init()
    {
        enemyCount = enemyCount * wave;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject enemy = PoolManager.Instance.RequestEnemy();
        }
    }

}

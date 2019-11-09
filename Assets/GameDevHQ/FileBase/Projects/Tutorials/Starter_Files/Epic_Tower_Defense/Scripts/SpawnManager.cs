using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject spawnPoint;
    public GameObject endPoint;
    public GameObject standbyPoint;

    public int spawnCount;

    [SerializeField] private bool waveComplete = false;

    public override void Init()
    {
        
    }

    private void Start()
    {
        spawnCount = spawnCount * GameManager.Instance.wave;
        waveComplete = GameManager.Instance.waveRunning;
    }

    private void Update()
    {
        //Try to move this out of Update()?
        if (Enemy.enemyCount <= 0)
        {
            waveComplete = true;
        }
    }

    public IEnumerator SpawnEnemyRoutine(int seconds)
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

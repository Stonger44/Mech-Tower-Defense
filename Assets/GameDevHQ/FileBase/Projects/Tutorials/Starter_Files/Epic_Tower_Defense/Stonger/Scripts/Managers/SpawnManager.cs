using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject spawnPoint;
    public GameObject endPoint;
    public GameObject standbyPoint;

    [SerializeField] private bool _waveRunning = false;

    private void OnEnable()
    {
        //Subscribe to events
        GameManager.onStartWave += StartSpawning;
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        GameManager.onStartWave -= StartSpawning;
    }

    public IEnumerator SpawnEnemyRoutine(int seconds)
    {
        while (_waveRunning)
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

            _waveRunning = GameManager.Instance.waveRunning;
        }
        
    }

    private void StartSpawning()
    {
        _waveRunning = GameManager.Instance.waveRunning;
        StartCoroutine(SpawnEnemyRoutine(3));
    }

}

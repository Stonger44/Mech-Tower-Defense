using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject spawnPoint;
    public GameObject endPoint;
    public GameObject standbyPoint;
    public GameObject junkyard;
    [SerializeField] private int _mech1SpawnDelayTime;
    [SerializeField] private int _mech2SpawnDelayTime;

    [SerializeField] private bool _waveRunning = false;
    [SerializeField] private int _spawnDelayTime;
    private bool _isSpawningEnemies = false;

    private GameObject _enemy;

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
        _spawnDelayTime = seconds;

        while (_waveRunning)
        {
            yield return new WaitForSeconds(_spawnDelayTime);

            _enemy = PoolManager.Instance.RequestEnemy();

            if (_enemy != null)
            {
                //Mech2 (The bigger Mech) is slower, so give it more time before the next spawn
                if (_enemy.tag == "Mech1")
                    _spawnDelayTime = _mech1SpawnDelayTime;
                else
                    _spawnDelayTime = _mech2SpawnDelayTime;
            }


            _waveRunning = GameManager.Instance.WaveRunning;
        }

        _isSpawningEnemies = false;


    }

    private void StartSpawning()
    {
        _waveRunning = GameManager.Instance.WaveRunning;

        if (_isSpawningEnemies == false)
        {
            _isSpawningEnemies = true;
            StartCoroutine(SpawnEnemyRoutine(3));
        }
    }

}

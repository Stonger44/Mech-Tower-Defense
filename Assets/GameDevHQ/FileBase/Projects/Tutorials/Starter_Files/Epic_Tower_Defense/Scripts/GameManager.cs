using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private int _initialHealth;
    [SerializeField] private int _health;
    public int totalWarFund;

    private int _initialWave = 1;
    public int wave { get; private set; }
    

    [SerializeField] private int _initialWaveEnemyCount;
    public int waveTotalEnemyCount { get; private set; }
    [SerializeField] private int _currentWaveEnemyCount;

    public bool waveRunning { get; private set; }
    public bool waveSuccess { get; private set; }

    public delegate void StartingWave();
    public static event StartingWave onStartWave;

    public override void Init()
    {
        wave = _initialWave;

        ResetPlayerHealthAndWaveEnemyCount();
    }

    private void OnEnable()
    {
        //Subscribe to events
        EndPoint.onEndPointReached += TakeDamage;
        Enemy.onDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        EndPoint.onEndPointReached -= TakeDamage;
        Enemy.onDeath -= OnEnemyDeath;
    }

    private void Start()
    {
        Debug.Log("Press [Space] to start Wave " + wave + ".");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !waveRunning)
        {
            StartWave();
        }
    }

    public void BroadcastStartWave()
    {
        onStartWave?.Invoke();
    }

    private void StartWave()
    {
        ResetPlayerHealthAndWaveEnemyCount();

        waveRunning = true;
        waveSuccess = false;

        BroadcastStartWave();
        Debug.Log("Wave " + wave + " started.");
    }

    private void WaveComplete()
    {
        waveRunning = false;
        waveSuccess = true;
        Debug.Log("Wave " + wave + " complete!");
        
        wave++;
    }

    private void TakeDamage()
    {
        _health--;

        if (_health <= 0)
        {
            _health = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        waveRunning = false;
        waveSuccess = false;
        Debug.Log("Wave " + wave + " failed.");
    }

    private void OnEnemyDeath(int warFund)
    {
        totalWarFund += warFund;

        _currentWaveEnemyCount--;

        if (_currentWaveEnemyCount <= 0)
        {
            WaveComplete();
        }

    }

    private void ResetPlayerHealthAndWaveEnemyCount()
    {
        _health = _initialHealth;
        waveTotalEnemyCount = _initialWaveEnemyCount * wave;
        _currentWaveEnemyCount = waveTotalEnemyCount;
    }
}

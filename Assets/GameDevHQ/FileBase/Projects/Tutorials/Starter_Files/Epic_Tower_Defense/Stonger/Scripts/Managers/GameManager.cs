using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private int _initialWaveEnemyCount;
    [SerializeField] private int _initialHealth;
    [SerializeField] private int _health;
    public int totalWarFund;

    private int _initialWave = 1;
    [SerializeField] private int _wave; //This is only here so I can see it in the inspector
    public int wave { get; private set; }
    

    
    [SerializeField] private int _currentWaveTotalEnemyCount;
    public int currentWaveTotalEnemyCount { get; private set; }
    [SerializeField] private int _currentWaveCurrentEnemyCount;

    public bool waveRunning { get; private set; }
    public bool waveSuccess { get; private set; }

    public static event Action onStartWave;


    public override void Init()
    {
        wave = _initialWave;
        _wave = wave;
        ResetPlayerHealthAndWaveEnemyCount();
    }

    private void OnEnable()
    {
        //Subscribe to events
        EndPoint.onEndPointReached += TakeDamage;
        Enemy.onDeath += OnEnemyExplosion;
        Enemy.onResetComplete += OnEnemyResetComplete;
        TowerLocation.onPurchaseTower += SpendWarFund;
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        EndPoint.onEndPointReached -= TakeDamage;
        Enemy.onDeath -= OnEnemyExplosion;
        Enemy.onResetComplete -= OnEnemyResetComplete;
        TowerLocation.onPurchaseTower -= SpendWarFund;
    }

    private void Start()
    {
        Debug.Log("Press [Space] to start Wave " + wave + ".");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !waveRunning)
            StartWave();
    }

    private void StartWave()
    {
        ResetPlayerHealthAndWaveEnemyCount();

        waveRunning = true;
        waveSuccess = false;

        _wave = wave;

        onStartWave?.Invoke();
        Debug.Log("Wave " + wave + " started.");
    }

    private void WaveComplete()
    {
        waveRunning = false;
        waveSuccess = true;
        wave++;

        Debug.Log("Wave " + (wave - 1) + " complete! Press [Space] to start Wave " + wave + ".");
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

    private void OnEnemyExplosion(GameObject enemy)
    {
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript == null)
            Debug.Log("enemyScript is NULL.");

        totalWarFund += enemyScript.warFund;

        _currentWaveCurrentEnemyCount--;
    }

    private void OnEnemyResetComplete(GameObject enemy)
    {
        if (Enemy.enemyCount <= 0)
            WaveComplete();
    }

    private void ResetPlayerHealthAndWaveEnemyCount()
    {
        _health = _initialHealth;
        currentWaveTotalEnemyCount = _initialWaveEnemyCount * wave;
        _currentWaveTotalEnemyCount = currentWaveTotalEnemyCount;
        _currentWaveCurrentEnemyCount = currentWaveTotalEnemyCount;
    }

    private void SpendWarFund(int amount)
    {
        totalWarFund -= amount;
    }

    public int GetCurrentWaveCurrentEnemyCount() => _currentWaveCurrentEnemyCount;
}

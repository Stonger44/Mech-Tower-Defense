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
    [SerializeField] private GameObject _healthBarGameObject;
    private float _healthPercent;

    [SerializeField] private int _totalWarFunds;
    public int TotalWarFunds { get; private set; } //This is only here so I can see it in the inspector

    private int _initialWave = 1;
    [SerializeField] private int _wave; //This is only here so I can see it in the inspector
    public int wave { get; private set; }
    
    [SerializeField] private int _currentWaveTotalEnemyCount;
    public int currentWaveTotalEnemyCount { get; private set; }
    [SerializeField] private int _currentWaveCurrentEnemyCount;

    public bool waveRunning { get; private set; }
    public bool waveSuccess { get; private set; }

    public static event Action onStartWave;
    public static event Action<GameObject> onGainedWarFundsFromEnemyDeath;
    public static event Action<GameObject, float> onHealthUpdate;

    public override void Init()
    {
        wave = _initialWave;
        _wave = wave;
        ResetWaveEnemyCount();
    }

    private void OnEnable()
    {
        //Subscribe to events
        EndPoint.onEndPointReached += TakeDamage;
        Enemy.onDeath += OnEnemyExplosion;
        Enemy.onResetComplete += OnEnemyResetComplete;
        TowerLocation.onPurchaseTower += SpendWarFunds;
        TowerLocation.onDismantledCurrentTower += CollectDismantledTowerWarFunds;
        TowerLocation.onPurchaseTowerUpgrade += SpendWarFunds;
    }

    private void OnDisable()
    {
        //Unsubscribe from events
        EndPoint.onEndPointReached -= TakeDamage;
        Enemy.onDeath -= OnEnemyExplosion;
        Enemy.onResetComplete -= OnEnemyResetComplete;
        TowerLocation.onPurchaseTower -= SpendWarFunds;
        TowerLocation.onDismantledCurrentTower -= CollectDismantledTowerWarFunds;
        TowerLocation.onPurchaseTowerUpgrade -= SpendWarFunds;
    }

    private void Start()
    {
        _health = _initialHealth;
        _healthPercent = (float)_health / (float)_initialHealth;
        onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);
        TotalWarFunds = _totalWarFunds;
        UI_Manager.Instance.UpdateWarFundsText(TotalWarFunds);

        Debug.Log("Press [Space] to start Wave " + wave + ".");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !waveRunning)
            StartWave();
    }

    private void StartWave()
    {
        ResetWaveEnemyCount();

        if (_health <= 0)
            _health = _initialHealth;
        _healthPercent = (float)_health / (float)_initialHealth;
        onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);

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
            _healthPercent = (float)_health / (float)_initialHealth;
            onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);
            GameOver();
            return;
        }
        _healthPercent = (float)_health / (float)_initialHealth;
        onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);
    }

    private void GameOver()
    {
        waveRunning = false;
        waveSuccess = false;
        Debug.Log("Wave " + wave + " failed. Press [Space] to try again.");
    }

    private void OnEnemyExplosion(GameObject enemy)
    {
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript == null)
            Debug.Log("enemyScript is NULL.");

        TotalWarFunds += enemyScript.warFunds;
        _totalWarFunds = TotalWarFunds;
        UI_Manager.Instance.UpdateWarFundsText(TotalWarFunds);

        if (TowerManager.Instance.IsViewingTower == true)
            onGainedWarFundsFromEnemyDeath?.Invoke(TowerManager.Instance.CurrentlyViewedTower);

        _currentWaveCurrentEnemyCount--;
    }

    private void OnEnemyResetComplete(GameObject enemy)
    {
        if (Enemy.enemyCount <= 0)
            WaveComplete();
    }

    private void ResetWaveEnemyCount()
    {
        currentWaveTotalEnemyCount = _initialWaveEnemyCount * wave;
        _currentWaveTotalEnemyCount = currentWaveTotalEnemyCount;
        _currentWaveCurrentEnemyCount = currentWaveTotalEnemyCount;
    }

    private void SpendWarFunds(int warFundsSpent)
    {
        TotalWarFunds -= warFundsSpent;
        _totalWarFunds = TotalWarFunds;
        UI_Manager.Instance.UpdateWarFundsText(TotalWarFunds);
    }

    private void CollectDismantledTowerWarFunds(int warFundsAcquired)
    {
        TotalWarFunds += warFundsAcquired;
        _totalWarFunds = TotalWarFunds;
        UI_Manager.Instance.UpdateWarFundsText(TotalWarFunds);
    }

    public int GetCurrentWaveCurrentEnemyCount() => _currentWaveCurrentEnemyCount;
}

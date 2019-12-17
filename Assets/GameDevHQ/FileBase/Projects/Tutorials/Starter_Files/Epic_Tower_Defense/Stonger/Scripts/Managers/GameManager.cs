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

    public float HealthCautionThreshold { get; private set; }
    public float HealthWarningThreshold { get; private set; }
    [SerializeField] private float _healthCautionThreshold;
    [SerializeField] private float _healthWarningThreshold;
    private float _healthPercent;

    public int TotalWarFunds { get; private set; } //This is only here so I can see it in the inspector
    [SerializeField] private int _totalWarFunds;

    public int Wave { get; private set; }
    [SerializeField] private int _wave; //This is only here so I can see it in the inspector
    [SerializeField] private int _initialWave = 1;
    [SerializeField] private int _finalWave;

    public int CurrentWaveTotalEnemyCount { get; private set; }
    [SerializeField] private int _currentWaveTotalEnemyCount;
    [SerializeField] private int _currentWaveCurrentEnemyCount;

    public bool WaveRunning { get; private set; }
    public bool WaveSuccess { get; private set; }

    public static event Action onStartWave;
    public static event Action<GameObject> onGainedWarFundsFromEnemyDeath;
    public static event Action<GameObject, float> onHealthUpdate;
    public static event Action<int, int> onHealthUpdateUI;
    public static event Action<int, int> onWaveUpdate;
    public static event Action<int, int> onEnemyCountUpdate;

    public override void Init()
    {
        HealthCautionThreshold = _healthCautionThreshold;
        HealthWarningThreshold = _healthWarningThreshold;

        Wave = _initialWave;
        _wave = Wave;
        onWaveUpdate?.Invoke(_wave, _finalWave);
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
        onHealthUpdateUI?.Invoke(_health, _initialHealth);
        TotalWarFunds = _totalWarFunds;
        UI_Manager.Instance.UpdateWarFundsText(TotalWarFunds);

        Debug.Log("Press [Space] to start Wave " + Wave + ".");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !WaveRunning)
            StartWave();
    }

    private void StartWave()
    {
        ResetWaveEnemyCount();

        if (_health <= 0)
            _health = _initialHealth;
        _healthPercent = (float)_health / (float)_initialHealth;
        onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);
        onHealthUpdateUI?.Invoke(_health, _initialHealth);

        WaveRunning = true;
        WaveSuccess = false;

        _wave = Wave;
        onWaveUpdate?.Invoke(_wave, _finalWave);
        onStartWave?.Invoke();
        Debug.Log("Wave " + Wave + " started.");
    }

    private void WaveComplete()
    {
        WaveRunning = false;
        WaveSuccess = true;
        Wave++;

        Debug.Log("Wave " + (Wave - 1) + " complete! Press [Space] to start Wave " + Wave + ".");
    }

    private void TakeDamage()
    {
        _health--;

        if (_health <= 0)
        {
            _health = 0;
            _healthPercent = (float)_health / (float)_initialHealth;
            onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);
            onHealthUpdateUI?.Invoke(_health, _initialHealth);
            GameOver();
            return;
        }
        _healthPercent = (float)_health / (float)_initialHealth;
        onHealthUpdate?.Invoke(_healthBarGameObject, _healthPercent);
        onHealthUpdateUI?.Invoke(_health, _initialHealth);
    }

    private void GameOver()
    {
        WaveRunning = false;
        WaveSuccess = false;
        Debug.Log("Wave " + Wave + " failed. Press [Space] to try again.");
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
        onEnemyCountUpdate.Invoke(_currentWaveCurrentEnemyCount, _currentWaveTotalEnemyCount);
    }

    private void OnEnemyResetComplete(GameObject enemy)
    {
        if (Enemy.enemyCount <= 0)
            WaveComplete();
    }

    private void ResetWaveEnemyCount()
    {
        CurrentWaveTotalEnemyCount = _initialWaveEnemyCount * Wave;
        _currentWaveTotalEnemyCount = CurrentWaveTotalEnemyCount;
        _currentWaveCurrentEnemyCount = CurrentWaveTotalEnemyCount;
        onEnemyCountUpdate.Invoke(_currentWaveCurrentEnemyCount, _currentWaveTotalEnemyCount);
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

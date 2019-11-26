using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoSingleton<PoolManager>
{
    private int _randomIndex;

    [SerializeField] private List<GameObject> _enemyPrefabs;
    public List<GameObject> enemyPool;
    
    public GameObject enemyContainer;

    private Enemy _currentEnemy;

    private void OnEnable()
    {
        GameManager.onStartWave += SetEnemiesInPoolToStandby;
    }

    private void OnDisable()
    {
        GameManager.onStartWave -= SetEnemiesInPoolToStandby;
    }

    private void Start()
    {
        GenerateEnemies(GameManager.Instance.currentWaveTotalEnemyCount);
    }

    private void GenerateEnemies(int numberOfEnemiesToGenerate)
    {
        for (int i = 0; i < numberOfEnemiesToGenerate; i++)
        {
            //I only want to spawn the bigger mechs 25% of the time
            _randomIndex = (Random.Range(0f, 1f) <= 0.75f) ? 0 : 1;

            GameObject enemy = Instantiate(_enemyPrefabs[_randomIndex]);

            //Put enemies in EnemyContainer to keep the heirarchy clean
            enemy.transform.parent = enemyContainer.transform;

            enemyPool.Add(enemy);
        }
    }

    public GameObject RequestEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            _currentEnemy = enemy.GetComponent<Enemy>();

            if (_currentEnemy != null && _currentEnemy.IsOnStandby())
            {
                _currentEnemy.SetToAttack();
                return enemy;
            }
        }

        ////If we get here, ther are no enemies available.
        //return null;

        //If maximum enemies for the wave has been reached, do not generate more enemies
        if (enemyPool.Count >= GameManager.Instance.currentWaveTotalEnemyCount)
            return null;

        GenerateEnemies(1);
        return RequestEnemy();
    }

    private void SetEnemiesInPoolToStandby()
    {
        //Set current Enemies in Pool to Standby
        foreach (var enemy in enemyPool)
        {
            //Reset Switch:
            //Enemy.OnEnable() sets the enemy to Standby, so first disable the enemy, so that it can be enabled.
            enemy.SetActive(false);
            enemy.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private int _randomIndex;

    [SerializeField] private List<GameObject> _enemyPrefabs;
    public List<GameObject> enemyPool;
    
    public GameObject enemyContainer;

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
        GenerateEnemies(GameManager.Instance.waveTotalEnemyCount);
    }

    private void GenerateEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
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
            var currentEnemy = enemy.GetComponent<Enemy>();

            if (currentEnemy != null && currentEnemy.IsOnStandby())
            {
                currentEnemy.SetToAttack();
                return enemy;
            }
        }

        //If maximum enemies for the wave has been reached, do not generate more enemies
        if (enemyPool.Count >= GameManager.Instance.waveTotalEnemyCount)
            return null;

        GenerateEnemies(1);
        return RequestEnemy();
    }

    private void SetEnemiesInPoolToStandby()
    {
        foreach (var enemy in enemyPool)
        {
            //Setting OnEnable sets the enemy to Standby,
            //so first disable, so that you can enable.
            enemy.SetActive(false);
            enemy.SetActive(true);
        }
    }
}

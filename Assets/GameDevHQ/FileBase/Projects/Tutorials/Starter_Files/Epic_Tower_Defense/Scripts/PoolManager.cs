using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private int _enemyCount;

    private int _randomIndex;

    [SerializeField]
    private List<GameObject> _enemyPrefabs;
    [SerializeField]
    private List<GameObject> _enemyPool;
    [SerializeField]
    private GameObject _enemyContainer;

    public override void Init()
    {

    }

    private void Start()
    {
        //I put this in Start() instead of Init() because the SpawnManager would be null.
        _enemyCount = SpawnManager.Instance.enemyCount;

        GenerateEnemies(_enemyCount);
    }

    private void Update()
    {

    }

    private void GenerateEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            //I only want to spawn the bigger mechs 25% of the time
            _randomIndex = Random.Range(0f, 1f) <= 0.75f ? 0 : 1;

            GameObject enemy = Instantiate(_enemyPrefabs[_randomIndex]);

            //Put enemies in EnemyContainer to keep the heirarchy clean
            enemy.transform.parent = _enemyContainer.transform;
            enemy.SetActive(false);

            _enemyPool.Add(enemy);
        }
    }

    //DANGER - Recursive Function
    public GameObject RequestEnemy()
    {
        foreach (var enemy in _enemyPool)
        {
            if (enemy.activeInHierarchy == false)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        GenerateEnemies(1);

        return RequestEnemy();
    }
}

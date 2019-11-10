using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private int _randomIndex;

    [SerializeField] private List<GameObject> _enemyPrefabs;
    public List<GameObject> enemyPool;
    
    public GameObject enemyContainer;

    public override void Init()
    {

    }

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

    // Update is called once per frame SO TRY TO ONLY USE IT FOR PLAYER INPUT
    private void Update()
    {
        
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

            if (currentEnemy != null && currentEnemy.IsOnStandBy())
            {
                currentEnemy.SetToAttack();
                return enemy;
            }
        }

        //Fix maximum enemies for the wave has been reached, do not generate more enemies
        if (enemyPool.Count >= GameManager.Instance.waveTotalEnemyCount)
        {
            return null;
        }

        GenerateEnemies(1);
        return RequestEnemy();
    }

    private void SetEnemiesInPoolToStandby()
    {
        foreach (var enemy in enemyPool)
        {
            if (enemy.activeSelf == false)
            {
                enemy.SetActive(true);
            }
        }
    }
}

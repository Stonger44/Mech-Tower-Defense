using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoSingleton<PoolManager>
{
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

    /*----------Enemy Pool----------*/
    #region Enemy Pool

    [SerializeField] private List<GameObject> _enemyPrefabs;
    [SerializeField] private List<GameObject> enemyPool;
    [SerializeField] private GameObject enemyContainer;
    private int _randomIndex;
    private enum EnemyType { Mech1, Mech2 }
    private Enemy _currentEnemy;


    private void GenerateEnemies(int numberOfEnemiesToGenerate)
    {
        for (int i = 0; i < numberOfEnemiesToGenerate; i++)
        {
            //I only want to spawn the bigger mechs 25% of the time
            _randomIndex = (Random.Range(0f, 1f) <= 0.75f) ? (int)EnemyType.Mech1 : (int)EnemyType.Mech2;

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

    #endregion
    /*----------Enemy Pool----------*/

    /*----------Explosion Pool----------*/
    #region Explosion Pool

    [SerializeField] private List<GameObject> _explosionPrefabs;
    [SerializeField] private List<GameObject> explosionPool;
    [SerializeField] private GameObject explosionContainer;
    private int _explosionIndex;


    private void GenerateExplosion(GameObject explodingEnemy)
    {
        _explosionIndex = (explodingEnemy.tag == "Mech1") ? (int)EnemyType.Mech1 : (int)EnemyType.Mech2;

        GameObject explosion = Instantiate(_explosionPrefabs[_explosionIndex]);
        explosion.SetActive(false);
        explosion.transform.parent = explosionContainer.transform;

        explosionPool.Add(explosion);
    }

    public GameObject RequestExplosion(GameObject explodingEnemy)
    {
        foreach (var explosion in explosionPool)
        {
            if (explosion.activeSelf == false && explosion.name.Contains(explodingEnemy.tag))
            {
                return explosion;
            }
        }

        GenerateExplosion(explodingEnemy);
        return RequestExplosion(explodingEnemy);
    }

    public void ResetExplosion(GameObject explosion)
    {
        //Turn off explosion
        explosion.SetActive(false);
        //Set position to explosion container
        explosion.transform.position = explosionContainer.transform.position;
    }

    #endregion
    /*----------Explosion Pool----------*/

}

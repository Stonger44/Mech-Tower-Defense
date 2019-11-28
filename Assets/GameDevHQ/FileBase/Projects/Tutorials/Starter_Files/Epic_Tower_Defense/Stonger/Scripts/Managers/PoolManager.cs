using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameDevHQ.FileBase.Missle_Launcher.Missle;

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

        //Generate one explosion prefeb for each unique enemy to start
        foreach (var enemy in _enemyPrefabs)
            GenerateExplosion(enemy);

        GenerateMissiles(6);
    }

    //Currently not used
    public void ResetPoolObject(GameObject gameObject)
    {
        gameObject.SetActive(false);

        switch (gameObject.name)
        {
            case "Explosion_Mech1":
            case "Explosion_Mech2":
                gameObject.transform.position = _explosionContainer.transform.position;
                break;
            case "Missile":
                gameObject.transform.position = _missileContainer.transform.position;
                break;
            default:
                break;
        }


    }

    /*----------Enemy Pool----------*/
    #region Enemy Pool

    [SerializeField] private List<GameObject> _enemyPrefabs;
    [SerializeField] private List<GameObject> _enemyPool;
    [SerializeField] private GameObject _enemyContainer;
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
            enemy.transform.parent = _enemyContainer.transform;

            _enemyPool.Add(enemy);
        }
    }

    public GameObject RequestEnemy()
    {
        foreach (var enemy in _enemyPool)
        {
            _currentEnemy = enemy.GetComponent<Enemy>();

            if (_currentEnemy != null && _currentEnemy.IsOnStandby())
            {
                _currentEnemy.SetToAttack();
                return enemy;
            }
        }

        //If maximum enemies for the wave has been reached, do not generate more enemies
        if (_enemyPool.Count >= GameManager.Instance.currentWaveTotalEnemyCount)
            return null;

        GenerateEnemies(1);
        return RequestEnemy();
    }

    private void SetEnemiesInPoolToStandby()
    {
        //Set current Enemies in Pool to Standby
        foreach (var enemy in _enemyPool)
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
    [SerializeField] private List<GameObject> _explosionPool;
    [SerializeField] private GameObject _explosionContainer;
    private int _explosionIndex;


    private void GenerateExplosion(GameObject explodingEnemy)
    {
        _explosionIndex = (explodingEnemy.tag == "Mech1") ? (int)EnemyType.Mech1 : (int)EnemyType.Mech2;

        GameObject explosion = Instantiate(_explosionPrefabs[_explosionIndex]);
        explosion.SetActive(false);
        explosion.transform.parent = _explosionContainer.transform;

        _explosionPool.Add(explosion);
    }

    public GameObject RequestExplosion(GameObject explodingEnemy)
    {
        foreach (var explosion in _explosionPool)
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
        explosion.SetActive(false);
        explosion.transform.position = _explosionContainer.transform.position;
    }

    #endregion
    /*----------Explosion Pool----------*/

    /*----------Missile Pool----------*/
    #region Missile Pool

    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private List<GameObject> _missilePool;
    [SerializeField] private GameObject _missileContainer;
    private Missle _currentMissile;

    private void GenerateMissiles(int numberOfMissilesToGenerate)
    {
        for (int i = 0; i < numberOfMissilesToGenerate; i++)
        {
            GameObject missile = Instantiate(_missilePrefab);
            missile.SetActive(false);
            missile.transform.parent = _missileContainer.transform;
            missile.transform.position = _missileContainer.transform.position;

            _missilePool.Add(missile);
        }
    }

    public GameObject RequestMissile()
    {
        foreach (var missile in _missilePool)
        {
            //_currentMissile = missile.GetComponent<Missle>();

            if (missile.activeSelf == false) // && _currentMissile != null && _currentMissile.GetIsMissileLaunched() == false)
            {
                return missile;
            }
        }

        GenerateMissiles(1);
        return RequestMissile();
    }

    public void ResetMissile(GameObject missile)
    {
        missile.SetActive(false);
        missile.transform.rotation = _missilePrefab.transform.rotation;
        missile.transform.parent = _missileContainer.transform;
        missile.transform.position = _missileContainer.transform.position;
    }

    #endregion
    /*----------Missile Pool----------*/

}

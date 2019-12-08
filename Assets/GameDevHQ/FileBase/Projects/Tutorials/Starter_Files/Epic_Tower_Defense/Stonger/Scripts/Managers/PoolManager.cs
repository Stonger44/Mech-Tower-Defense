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

        foreach (var explosion in _enemyPrefabs)
            GenerateExplosion(explosion);
        GenerateExplosion(_missilePrefab);

        GenerateMissiles(6);

        GenerateAllTowers(_numberOfEachTowerToGenerate);
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
                gameObject.transform.position = missileContainer.transform.position;
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
    private GameObject _generatedEnemy;
    private Enemy _currentEnemy;


    private void GenerateEnemies(int numberOfEnemiesToGenerate)
    {
        for (int i = 0; i < numberOfEnemiesToGenerate; i++)
        {
            //I only want to spawn the bigger mechs 25% of the time
            _randomIndex = (Random.Range(0f, 1f) <= 0.75f) ? (int)EnemyType.Mech1 : (int)EnemyType.Mech2;

            _generatedEnemy = Instantiate(_enemyPrefabs[_randomIndex]);

            //Put enemies in EnemyContainer to keep the heirarchy clean
            _generatedEnemy.transform.parent = _enemyContainer.transform;

            _enemyPool.Add(_generatedEnemy);
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
    private enum ExplosionType { Mech1, Mech2, Missile };


    private void GenerateExplosion(GameObject explodingObject)
    {
        switch (explodingObject.tag)
        {
            case "Mech1":
                _explosionIndex = (int)ExplosionType.Mech1;
                break;
            case "Mech2":
                _explosionIndex = (int)ExplosionType.Mech2;
                break;
            case "Missile":
                _explosionIndex = (int)ExplosionType.Missile;
                break;
            default:
                Debug.LogError("Type of explosion not found.");
                break;
        }

        GameObject explosion = Instantiate(_explosionPrefabs[_explosionIndex]);
        explosion.SetActive(false);
        explosion.transform.parent = _explosionContainer.transform;

        _explosionPool.Add(explosion);
    }

    public GameObject RequestExplosion(GameObject explodingObject)
    {
        foreach (var explosion in _explosionPool)
        {
            if (explosion.activeSelf == false && explosion.name.Contains(explodingObject.tag))
            {
                return explosion;
            }
        }

        GenerateExplosion(explodingObject);
        return RequestExplosion(explodingObject);
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
    public GameObject missileContainer;
    private Missile _currentMissile;

    private void GenerateMissiles(int numberOfMissilesToGenerate)
    {
        for (int i = 0; i < numberOfMissilesToGenerate; i++)
        {
            GameObject missile = Instantiate(_missilePrefab);
            missile.SetActive(false);
            missile.transform.parent = missileContainer.transform;
            missile.transform.position = missileContainer.transform.position;

            _missilePool.Add(missile);
        }
    }

    public GameObject RequestMissile()
    {
        foreach (var missile in _missilePool)
        {
            if (missile.activeSelf == false)
            {
                missile.SetActive(true);
                return missile;
            }
        }

        GenerateMissiles(1);
        return RequestMissile();
    }

    public void ResetMissile(GameObject missile)
    {
        missile.transform.rotation = _missilePrefab.transform.rotation;
        missile.transform.parent = missileContainer.transform;
        missile.transform.position = missileContainer.transform.position;
    }

    #endregion
    /*----------Missile Pool----------*/

    /*----------Tower Pool----------*/
    #region Tower Pool

    [SerializeField] private List<GameObject> _towerPrefabs;
    [SerializeField] private List<GameObject> _towerPool;
    [SerializeField] private GameObject _towerContainer;
    [SerializeField] private int _numberOfEachTowerToGenerate;
    private GameObject _currentTower;

    private void GenerateAllTowers(int numberOfEachTowerToGenerate)
    {
        foreach (var tower in _towerPrefabs)
        {
            for (int i = 0; i < numberOfEachTowerToGenerate; i++)
            {
                _currentTower = Instantiate(tower, _towerContainer.transform.position, Quaternion.Euler(0, 90, 0));
                _currentTower.transform.parent = _towerContainer.transform;
                _currentTower.SetActive(false);
                _towerPool.Add(_currentTower);
            }
        }
    }

    public GameObject RequestTower(GameObject requestedTower)
    {
        foreach (var tower in _towerPool)
        {
            if (tower.activeSelf == false && tower.tag == requestedTower.tag)
            {
                tower.SetActive(true);
                return tower;
            }
        }

        //No Towers available?!
        Debug.LogError("No towers of requested tower type available!");
        return null;
    } 

    #endregion
    /*----------Tower Pool----------*/
}

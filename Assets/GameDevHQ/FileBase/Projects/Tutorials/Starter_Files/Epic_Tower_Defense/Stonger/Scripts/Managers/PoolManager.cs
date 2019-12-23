using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoSingleton<PoolManager>
{
    private void OnEnable()
    {
        GameManager.onStartWave += SetEnemiesInPoolToStandby;
        UI_Manager.onResetEnemiesFotNextWave += SetEnemiesInPoolToStandby;
    }

    private void OnDisable()
    {
        GameManager.onStartWave -= SetEnemiesInPoolToStandby;
        UI_Manager.onResetEnemiesFotNextWave -= SetEnemiesInPoolToStandby;
    }

    private void Start()
    {
        GenerateEnemies(GameManager.Instance.CurrentWaveTotalEnemyCount);

        foreach (var explosion in _enemyPrefabs)
            GenerateExplosion(explosion);
        for (int i = 0; i < 6; i++)
            GenerateExplosion(_missilePrefab);
        GenerateExplosion(_towerPrefabs[0]); //Any tower will do, the explosion is the same
        GenerateMissiles(6);

        GenerateAllTowers(_numberOfEachTowerToGenerate);
    }

    /*----------Enemy Pool----------*/
    #region Enemy Pool

    [SerializeField] private List<GameObject> _enemyPrefabs;
    [SerializeField] private List<GameObject> _enemyPool;
    [SerializeField] private List<GameObject> _randomizedEnemyPool;
    [SerializeField] private GameObject _enemyContainer;
    private int _randomIndex;
    private int _randomEnemyPoolIndex;
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
        if (GameManager.Instance.WaveRunning == false)
            return null;

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
        if (_enemyPool.Count >= GameManager.Instance.CurrentWaveTotalEnemyCount)
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

        ////Shuffle EnemyPool
        //_randomizedEnemyPool.Clear();
        
        //for (int i = 0; i < _enemyPool.Count; i++)
        //{
        //    _randomEnemyPoolIndex = Random.Range(0, _enemyPool.Count);

        //    _randomizedEnemyPool.Add(_enemyPool[_randomEnemyPoolIndex]);
        //    _enemyPool.Remove(_enemyPool[_randomEnemyPoolIndex]);
        //}

        //_enemyPool = _randomizedEnemyPool;
    }

    #endregion
    /*----------Enemy Pool----------*/

    /*----------Explosion Pool----------*/
    #region Explosion Pool

    [SerializeField] private List<GameObject> _explosionPrefabs;
    [SerializeField] private List<GameObject> _explosionPool;
    [SerializeField] private GameObject _explosionContainer;
    private int _explosionIndex;
    private enum ExplosionType { Mech1, Mech2, Missile, Tower };


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
                if (explodingObject.tag.Contains("Tower"))
                    _explosionIndex = (int)ExplosionType.Tower;
                else
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
            if (explosion.activeSelf == false)
            {
                if ((explosion.name.Contains("Tower") && explodingObject.tag.Contains("Tower")) || explosion.name.Contains(explodingObject.tag))
                {
                    return explosion;
                }
            }
        }

        GenerateExplosion(explodingObject);
        return RequestExplosion(explodingObject);
    }

    public void ResetExplosionPosition(GameObject explosion)
    {
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

    public void ResetMissileTransform(GameObject missile)
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
                return tower;
            }
        }

        //No Towers available?!
        Debug.LogError("No towers of requested tower type available!");
        return null;
    }

    public void ResetTowerPosition(GameObject towerToReset)
    {
        towerToReset.transform.position = _towerContainer.transform.position;
        towerToReset.SetActive(false);
    }

    #endregion
    /*----------Tower Pool----------*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public GameObject spawnPoint;
    public GameObject endPoint;

    [SerializeField]
    private List<GameObject> _enemyPrefabs;

    private GameObject _enemyContainer;

    public override void Init()
    {
        _enemyContainer = GameObject.Find("EnemyContainer");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, _enemyPrefabs.Count);

        GameObject enemy = Instantiate(_enemyPrefabs[randomIndex], spawnPoint.transform.position, Quaternion.Euler(0, -90, 0));

        enemy.transform.parent = _enemyContainer.transform;
    }
}

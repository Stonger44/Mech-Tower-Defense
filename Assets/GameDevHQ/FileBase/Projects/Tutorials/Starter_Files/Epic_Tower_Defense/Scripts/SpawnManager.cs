using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField]
    private Vector3 _spawnPoint = new Vector3(1.5f, 1.0f, 0.16f);
    [SerializeField]
    private List<GameObject> _enemyPrefabs;

    private GameObject _enemyContainer;

    private void Start()
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

        GameObject enemy = Instantiate(_enemyPrefabs[randomIndex], _spawnPoint, Quaternion.Euler(0, -90, 0));

        enemy.transform.parent = _enemyContainer.transform;
    }
}

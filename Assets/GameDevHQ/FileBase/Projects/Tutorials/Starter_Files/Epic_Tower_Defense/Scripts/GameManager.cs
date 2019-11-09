using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private int health = 3;
    public int totalWarFund = 0;

    public int wave = 1;

    public bool waveRunning { get; private set; }

    public override void Init()
    {
        
    }

    private void OnEnable()
    {
        EndPoint.onEndPointReached += TakeDamage;
    }

    private void OnDisable()
    {
        EndPoint.onEndPointReached -= TakeDamage;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !waveRunning)
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        Debug.Log("Wave " + wave + " started.");
        StartCoroutine(SpawnManager.Instance.SpawnEnemyRoutine(3));

        waveRunning = true;
    }

    private void WaveComplete()
    {
        waveRunning = false;
    }

    private void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            health = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        waveRunning = false;
        Debug.Log("Game Over");
    }

}

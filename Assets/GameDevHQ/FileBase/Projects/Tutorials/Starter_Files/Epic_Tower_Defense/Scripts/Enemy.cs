using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public static int enemyCount;

    private Vector3 _spawnPoint;
    private Vector3 _endPoint;
    private Vector3 _standbyPoint;

    private NavMeshAgent _navMeshAgent;

    [SerializeField] private int _initialHealth;
    [SerializeField] private int _health;
    [SerializeField] private int _warFund;
    [SerializeField] private bool _onStandby = false;

    public static event Action<int> onDeath;

    private void Awake()
    {
        //I put this here because I Enable and Disable the enemy objects;
        //This way GetComponent<> is only called once.
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        enemyCount++;

        _spawnPoint = SpawnManager.Instance.spawnPoint.transform.position;
        _endPoint = SpawnManager.Instance.endPoint.transform.position;
        _standbyPoint = SpawnManager.Instance.standbyPoint.transform.position;

        _health = _initialHealth;

        SetToStandby();
    }

    private void OnDisable()
    {
        enemyCount--;
    }

    // Update is called once per frame SO TRY TO ONLY USE IT FOR PLAYER INPUT
    void Update()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        _health -= damageAmount;

        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
    }

    public void Die()
    {
        this.gameObject.SetActive(false);

        //Broadcast enemy death
        onDeath?.Invoke(_warFund);
    }

    public void SetToAttack()
    {
        _onStandby = false;
        _navMeshAgent.enabled = true;
        _navMeshAgent.Warp(_spawnPoint);
        _navMeshAgent.SetDestination(_endPoint);
    }

    public void SetToStandby()
    {
        _navMeshAgent.enabled = false;
        this.transform.position = _standbyPoint;

        _onStandby = true;
    }

    public bool IsOnStandby() => _onStandby;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "EndPoint")
        {
            SetToStandby();
        }
    }
}

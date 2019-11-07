using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Vector3 _spawnPoint;
    private Vector3 _endPoint;

    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private int _health = 0;
    [SerializeField]
    private int _warFund = 0;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        _spawnPoint = SpawnManager.Instance.spawnPoint.transform.position;
        _endPoint = SpawnManager.Instance.endPoint.transform.position;

        _navMeshAgent.SetDestination(_endPoint);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x <= _endPoint.x)
        {
            _navMeshAgent.Warp(_spawnPoint);
            _navMeshAgent.SetDestination(_endPoint);
        }
    }
}

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

    }

    private void OnEnable()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        _spawnPoint = SpawnManager.Instance.spawnPoint.transform.position;
        _endPoint = SpawnManager.Instance.endPoint.transform.position;

        //When SetActive, "spawn" at (warp to) the spawn point and start moving toward the endpoint
        _navMeshAgent.Warp(_spawnPoint);
        _navMeshAgent.SetDestination(_endPoint);
    }

    // Update is called once per frame
    void Update()
    {
        //When an enemy reaches the endpoint, deactivate it (recycle it to the Enemy pool)
        if (this.transform.position.x <= _endPoint.x)
        {
            this.gameObject.SetActive(false);
        }
    }
}

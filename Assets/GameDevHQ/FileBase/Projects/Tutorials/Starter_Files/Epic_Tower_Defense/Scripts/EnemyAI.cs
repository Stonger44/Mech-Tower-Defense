using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private GameObject _endPoint = null;

    private NavMeshAgent _navMeshAgent = null;

    [SerializeField]
    private int _health = 0;
    [SerializeField]
    private int _warFund = 0;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        _navMeshAgent.SetDestination(_endPoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

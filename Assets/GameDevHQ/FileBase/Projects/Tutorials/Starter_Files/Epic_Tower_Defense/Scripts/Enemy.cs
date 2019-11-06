using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Vector3 _endPoint = new Vector3(-48.5f, 0.6f, 0.16f);

    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private int _health = 0;
    [SerializeField]
    private int _warFund = 0;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        _navMeshAgent.SetDestination(_endPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

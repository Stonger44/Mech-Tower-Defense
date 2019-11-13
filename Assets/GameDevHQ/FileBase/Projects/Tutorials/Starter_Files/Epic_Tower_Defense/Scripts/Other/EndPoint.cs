using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public static event Action onEndPointReached;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame SO TRY TO ONLY USE IT FOR PLAYER INPUT
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Mech"))
        {
            onEndPointReached?.Invoke();
        }
    }
}

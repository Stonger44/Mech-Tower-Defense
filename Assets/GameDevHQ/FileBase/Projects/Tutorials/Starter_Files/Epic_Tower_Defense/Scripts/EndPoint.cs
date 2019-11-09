using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
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
            var enemyScript = other.GetComponent<Enemy>();

            //When an enemy reaches the endpoint, decrement player health, move and set the enemy to standby
            enemyScript.SetToStandBy();
        }
    }
}

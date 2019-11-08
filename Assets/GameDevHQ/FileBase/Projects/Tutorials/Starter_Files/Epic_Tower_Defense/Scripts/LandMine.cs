using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerStay(Collider otherObject)
    {
        if (otherObject.tag == "Enemy")
        {
            Debug.Log("Enemy Alert");
            var enemy = otherObject.GetComponent<Enemy>();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("LandMineFired");
                enemy.Die();
            } 
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    private float _nextFire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame SO TRY TO ONLY USE IT FOR PLAYER INPUT
    void Update()
    {
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Contains("Mech"))
        {
            Debug.Log("Enemy Alert");
            var enemy = other.GetComponent<Enemy>();

            if (Input.GetKeyDown(KeyCode.Space) && (Time.time > _nextFire))
            {
                Debug.Log("LandMineFired");
                enemy.TakeDamage(10);
                _nextFire = Time.time + 1.0f;
            } 
        }
        
    }
}

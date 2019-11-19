using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private GameObject _horizontalAimPivot;
    [SerializeField] private GameObject _verticalAimPivot;

    [SerializeField] private GameObject _currentEnemy;

    private Vector3 _lookDirection;
    private Vector3 _horizontalOnlyLookDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _lookDirection = _currentEnemy.transform.position - this.transform.position;

        
        _horizontalOnlyLookDirection.x = _lookDirection.x;
        //_horizontalOnlyLookDirection.y = _lookDirection.y; Don't set y, only want to pivot horizontally
        _horizontalOnlyLookDirection.z = _lookDirection.z;

        _horizontalAimPivot.transform.rotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
        _verticalAimPivot.transform.rotation = Quaternion.LookRotation(_lookDirection);

        Debug.DrawRay(_verticalAimPivot.transform.position, _lookDirection, Color.red);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject _horizontalAimPivot;
    [SerializeField] private GameObject _verticalAimPivot;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _standbySpeed;
    [SerializeField] private float _trackingSpeed;
    private Vector3 _neutralPosition;
    private Vector3 _lookDirection;
    private Vector3 _horizontalOnlyLookDirection;
    private Quaternion _horizontalOnlyRotation;

    [SerializeField] private GameObject _currentEnemy;

    private bool _resettingPosition;

    // Start is called before the first frame update
    void Start()
    {
        _neutralPosition.x = this.transform.position.x + 10;
        _neutralPosition.y = this.transform.position.y;
        _neutralPosition.z = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        SlerpAim();
    }

    //private void Aim()
    //{
    //    if (_currentEnemy != null)
    //        _lookDirection = _currentEnemy.transform.position - this.transform.position;
    //    else
    //        _lookDirection = _neutralPosition - this.transform.position;

    //    _horizontalOnlyLookDirection.x = _lookDirection.x;
    //    _horizontalOnlyLookDirection.z = _lookDirection.z;

    //    _horizontalAimPivot.transform.rotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
    //    _verticalAimPivot.transform.rotation = Quaternion.LookRotation(_lookDirection);
    //}

    private void SlerpAim()
    {
        if (_currentEnemy != null)
        {
            _lookDirection = _currentEnemy.transform.position - this.transform.position;
            _rotationSpeed = _trackingSpeed;
        }
        else
        {
            _lookDirection = _neutralPosition - this.transform.position;
            _rotationSpeed = _standbySpeed;
        }
            

        _horizontalOnlyLookDirection.x = _lookDirection.x;
        _horizontalOnlyLookDirection.z = _lookDirection.z;

        Quaternion lookRotation = Quaternion.LookRotation(_lookDirection);
        Quaternion horizontalOnlyLookRotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);

        _horizontalAimPivot.transform.rotation = Quaternion.Slerp(_horizontalAimPivot.transform.rotation, horizontalOnlyLookRotation, _rotationSpeed * Time.deltaTime);
        _verticalAimPivot.transform.rotation = Quaternion.Slerp(_verticalAimPivot.transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Mech") && _currentEnemy == null)
        {
            _currentEnemy = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentEnemy == other.gameObject)
        {
            _currentEnemy = null;
        }
    }

    private IEnumerator ResetAimPositionRoutine()
    {
        yield return new WaitForSeconds(1);

        _resettingPosition = false;
    }
}

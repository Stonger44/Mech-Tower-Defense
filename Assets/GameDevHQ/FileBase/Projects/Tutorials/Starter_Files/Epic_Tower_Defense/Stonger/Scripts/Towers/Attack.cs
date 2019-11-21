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

    [SerializeField] private Queue _targetQueue;
    [SerializeField] private GameObject _currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        if (_horizontalAimPivot == null)
            Debug.LogError("_horizontalAimPivot is NULL.");

        if (_verticalAimPivot == null)
            Debug.LogError("_verticalAimPivot is NULL.");

        _neutralPosition.x = this.transform.position.x + 10;
        _neutralPosition.y = this.transform.position.y;
        _neutralPosition.z = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        SlerpAim();
    }

    //Currently not using this, but might later
    private void Aim()
    {
        if (_currentTarget != null)
            _lookDirection = _currentTarget.transform.position - this.transform.position;
        else
            _lookDirection = _neutralPosition - this.transform.position;

        _horizontalOnlyLookDirection.x = _lookDirection.x;
        _horizontalOnlyLookDirection.z = _lookDirection.z;

        _horizontalAimPivot.transform.rotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
        _verticalAimPivot.transform.rotation = Quaternion.LookRotation(_lookDirection);
    }

    private void SlerpAim()
    {
        if (_currentTarget != null)
        {
            _lookDirection = _currentTarget.transform.position - this.transform.position;
            _rotationSpeed = _trackingSpeed;
        }
        else
        {
            _lookDirection = _neutralPosition - this.transform.position;
            _rotationSpeed = _standbySpeed;
        }
        Debug.DrawRay(this.transform.position, _lookDirection, Color.red);

        _horizontalOnlyLookDirection.x = _lookDirection.x;
        _horizontalOnlyLookDirection.z = _lookDirection.z;

        Quaternion lookRotation = Quaternion.LookRotation(_lookDirection);
        Quaternion horizontalOnlyLookRotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);

        _horizontalAimPivot.transform.rotation = Quaternion.Slerp(_horizontalAimPivot.transform.rotation, horizontalOnlyLookRotation, _rotationSpeed * Time.deltaTime);
        _verticalAimPivot.transform.rotation = Quaternion.Slerp(_verticalAimPivot.transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Mech") && _currentTarget == null)
        {
            _currentTarget = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentTarget == other.gameObject)
        {
            _currentTarget = null;
        }
    }
}

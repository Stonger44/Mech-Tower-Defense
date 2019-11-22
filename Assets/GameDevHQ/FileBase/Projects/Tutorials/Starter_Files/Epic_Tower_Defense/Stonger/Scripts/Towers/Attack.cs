using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject _horizontalAimPivot;
    [SerializeField] private GameObject _verticalAimPivot;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _standbySpeed;
    [SerializeField] private float _trackingSpeed;

    private Vector3 _neutralPosition;
    private Vector3 _lookDirection;
    private Vector3 _previousLookDirection;
    private Vector3 _horizontalOnlyLookDirection;
    private Quaternion _horizontalOnlyRotation;

    [SerializeField] private List<GameObject> _targetList = new List<GameObject>();
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
        //if no targets, move back to standby position
        if (_targetList.Count <= 0)
        {
            SlerpAim();
        }
    }

    #region Aim() with no Slerp, currently not in use
    //private void Aim()
    //{
    //    if (_currentTarget != null)
    //        _lookDirection = _currentTarget.transform.position - this.transform.position;
    //    else
    //        _lookDirection = _neutralPosition - this.transform.position;

    //    _horizontalOnlyLookDirection.x = _lookDirection.x;
    //    _horizontalOnlyLookDirection.z = _lookDirection.z;

    //    _horizontalAimPivot.transform.rotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
    //    _verticalAimPivot.transform.rotation = Quaternion.LookRotation(_lookDirection);
    //}
    #endregion

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

        var verticalOnlyLookDirection = new Vector3();
        verticalOnlyLookDirection.x = _lookDirection.x;
        verticalOnlyLookDirection.y = _lookDirection.y;
        verticalOnlyLookDirection.z = _lookDirection.z;        
        

        Quaternion horizontalOnlyLookRotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
        Quaternion verticalOnlyLookRotation = Quaternion.LookRotation(verticalOnlyLookDirection);
        //Quaternion lookRotation = Quaternion.LookRotation(_lookDirection);


        _horizontalAimPivot.transform.rotation = Quaternion.Slerp(_horizontalAimPivot.transform.rotation, horizontalOnlyLookRotation, _rotationSpeed * Time.deltaTime);
        _verticalAimPivot.transform.rotation = Quaternion.Slerp(_verticalAimPivot.transform.rotation, verticalOnlyLookRotation, _rotationSpeed * Time.deltaTime);
        //_verticalAimPivot.transform.rotation = Quaternion.Slerp(_verticalAimPivot.transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Mech"))
        {
            _targetList.Add(other.gameObject);
        }

        if (_currentTarget == null && _targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.gameObject);
            //_currentTarget = _targetList.FirstOrDefault(x => x.tag.Contains("Mech"));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == _currentTarget)
        {
            SlerpAim();
        }

        //Attack
    }

    private void OnTriggerExit(Collider other)
    {
        if (_targetList.Count > 0 && _targetList.Contains(other.gameObject))
        {
            _targetList.Remove(other.gameObject);
        }

        if (other.gameObject == _currentTarget)
        {
            _currentTarget = null;
        }

        if (_currentTarget == null && _targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.gameObject);
            //_currentTarget = _targetList.FirstOrDefault(x => x.tag.Contains("Mech"));
        }
    }
}

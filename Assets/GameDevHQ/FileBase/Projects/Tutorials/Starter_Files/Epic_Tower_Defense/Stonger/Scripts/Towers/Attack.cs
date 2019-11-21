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

    [SerializeField] private float _closeTargetMargin;

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
            SlerpAim();
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

        //This doesn't work. I'm trying to switch to no Slerp Aim to avoid an unnatural swinging rotation; I want the tower to stop immmediately when it catches up to the target.
        if ((_lookDirection.x - _previousLookDirection.x < _closeTargetMargin && _lookDirection.x - _previousLookDirection.x > -_closeTargetMargin)
            && (_lookDirection.y - _previousLookDirection.y < _closeTargetMargin && _lookDirection.y - _previousLookDirection.y > -_closeTargetMargin)
            && (_lookDirection.x - _previousLookDirection.z < _closeTargetMargin && _lookDirection.z - _previousLookDirection.z > -_closeTargetMargin))
        {
            _horizontalOnlyLookDirection.x = _lookDirection.x;
            _horizontalOnlyLookDirection.z = _lookDirection.z;

            _horizontalAimPivot.transform.rotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
            _verticalAimPivot.transform.rotation = Quaternion.LookRotation(_lookDirection);

            return;
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
        if (other.tag.Contains("Mech"))
        {
            _targetList.Add(other.gameObject);
        }

        if (_currentTarget == null && _targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.tag.Contains("Mech")); //return the first target in the queue and set as _currentEnemy
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == _currentTarget)
            SlerpAim();

        //Attack
    }

    private void OnTriggerExit(Collider other)
    {
        if (_targetList.Count > 0 && _targetList.Contains(other.gameObject))
            _targetList.Remove(other.gameObject);

        if (other.gameObject == _currentTarget)
        {
            _currentTarget = null;
        }
        
        if (_currentTarget == null && _targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.tag.Contains("Mech")); //return the first target in the queue and set as _currentEnemy
        }
    }
}

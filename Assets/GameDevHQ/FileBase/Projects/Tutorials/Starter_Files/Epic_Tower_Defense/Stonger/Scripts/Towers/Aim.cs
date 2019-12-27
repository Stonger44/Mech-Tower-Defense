using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Aim : MonoBehaviour
{
    [SerializeField] private GameObject _objectRoot;
    [SerializeField] private GameObject _horizontalAimPivot;
    [SerializeField] private GameObject _verticalAimPivot;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _standbySpeed;
    [SerializeField] private float _trackingSpeed;

    private Vector3 _neutralPosition;
    private Vector3 _lookDirection;
    private Vector3 _horizontalOnlyLookDirection;
    private Quaternion _horizontalOnlyLookRotation;
    private Quaternion _lookRotation;

    [SerializeField] private List<GameObject> _targetList = new List<GameObject>();
    [SerializeField] private GameObject _currentTarget;

    private string _targetName;

    public static event Action<GameObject, GameObject> onTargetInRange;
    public static event Action<GameObject> onNoTargetInRange;

    private void OnEnable()
    {
        Enemy.onDying += CheckCurrentTarget;

        SetNeutralPosition();
    }

    private void OnDisable()
    {
        Enemy.onDying -= CheckCurrentTarget;

        _targetList.Clear();
        _currentTarget = null;

        //Reset object position
        SetNeutralPosition();
        NoSlerpAim();
    }

    public void SetNeutralPosition()
    {
        if (_objectRoot == null)
            Debug.LogError("_objectRoot is NULL.");

        if (_horizontalAimPivot == null)
            Debug.LogError("_horizontalAimPivot is NULL.");

        if (_verticalAimPivot == null && !_objectRoot.name.Contains("Missile"))
            Debug.LogError("_verticalAimPivot is NULL.");

        _neutralPosition.x = this.transform.position.x + 10;
        _neutralPosition.y = this.transform.position.y;
        _neutralPosition.z = this.transform.position.z;
    }

    // Start is called before the first frame update
    void Start()
    {
        _targetName = "Mech";
    }

    // Update is called once per frame
    void Update()
    {
        //if no targets, move back to standby position
        if (_targetList.Count <= 0)
        {
            SlerpAim();
            onNoTargetInRange?.Invoke(_objectRoot);
        }
    }

    private void NoSlerpAim()
    {
        if (_currentTarget != null)
            _lookDirection = _currentTarget.transform.position - this.transform.position;
        else
            _lookDirection = _neutralPosition - this.transform.position;

        _horizontalOnlyLookDirection.x = _lookDirection.x;
        _horizontalOnlyLookDirection.z = _lookDirection.z;

        _horizontalAimPivot.transform.rotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
        if (!_objectRoot.name.Contains("Missile"))
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
        //Debug.DrawRay(this.transform.position, _lookDirection, Color.red);

        _horizontalOnlyLookDirection.x = _lookDirection.x;
        _horizontalOnlyLookDirection.z = _lookDirection.z;

        _horizontalOnlyLookRotation = Quaternion.LookRotation(_horizontalOnlyLookDirection);
        _horizontalAimPivot.transform.rotation = Quaternion.Slerp(_horizontalAimPivot.transform.rotation, _horizontalOnlyLookRotation, _rotationSpeed * Time.deltaTime);

        //Missile Launchers only have horizontal rotation
        if (!_objectRoot.name.Contains("Missile"))
        {
            _lookRotation = Quaternion.LookRotation(_lookDirection);
            _verticalAimPivot.transform.rotation = Quaternion.Slerp(_verticalAimPivot.transform.rotation, _lookRotation, _rotationSpeed * Time.deltaTime);
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains(_targetName) && _targetList.Contains(other.gameObject) == false)
            _targetList.Add(other.gameObject);

        if (_currentTarget == null && _targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.gameObject);

            if (_currentTarget != null)
            {
                SlerpAim();
                onTargetInRange?.Invoke(_objectRoot, _currentTarget);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Contains(_targetName) && _targetList.Contains(other.gameObject) == false)
            _targetList.Add(other.gameObject);

        if (_currentTarget == null && _targetList.Contains(other.gameObject))
            _currentTarget = other.gameObject;

        if (other.gameObject == _currentTarget)
        {
            SlerpAim();
            onTargetInRange?.Invoke(_objectRoot, _currentTarget);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_targetList.Count > 0 && _targetList.Contains(other.gameObject))
        {
            _targetList.Remove(other.gameObject);
            onNoTargetInRange?.Invoke(_objectRoot);
        }

        if (other.gameObject == _currentTarget)
            _currentTarget = null;


        if (_currentTarget == null && _targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.gameObject);

            SlerpAim();
            onTargetInRange?.Invoke(_objectRoot, _currentTarget);
        }
    }

    private void CheckCurrentTarget(GameObject destroyedTarget)
    {   
        if (_targetList.Contains(destroyedTarget))
            _targetList.Remove(destroyedTarget);

        if (destroyedTarget == _currentTarget)
            _currentTarget = null;

        if (_targetList.Count > 0)
        {
            _currentTarget = _targetList.FirstOrDefault(x => x.gameObject);
            onTargetInRange?.Invoke(_objectRoot, _currentTarget);
        }
        else
        {
            onNoTargetInRange?.Invoke(_objectRoot);
        }

        SlerpAim();
    }
}

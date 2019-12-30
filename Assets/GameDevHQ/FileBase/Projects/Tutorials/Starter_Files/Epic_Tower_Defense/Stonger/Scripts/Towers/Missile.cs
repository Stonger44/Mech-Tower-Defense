using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //require rigidbody
[RequireComponent(typeof(AudioSource))] //require audiosource
public class Missile : Explodable
{
    [SerializeField]
    private ParticleSystem _particle; //reference to the particle system

    [SerializeField]
    private float _launchSpeed; //launch speed of the rocket
    [SerializeField]
    private float _power; //power of the rocket
    [SerializeField] //fuse delay of the rocket
    private float _fuseDelay;

    private Rigidbody _rigidbody; //reference to the rigidbody of the rocket
    private AudioSource _audioSource; //reference to the audiosource of the rocket

    private bool _launched = false; //bool for if the rocket has launched
    private float _initialLaunchTime = 2.0f; //initial launch time for the rocket
    private bool _thrust; //bool to enable the rocket thrusters

    private bool _fuseOut = false; //bool for if the rocket fuse
    private bool _trackRotation = false; //bool to track rotation of the rocket

    //Extended Code
    private GameObject _attackAltitude;
    private GameObject _detonationAltitude;
    private GameObject _currentTarget;
    [SerializeField] private float _targetPositionRandomRange;
    private Vector3 _approximateTargetPosition;
    private Enemy _currentTargetScript;
    private bool _isSeekingTarget;
    private Vector3 _lookDirection;
    private int _damageAmount;
    [SerializeField] private float _blastRadius;
    private bool _isMissileDetonating;

    [SerializeField] private Collider[] _hitEnemiesArray = new Collider[50];

    private GameObject _missileLauncher;

    //public static event Action<GameObject, GameObject, int> onTargetHit;
    public static event Action<GameObject, Collider[]> onDetonate;

    // Use this for initialization
    IEnumerator Start()
    {
        _attackAltitude = GameObject.Find("AttackAltitude");
        _detonationAltitude = GameObject.Find("DetonationAltitude");

        _rigidbody = GetComponent<Rigidbody>(); //assign the rigidbody component 
        _audioSource = GetComponent<AudioSource>(); //assign the audiosource component
        _audioSource.pitch = UnityEngine.Random.Range(0.7f, 1.9f); //randomize the pitch of the rocket audio
        _particle.Play(); //play the particles of the rocket
        _audioSource.Play(); //play the rocket sound

        yield return new WaitForSeconds(_fuseDelay); //wait for the fuse delay

        _initialLaunchTime = Time.time + 1.0f; //set the initial launch time
        _fuseOut = true; //set fuseOut to true
        _launched = true; //set the launch bool to true 
        _thrust = false; //set thrust bool to false

    }

    private void OnEnable()
    {
        _initialLaunchTime = Time.time; //set the initial launch time
        _fuseOut = true; //set fuseOut to true
        _launched = true; //set the launch bool to true 
        _trackRotation = false;

        if (_rigidbody != null)
        {
            _rigidbody.useGravity = true;
            _rigidbody.detectCollisions = true;
        }

        _approximateTargetPosition = Vector3.zero;

        Array.Clear(_hitEnemiesArray, 0, _hitEnemiesArray.Length);
    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isSeekingTarget)
        {
            LockOnToTarget();
        }

        MoveMissile();
    }

    private void MoveMissile()
    {
        if (_fuseOut == false) //check if fuseOut is false
            return;

        if (_launched == true) //check if launched is true
        {
            _rigidbody.AddForce(transform.forward * _launchSpeed); //add force to the rocket in the forward direction

            if (Time.time > _initialLaunchTime + _fuseDelay) //check if the initial launch + fuse delay has passed
            {
                _launched = false; //launched bool goes false
                _thrust = true; //thrust bool goes true
            }
        }

        if (_thrust == true) //if thrust is true
        {
            _rigidbody.useGravity = true; //enable gravity 
            _rigidbody.velocity = transform.forward * _power; //set velocity multiplied by the power variable
                                                              //_thrust = false; //set thrust bool to false
            _trackRotation = true; //track rotation bool set to true
        }

        if (_trackRotation == true) //check track rotation bool
        {
            _rigidbody.rotation = Quaternion.LookRotation(_rigidbody.velocity); // adjust rotation of rocket based on velocity
            _rigidbody.AddForce(transform.forward * 50f); //add force to the rocket
        }
    }

    /// <summary>
    /// This method is used to assign traits to our missle assigned from the launcher.
    /// </summary>
    public void AssignMissileRules(float launchSpeed, float power, float fuseDelay, float destroyTimer, GameObject missileLauncher, GameObject currentTarget, int damagAmount)
    {
        _launchSpeed = launchSpeed; //set the launch speed
        _power = power; //set the power
        _fuseDelay = fuseDelay; //set the fuse delay

        _missileLauncher = missileLauncher;
        _currentTarget = currentTarget;
        _damageAmount = damagAmount;

        if (_currentTarget != null)
            _currentTargetScript = _currentTarget.GetComponent<Enemy>();

        if (_currentTarget == null)
            Debug.LogError("_currentTarget is NULL.");

        //Reset
        StartCoroutine(ResetMissile(destroyTimer));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _attackAltitude)
        {
            _isSeekingTarget = IsCurrentEnemyAnActiveTarget();
        }

        if (other.gameObject == _detonationAltitude || other.gameObject.tag.Contains("Mech"))
        {
            if (!_isMissileDetonating)
            {
                _isMissileDetonating = true;
                DetonateMissile(other);
            }
        }

    }

    private void LockOnToTarget()
    {
        //Target lost
        if (IsCurrentEnemyAnActiveTarget() == false)
        {
            _isSeekingTarget = false;
            return;
        }

        //_lookDirection = _currentTarget.transform.position - this.transform.position;

        //Randomize the target position a bit so the missiles aren't ridiculously accurate
        _approximateTargetPosition.x = _currentTarget.transform.position.x + UnityEngine.Random.Range(-_targetPositionRandomRange, _targetPositionRandomRange);
        //_approximateTargetPosition.y = _currentTarget.transform.position.y + UnityEngine.Random.Range(-_targetPositionRandomRange, _targetPositionRandomRange);
        _approximateTargetPosition.z = _currentTarget.transform.position.z + UnityEngine.Random.Range(-_targetPositionRandomRange, _targetPositionRandomRange);

        _lookDirection = _approximateTargetPosition - this.transform.position;

        this.transform.rotation = Quaternion.LookRotation(_lookDirection);
    }

    private IEnumerator ResetMissile(float resetTimer)
    {
        yield return new WaitForSeconds(resetTimer);

        //If the missile is still active after the resetTime, then reset the missile
        if (this.gameObject.activeSelf == true)
        {
            ResetMissile();
            this.gameObject.SetActive(false);
        }
    }

    private void DetonateMissile(Collider other)
    {
        //Detonate:
        PlayExplosion();

        int hitEnemyCount = Physics.OverlapSphereNonAlloc(this.transform.position, _blastRadius, _hitEnemiesArray, 1 << 8);

        if (hitEnemyCount > 0)
            onDetonate?.Invoke(_missileLauncher, _hitEnemiesArray);

        ResetMissile();

        _isMissileDetonating = false;
        this.gameObject.SetActive(false);
    }

    private void ResetMissile()
    {
        _fuseOut = false;
        this._audioSource.Stop();
        _currentTarget = null;
        _isSeekingTarget = false;

        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.detectCollisions = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        PoolManager.Instance.ResetMissileTransform(this.gameObject);
    }

    private bool IsCurrentEnemyAnActiveTarget()
    {
        bool currentEnemyIsActiveTarget = false;

        if (_currentTarget != null && _currentTarget.activeSelf != false)
        {
            if (_currentTargetScript != null && _currentTargetScript.IsOnStandby() == false && _currentTargetScript.IsInJunkyard() == false && _currentTargetScript.IsDying() == false)
            {
                currentEnemyIsActiveTarget = true;
            }
        }

        return currentEnemyIsActiveTarget;
    }
}

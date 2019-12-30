using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Explodable
{
    public static int enemyCount;

    private Vector3 _spawnPoint;
    private Vector3 _endPoint;
    private Vector3 _standbyPoint;
    private Vector3 _junkyard;

    private NavMeshAgent _navMeshAgent;

    [SerializeField] private int _initialHealth;
    [SerializeField] private int _health;
    private float _healthPercent;
    [SerializeField] private int _warFunds;
    public int warFunds { get; private set; }
    [SerializeField] private bool _onStandby = false;
    [SerializeField] private bool _inJunkyard = false;

    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isDying;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _navMeshRadius;
    private Quaternion _defaultRotation;

    //private Collider[] _hitEnemiesArray = new Collider[50];
    [SerializeField] private int _missileDamageAmount;
    private bool _isMissileBarrageDamageRoutineRunning = false;

    /*----------Death Routine----------*/
    private WaitForSeconds _waitForSeconds_StopMovingForward = new WaitForSeconds(0.2f);
    private WaitForSeconds _waitForSeconds_SignalDeath = new WaitForSeconds(1.0f);
    private WaitForSeconds _waitForSeconds_DeathAnimation = new WaitForSeconds(3.0f);
    private WaitForSeconds _waitForSeconds_Explosion = new WaitForSeconds(0.5f);
    /*----------Death Routine----------*/

    /*----------Shoot Routine----------*/
    private WaitForSeconds _waitForSeconds_Aim = new WaitForSeconds(0.1f);
    private WaitForSeconds _waitForSeconds_InitialFireDelay;
    private WaitForSeconds _waitForSeconds_FireDelay;
    private WaitForSeconds _waitForSeconds_ResetAim = new WaitForSeconds(1.0f);
    /*----------Shoot Routine----------*/

    /*----------Aiming----------*/
    [SerializeField] private GameObject _aimPivot;
    [SerializeField] private GameObject _neutralLookPointObject;

    [SerializeField] private GameObject _attackingObject;
    [SerializeField] private GameObject _currentTarget;

    private Vector3 _lookDirection;
    private Quaternion _lookRotation;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _trackingSpeed;

    [SerializeField] private bool _isReturningFire;
    [SerializeField] private bool _isAiming;

    [SerializeField] private float _initialFireDelay;
    [SerializeField] private int _roundCount;
    [SerializeField] private float _fireDelay;

    [SerializeField] private AudioSource _shootSound;

    [SerializeField] private List<GameObject> _muzzleFlashList;

    [SerializeField] private int _damageToDeal;
    /*----------Aiming----------*/

    public static event Action<GameObject> onDying; //Used to stop the towers from targeting an already dead target
    public static event Action<GameObject> onDeath; //GameManager uses this to decrement enemyCount and add warFunds
    public static event Action<GameObject> onResetComplete; //After this broadcast the (last) enemy has already reset itself so the next wave can start.
    public static event Action<GameObject, int> onAttack;
    public static event Action<GameObject, float> onHealthUpdate;

    private void OnEnable()
    {
        Gatling_Gun.onShoot += TakeDamage;
        //Missile.onTargetHit += TakeDamage;
        Missile.onDetonate += OnMissileDetonation;

        enemyCount++;

        _spawnPoint = SpawnManager.Instance.spawnPoint.transform.position;
        _endPoint = SpawnManager.Instance.endPoint.transform.position;
        _standbyPoint = SpawnManager.Instance.standbyPoint.transform.position;
        _junkyard = SpawnManager.Instance.junkyard.transform.position;

        SetToStandby();
    }

    private void OnDisable()
    {
        Gatling_Gun.onShoot -= TakeDamage;
        //Missile.onTargetHit -= TakeDamage;
        Missile.onDetonate -= OnMissileDetonation;

        enemyCount--;

        _onStandby = false;
        _isMissileBarrageDamageRoutineRunning = false;
        //_hitEnemiesArray = null;
    }

    private void OnMissileDetonation(GameObject attackingObject, Collider[] hitEnemiesArray)
    {
        //if (_isMissileBarrageDamageRoutineRunning)
        //    return;

        //if (_hitEnemiesArray == null)
        //    _hitEnemiesArray = hitEnemiesArray;

        if (hitEnemiesArray.FirstOrDefault(collider => collider == _collider) != null)
        {
            if (_isDying == false && _onStandby == false && _inJunkyard == false)
            {
                //if (_isMissileBarrageDamageRoutineRunning == false)
                //{
                    //_isMissileBarrageDamageRoutineRunning = true;
                    //StartCoroutine(MissileBarrageDamageRoutine(attackingObject));
                    TakeDamage(attackingObject, this.gameObject, _missileDamageAmount);
                //}
            }
        }

        //foreach (var enemy in hitEnemiesArray)
        //{
        //    if (enemy.collider == _collider)
        //    {
        //        if (_isDying == false && _onStandby == false && _inJunkyard == false)
        //        {
        //            if (_isMissileBarrageDamageRoutineRunning == false)
        //            {
        //                _isMissileBarrageDamageRoutineRunning = true;
        //                StartCoroutine(MissileBarrageDamageRoutine(attackingObject));
        //            }
        //        }
        //    }
        //}
    }

    private WaitForSeconds _waitForSeconds_MissileDamageDelay = new WaitForSeconds(0.2f);

    private IEnumerator MissileBarrageDamageRoutine(GameObject attackingObject)
    {
        int missilesLaunched = (attackingObject.CompareTag("Tower_Missile_Launcher_Upgrade")) ? 12 : 6;

        for (int i = 0; i < missilesLaunched; i++)
        {
            if (_isDying == true || _onStandby == true || _inJunkyard == true)
                break;

            TakeDamage(attackingObject, this.gameObject, _missileDamageAmount);

            yield return _waitForSeconds_MissileDamageDelay;
        }

        //_hitEnemiesArray = null;
        _isMissileBarrageDamageRoutineRunning = false;
    }

    private void Start()
    {
        warFunds = _warFunds;
        _defaultRotation = this.transform.rotation;

        if (_collider == null)
            Debug.LogError("_collider is NULL.");

        if (_shootSound == null)
            Debug.LogError("_shootSound is NULL.");

        _waitForSeconds_InitialFireDelay = new WaitForSeconds(_initialFireDelay);
        _waitForSeconds_FireDelay = new WaitForSeconds(_fireDelay);

    }

    private void Update()
    {
        if (_isAiming)
            Aim();
    }

    public void SetToAttack()
    {
        _healthPercent = (float)_health / (float)_initialHealth;
        onHealthUpdate?.Invoke(this.gameObject, _healthPercent);

        _onStandby = false;
        _navMeshAgent.enabled = true;
        _navMeshAgent.radius = _navMeshRadius;
        _navMeshAgent.Warp(_spawnPoint);
        _navMeshAgent.SetDestination(_endPoint);
    }

    public void SetToStandby()
    {
        DisableNavMesh();

        MuzzleFlashes_SetActive(false);

        this.transform.position = _standbyPoint;
        _isDying = false;
        _inJunkyard = false;
        _health = _initialHealth;
        _onStandby = true;
        _attackingObject = null;
    }

    public bool IsOnStandby() => _onStandby;
    public bool IsInJunkyard() => _inJunkyard;
    public bool IsDying() => _isDying;

    private void MuzzleFlashes_SetActive(bool activeState)
    {
        foreach (var muzzleFlash in _muzzleFlashList)
        {
            muzzleFlash.SetActive(activeState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "EndPoint")
        {
            SetToStandby();
        }
    }

    private void TakeDamage(GameObject attackingObject, GameObject currentTarget, int damageAmount)
    {
        if (this.gameObject == currentTarget)
        {
            _health -= damageAmount;

            //Death
            if (_health <= 0)
            {
                _health = 0;
                if (!_isDying)
                {
                    _isDying = true;
                    StopShooting();
                    StartCoroutine(DieRoutine());
                }
            }

            _healthPercent = (float)_health / (float)_initialHealth;
            onHealthUpdate?.Invoke(this.gameObject, _healthPercent);

            if (_isDying)
                return;

            if (IsAttackingObjectATower(attackingObject))
            {
                if (!_isReturningFire)
                {
                    _attackingObject = attackingObject;
                    _isReturningFire = true;
                    StartCoroutine(ShootRoutine());
                }
            }
        }
    }

    private bool IsAttackingObjectATower(GameObject attackingObject)
    {
        if (attackingObject.CompareTag("Tower_Gatling_Gun")
            || attackingObject.CompareTag("Tower_Gatling_Gun_Upgrade")
            || attackingObject.CompareTag("Tower_Missile_Launcher")
            || attackingObject.CompareTag("Tower_Missile_Launcher_Upgrade"))
        {
            return true;
        }

        return false;
    }

    private void StopShooting()
    {
        _attackingObject = null;
        _animator.SetBool("IsShooting", false);
        _shootSound.enabled = false;
        _isReturningFire = false;
        MuzzleFlashes_SetActive(false);
    }

    private IEnumerator ShootRoutine()
    {
        //Look/Aim
        yield return new WaitForSeconds(0.1f);
        _isAiming = true;

        //Shoot
        _animator.SetBool("IsShooting", true);
        yield return _waitForSeconds_InitialFireDelay;
        for (int i = 0; i < _roundCount; i++)
        {
            //Shoot
            _shootSound.Play();
            MuzzleFlashes_SetActive(true);
            onAttack?.Invoke(_attackingObject, _damageToDeal);

            //Turn off muzzle flash
            yield return _waitForSeconds_FireDelay;
            MuzzleFlashes_SetActive(false);

            if (_isDying || _attackingObject == null || _attackingObject.activeSelf == false)
                break;

            //FireDelay
            yield return _waitForSeconds_FireDelay;
        }
        _shootSound.Stop();

        //Stop Shooting
        _attackingObject = null;
        _animator.SetBool("IsShooting", false);

        //Return Mech to neutral look position
        yield return _waitForSeconds_ResetAim;
        _isAiming = false;
        _isReturningFire = false;
    }

    private void Aim()
    {
        _currentTarget = (_attackingObject == null) ? _neutralLookPointObject : _attackingObject;

        _lookDirection = _currentTarget.transform.position - _aimPivot.transform.position;
        _rotationSpeed = _trackingSpeed;

        //Debug.DrawRay(_aimPivot.transform.position, _lookDirection, Color.green);

        _lookRotation = Quaternion.LookRotation(_lookDirection);
        _aimPivot.transform.rotation = Quaternion.Slerp(_aimPivot.transform.rotation, _lookRotation, _rotationSpeed * Time.deltaTime);
    }

    private void DisableNavMesh()
    {
        if (_navMeshAgent == null)
            _navMeshAgent = this.GetComponent<NavMeshAgent>();

        _navMeshAgent.enabled = false;
    }

    private IEnumerator DieRoutine()
    {
        //Death Animation
        _animator.SetBool("IsDying", true);

        //Stop moving forward
        yield return _waitForSeconds_StopMovingForward;
        _navMeshAgent.isStopped = true;

        //Signal death (basically to get towers to stop shooting at the enemy; it's already dying, so it's already dead)
        yield return _waitForSeconds_SignalDeath;
        _collider.enabled = false;
        onDying?.Invoke(this.gameObject);

        //Play death animation before the...
        yield return _waitForSeconds_DeathAnimation;

        //...Explosion!
        PlayExplosion();

        //Hide enemy (because it exploded)
        yield return _waitForSeconds_Explosion;
        SendToJunkyard();
        onDeath?.Invoke(this.gameObject);
        ResetEnemy();

        yield return _waitForSeconds_ResetAim;
        _isAiming = false;

        this.gameObject.SetActive(false);
        onResetComplete?.Invoke(this.gameObject); //This will signal the end of the wave, but enemy must reset first!

        _isDying = false;
    }

    private void SendToJunkyard()
    {
        DisableNavMesh();

        this.transform.position = _junkyard;
        this.transform.rotation = _defaultRotation;
        _inJunkyard = true;
    }

    private void ResetEnemy()
    {
        _currentTarget = null;
        _animator.SetBool("IsDying", false);
        _collider.enabled = true;
        _shootSound.enabled = true;
        _isAiming = true; //reset aim position
    }
}

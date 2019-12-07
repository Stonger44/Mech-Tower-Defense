using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int _warFunds;
    public int warFunds { get; private set; }
    [SerializeField] private bool _onStandby = false;
    [SerializeField] private bool _inJunkyard = false;

    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isDying;
    [SerializeField] private GameObject _skin;
    [SerializeField] private float _navMeshRadius;
    private Quaternion _originalRotation;

    public static event Action<GameObject> onDying; //Used to stop the towers from targeting an already dead target
    public static event Action<GameObject> onDeath; //GameManager uses this to decrement enemyCount and add warFunds
    public static event Action<GameObject> onResetComplete; //After this broadcast the (last) enemy has already reset itself so the next wave can start.

    private void OnEnable()
    {
        Gatling_Gun.onShoot += TakeDamage;
        Missile.onTargetHit += TakeDamage;

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
        Missile.onTargetHit -= TakeDamage;

        enemyCount--;

        _onStandby = false;
    }

    private void Start()
    {
        warFunds = _warFunds;
        _originalRotation = this.gameObject.transform.rotation;
    }

    private void Update()
    {

    }

    public void SetToAttack()
    {
        _onStandby = false;
        _navMeshAgent.enabled = true;
        _navMeshAgent.radius = _navMeshRadius;
        _navMeshAgent.Warp(_spawnPoint);
        _navMeshAgent.SetDestination(_endPoint);
    }

    public void SetToStandby()
    {
        DisableNavMesh();

        this.transform.position = _standbyPoint;
        _inJunkyard = false;
        _onStandby = true;
    }

    public bool IsOnStandby() => _onStandby;
    public bool IsInJunkyard() => _inJunkyard;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "EndPoint")
        {
            SetToStandby();
        }
    }

    private void TakeDamage(GameObject currentTarget, int damageAmount)
    {
        if (this.gameObject == currentTarget)
        {
            _health -= damageAmount;

            if (_health <= 0)
            {
                _health = 0;
                if (!_isDying)
                {
                    _isDying = true;
                    StartCoroutine(DieRoutine());
                }
            }
        }
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
        yield return new WaitForSeconds(0.2f);
        _navMeshAgent.isStopped = true;

        //Signal death (basically to get towers to stop shooting at the enemy; it's already dying, so it's already dead)
        yield return new WaitForSeconds(0.5f);
        onDying?.Invoke(this.gameObject);

        //Play death animation a bit before the...
        yield return new WaitForSeconds(1.9f);

        //...Explosion!
        PlayExplosion();

        //Hide enemy (because it exploded)
        yield return new WaitForSeconds(0.5f);
        SendToJunkyard();
        onDeath?.Invoke(this.gameObject);
        
        //Must wait until death animation completely finishes before resetting enemy
        yield return new WaitForSeconds(3.0f);
        ResetEnemy();

        //Wait for smoke animation to finish
        yield return new WaitForSeconds(1.75f);
        PoolManager.Instance.ResetExplosion(_explosion);

        this.gameObject.SetActive(false);
        onResetComplete?.Invoke(this.gameObject); //This will signal the end of the wave, but enemy must reset first!

        _isDying = false;
    }

    private void SendToJunkyard()
    {
        DisableNavMesh();

        this.transform.position = _junkyard;
        this.transform.rotation = _originalRotation;
        _inJunkyard = true;
    }

    private void ResetEnemy()
    {
        _health = _initialHealth;
        _animator.SetBool("IsDying", false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameDevHQ.FileBase.Gatling_Gun;
using GameDevHQ.FileBase.Missle_Launcher;

public class Enemy : MonoBehaviour
{
    public static int enemyCount;

    private Vector3 _spawnPoint;
    private Vector3 _endPoint;
    private Vector3 _standbyPoint;
    private Vector3 _junkyard;

    private NavMeshAgent _navMeshAgent;

    [SerializeField] private int _initialHealth;
    [SerializeField] private int _health;
    [SerializeField] private int _warFund;
    public int warFund { get; private set; }
    [SerializeField] private bool _onStandby = false;
    [SerializeField] private bool _inJunkyard = false;

    [SerializeField] private GameObject _explosionObject;
    [SerializeField] private AudioSource _explosionSound;

    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isDying;
    [SerializeField] private GameObject _skin;
    [SerializeField] private float _navMeshRadius;

    public static event Action<GameObject> onDying; //Used to stop the towers from targeting an already dead target
    public static event Action<GameObject> onExplosion; //GameManager uses this to decrement enemyCount and add warFund
    public static event Action<GameObject> onDeath; //After this broadcast the (last) enemy has already reset itself so the next wave can start.

    private void OnEnable()
    {
        Gatling_Gun.onShoot += TakeDamage;
        Missle_Launcher.onMissileHit += TakeDamage;

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
        Missle_Launcher.onMissileHit -= TakeDamage;

        enemyCount--;
    }

    private void Start()
    {
        warFund = _warFund;
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

    private void SendToJunkyard()
    {
        DisableNavMesh();

        this.transform.position = _junkyard;
        _inJunkyard = true;
    }

    private void ResetEnemy()
    {
        if (_explosionObject == null)
            Debug.LogError("_explosionObject is NULL.");

        _explosionObject.SetActive(false);

        _skin.SetActive(true);

        _health = _initialHealth;

        _animator.SetBool("IsDying", false);
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

        //Single death (basically to get towers to stop shooting at the enemy, it's already dying, so it's already dead)
        yield return new WaitForSeconds(0.5f);
        onDying?.Invoke(this.gameObject);

        //Wait for death animation to finish
        yield return new WaitForSeconds(4.0f);

        //...Explosion!
        _explosionObject.SetActive(true);
        _explosionSound.Play();

        //Hide enemy (because it exploded)
        yield return new WaitForSeconds(0.5f);
        _skin.SetActive(false);
        _navMeshAgent.radius = 0f;
        //yield return new WaitForSeconds(0.5f);
        onExplosion?.Invoke(this.gameObject);

        //Wait for smoke animation to finish
        yield return new WaitForSeconds(4.44f);
        SendToJunkyard();
        ResetEnemy();
        //Wait for animation to reset to running
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
        onDeath?.Invoke(this.gameObject);

        _isDying = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameDevHQ.FileBase.Gatling_Gun;

public class Enemy : MonoBehaviour
{
    public static int enemyCount;

    private Vector3 _spawnPoint;
    private Vector3 _endPoint;
    private Vector3 _standbyPoint;

    private NavMeshAgent _navMeshAgent;

    [SerializeField] private int _initialHealth;
    [SerializeField] private int _health;
    [SerializeField] private int _warFund;
    public int warFund { get; private set; }
    [SerializeField] private bool _onStandby = false;

    [SerializeField] private GameObject _explosionObject;
    [SerializeField] private AudioSource _explosionSound;

    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isDying;
    [SerializeField] private GameObject _skin;

    public static event Action<GameObject> onDeath;

    private void OnEnable()
    {
        Gatling_Gun.onShoot += TakeDamage;

        enemyCount++;

        _spawnPoint = SpawnManager.Instance.spawnPoint.transform.position;
        _endPoint = SpawnManager.Instance.endPoint.transform.position;
        _standbyPoint = SpawnManager.Instance.standbyPoint.transform.position;

        _health = _initialHealth;
        _skin.SetActive(true);
        _animator.SetBool("IsDying", false);

        if (_explosionObject == null)
            Debug.LogError("_explosionObject is NULL.");

        _explosionSound.Stop();
        _explosionObject.SetActive(false);

        SetToStandby();
    }

    private void OnDisable()
    {
        Gatling_Gun.onShoot -= TakeDamage;

        enemyCount--;
    }

    private void Start()
    {
        warFund = _warFund;
    }

    public GameObject GetSkin()
    {
        return _skin;
    }

    public void SetToAttack()
    {
        _onStandby = false;
        _navMeshAgent.enabled = true;
        _navMeshAgent.Warp(_spawnPoint);
        _navMeshAgent.SetDestination(_endPoint);
    }

    public void SetToStandby()
    {
        if (_navMeshAgent == null)
            _navMeshAgent = this.GetComponent<NavMeshAgent>();

        _navMeshAgent.enabled = false;
        this.transform.position = _standbyPoint;

        _onStandby = true;
    }

    public bool IsOnStandby() => _onStandby;

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
                    StartCoroutine(DieRoutine());
            }
        }
    }

    private IEnumerator DieRoutine()
    {
        _isDying = true;

        //Death Animation
        _animator.SetBool("IsDying", true);


        if (this.gameObject.tag == "Mech1")
        {
            yield return new WaitForSeconds(0.5f);
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(3.5f);

        }
        else if (this.gameObject.tag == "Mech2")
        {
            yield return new WaitForSeconds(1f);
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(2f);
        }

        //Death Explosion
        _explosionObject.SetActive(true);
        _explosionSound.Play();

        yield return new WaitForSeconds(0.44f);

        _skin.SetActive(false);
        onDeath?.Invoke(this.gameObject);

        yield return new WaitForSeconds(4.44f);

        this.gameObject.SetActive(false);

        _isDying = false;
    }
}

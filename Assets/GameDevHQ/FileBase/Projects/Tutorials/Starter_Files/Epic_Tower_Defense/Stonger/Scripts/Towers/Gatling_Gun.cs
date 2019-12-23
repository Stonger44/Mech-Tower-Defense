using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))] //Require Audio Source component
public class Gatling_Gun : Explodable, ITower
{
    [SerializeField] private Transform[] _gunBarrel; //Reference to hold the gun barrel
    [SerializeField] private GameObject[] _muzzleFlash; //reference to the muzzle flash effect to play when firing
    [SerializeField] private ParticleSystem[] _bulletCasings; //reference to the bullet casing effect to play when firing
    [SerializeField] private AudioClip _fireSound; //Reference to the audio clip

    private AudioSource _audioSource; //reference to the audio source component
    private bool _startWeaponNoise = true;

    /*----------Extended Code----------*/
    [SerializeField] private int _startingHealth; //Only here so I can see it in the inspector
    [SerializeField] private int _health; //Only here so I can see it in the inspector
    private float _healthPercent;

    public TowerSprites TowerSprites { get; set; }
    [SerializeField] private TowerSprites _towerSprites;

    public string Name { get; set; }

    public int Health { get; set; }
    public int InitialHealth { get; set; }// = 500;
    public int UpgradeInitialHealth { get; set; }// = 1000;

    public int DamageTaken { get; set; }

    public int WarFundCost { get; set; }// = 500;
    public int WarFundSellValue { get; set; }// = 250;
    public int WarFundRepairCost { get; set; }
    
    public int UpgradeWarFundCost { get; set; }// = 1000;
    public int UpgradeWarFundSellValue { get; set; }// = 500;
    public int UpgradeWarFundRepairCost { get; set; }

    [SerializeField] private GameObject _towerRange;

    private bool _isAttacking;
    [SerializeField] private float _fireDelay;
    private bool _isDying;
    [SerializeField] private int _damageToDeal;

    public static event Action<GameObject, GameObject, int> onShoot;
    public static event Action<GameObject> onDeath;
    public static event Action<GameObject, float> onHealthUpdate;
    public static event Action<int> onBroadcastTowerWarFundValue;

    private void OnEnable()
    {
        Aim.onTargetInRange += Shoot;
        Aim.onNoTargetInRange += StopShooting;
        TowerLocation.onViewingCurrentTower += ToggleTowerRange;
        TowerLocation.onSetNewTowerHealth += UpdateHealthBar;
        TowerLocation.onRepairedCurrentTowerHealth += ResetHealth;
        TowerManager.onStopViewingTower += ToggleTowerRange;
        Enemy.onAttack += TakeDamage;
        GameManager.onSelfDestructTowers += SelfDestruct;
        GameManager.onCollectCurrentActiveTowersTotalWarFundValue += BroadcastTowerWarFundValue;

        if (this.gameObject.tag.Contains("Upgrade"))
        {
            UpgradeInitialHealth = _startingHealth;
            InitialHealth = UpgradeInitialHealth / 2;
            Health = UpgradeInitialHealth;
        }
        else
        {
            InitialHealth = _startingHealth;
            UpgradeInitialHealth = InitialHealth * 2;
            Health = InitialHealth;
        }

        _health = Health;

        Name = this.gameObject.tag;

        WarFundCost = InitialHealth;
        WarFundSellValue = WarFundCost / 2;
        WarFundRepairCost = (int)(WarFundCost * 0.75f);

        UpgradeWarFundCost = UpgradeInitialHealth;
        UpgradeWarFundSellValue = UpgradeWarFundCost / 2;
        UpgradeWarFundRepairCost = (int)(UpgradeWarFundCost * 0.75f);

        TowerSprites = _towerSprites;
        if (TowerSprites == null)
            Debug.LogError("TowerUI is NULL.");

        _isDying = false;
    }

    private void OnDisable()
    {
        Aim.onTargetInRange -= Shoot;
        Aim.onNoTargetInRange -= StopShooting;
        TowerLocation.onViewingCurrentTower -= ToggleTowerRange;
        TowerLocation.onSetNewTowerHealth -= UpdateHealthBar;
        TowerLocation.onRepairedCurrentTowerHealth -= ResetHealth;
        TowerManager.onStopViewingTower -= ToggleTowerRange;
        Enemy.onAttack -= TakeDamage;
        GameManager.onSelfDestructTowers -= SelfDestruct;
        GameManager.onCollectCurrentActiveTowersTotalWarFundValue -= BroadcastTowerWarFundValue;

        _isAttacking = false;
        _towerRange.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        foreach (var muzzle in _muzzleFlash)
        {
            muzzle.SetActive(false);
        }

        _audioSource = GetComponent<AudioSource>(); //ssign the Audio Source to the reference variable
        _audioSource.playOnAwake = false; //disabling play on awake
        _audioSource.loop = true; //making sure our sound effect loops
        _audioSource.clip = _fireSound; //assign the clip to play
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ResetHealth()
    {
        Health = (this.gameObject.tag.Contains("Upgrade") ? UpgradeInitialHealth : InitialHealth); //Reset Health
        _health = Health;
        _healthPercent = 1;
        onHealthUpdate?.Invoke(this.gameObject, _healthPercent);
    }

    // Method to rotate gun barrel 
    void RotateBarrel()
    {
        foreach (var gunBarrel in _gunBarrel)
        {
            gunBarrel.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f);
        }
    }

    private void Shoot(GameObject attackingTower, GameObject currentTarget)
    {
        if (attackingTower == this.gameObject && currentTarget.tag.Contains("Mech"))
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                StartCoroutine(AttackRoutine(currentTarget));
            }

            RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel

            //for loop to iterate through all muzzle flash objects
            for (int i = 0; i < _muzzleFlash.Length; i++)
            {
                _muzzleFlash[i].SetActive(true); //enable muzzle effect particle effect
                _bulletCasings[i].Emit(1); //Emit the bullet casing particle effect   
            }

            if (_startWeaponNoise == true) //checking if we need to start the gun sound
            {
                _audioSource.Play(); //play audio clip attached to audio source
                _startWeaponNoise = false; //set the start weapon noise value to false to prevent calling it again
            }
        }
    }

    private void StopShooting(GameObject attackingTower)
    {
        if (attackingTower == this.gameObject)
        {
            //for loop to iterate through all muzzle flash objects
            for (int i = 0; i < _muzzleFlash.Length; i++)
            {
                _muzzleFlash[i].SetActive(false); //enable muzzle effect particle effect
            }
            _audioSource.Stop(); //stop the sound effect from playing
            _startWeaponNoise = true; //set the start weapon noise value to true
        }
    }

    private IEnumerator AttackRoutine(GameObject currentTarget)
    {
        onShoot?.Invoke(this.gameObject, currentTarget, _damageToDeal);
        yield return new WaitForSeconds(_fireDelay);

        _isAttacking = false;
    }

    /*----------ITower Functions----------*/
    public void ToggleTowerRange(GameObject currentlyViewedTower)
    {
        if (currentlyViewedTower == this.gameObject)
        {
            _towerRange.SetActive(TowerManager.Instance.IsViewingTower);
        }
        else
        {
            if (_towerRange.activeSelf == true)
            {
                _towerRange.SetActive(false);
            }
        }
    }

    public void TakeDamage(GameObject attackedObject, int damageAmount)
    {
        if (attackedObject == this.gameObject)
        {
            Health -= damageAmount;
            _health = Health;

            if (Health <= 0)
            {
                Health = 0;
                _health = Health;

                if (!_isDying)
                {
                    _isDying = true;
                    StartCoroutine(DieRoutine());
                }
            }

            _healthPercent = (float)Health / (this.gameObject.tag.Contains("Upgrade") ? (float)UpgradeInitialHealth : (float)InitialHealth);
            onHealthUpdate?.Invoke(this.gameObject, _healthPercent);
        }
    }

    public IEnumerator DieRoutine()
    {
        //TowerExplosion
        PlayExplosion();

        //Reset Tower
        yield return new WaitForSeconds(0.5f);

        //Notify TowerLocation that spot is now vacant
        onDeath?.Invoke(this.gameObject);
    }

    private void UpdateHealthBar(GameObject towerToUpdate)
    {
        if (this.gameObject == towerToUpdate)
        {
            _healthPercent = (float)Health / (this.gameObject.tag.Contains("Upgrade") ? (float)UpgradeInitialHealth : (float)InitialHealth);
            onHealthUpdate?.Invoke(this.gameObject, _healthPercent);
        }
    }

    public void SelfDestruct()
    {
        if (this.gameObject.activeSelf == true && !_isDying)
        {
            _isDying = true;
            StartCoroutine(DieRoutine());
        }
    }

    private void BroadcastTowerWarFundValue()
    {
        if (this.gameObject.activeSelf == true)
        {
            if (this.gameObject.tag.Contains("Upgrade"))
                onBroadcastTowerWarFundValue?.Invoke(UpgradeWarFundCost);
            else
                onBroadcastTowerWarFundValue?.Invoke(WarFundCost);
        }
    }
}

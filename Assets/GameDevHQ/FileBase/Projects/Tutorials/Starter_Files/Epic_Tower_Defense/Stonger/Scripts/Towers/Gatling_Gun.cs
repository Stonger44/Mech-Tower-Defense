using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))] //Require Audio Source component
public class Gatling_Gun : MonoBehaviour, ITower
{
    [SerializeField] private Transform[] _gunBarrel; //Reference to hold the gun barrel
    [SerializeField] private GameObject[] _muzzleFlash; //reference to the muzzle flash effect to play when firing
    [SerializeField] private ParticleSystem[] _bulletCasings; //reference to the bullet casing effect to play when firing
    [SerializeField] private AudioClip _fireSound; //Reference to the audio clip

    private AudioSource _audioSource; //reference to the audio source component
    private bool _startWeaponNoise = true;

    /*----------Extended Code----------*/
    [SerializeField] private int _health; //Only here so I can see it in the inspector
    public int Health { get; set; }
    public int DamageTaken { get; set; }

    public int InitialHealth { get; set; } = 10;
    public int WarFundCost { get; set; } = 500;
    public int WarFundSellValue { get; set; } = 250;

    public int UpgradeInitialHealth { get; set; } = 20;
    public int UpgradeWarFundCost { get; set; } = 1000;
    public int UpgradeWarFundSellValue { get; set; } = 500;

    [SerializeField] private GameObject _towerRange;

    private bool _isAttacking;
    [SerializeField] private int _damageDealt;

    public static event Action<GameObject, GameObject, int> onShoot;

    private void OnEnable()
    {
        Aim.onTargetInRange += Shoot;
        Aim.onNoTargetInRange += StopShooting;
        TowerLocation.onViewingCurrentTower += ToggleTowerRange;
        TowerManager.onStopViewingTower += ToggleTowerRange;

        if (this.gameObject.tag.Contains("Upgrade"))
        {
            Health = UpgradeInitialHealth;
        }
        else
        {
            Health = InitialHealth;
        }
        _health = Health;
    }

    private void OnDisable()
    {
        Aim.onTargetInRange -= Shoot;
        Aim.onNoTargetInRange -= StopShooting;
        TowerLocation.onViewingCurrentTower -= ToggleTowerRange;
        TowerManager.onStopViewingTower -= ToggleTowerRange;

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
        onShoot?.Invoke(this.gameObject, currentTarget, _damageDealt);
        yield return new WaitForSeconds(1);

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
}

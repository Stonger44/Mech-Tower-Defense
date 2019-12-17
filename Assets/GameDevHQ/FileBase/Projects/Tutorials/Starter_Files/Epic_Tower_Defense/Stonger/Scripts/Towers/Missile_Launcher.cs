using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Launcher : Explodable, ITower
{
    [SerializeField] private GameObject _missilePrefab; //holds the missle gameobject to clone
    [SerializeField] private GameObject[] _misslePositions; //array to hold the rocket positions on the turret
    [SerializeField] private float _fireDelay; //fire delay between rockets
    [SerializeField] private float _launchSpeed; //initial launch speed of the rocket
    [SerializeField] private float _power; //power to apply to the force of the rocket
    [SerializeField] private float _fuseDelay; //fuse delay before the rocket launches
    [SerializeField] private float _reloadTime; //time in between reloading the rockets
    [SerializeField] private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
    private bool _launched; //bool to check if we launched the rockets

    /*----------Extended Code----------*/
    [SerializeField] private int _health; //Only here so I can see it in the inspector
    private float _healthPercent;
    public int Health { get; set; }
    public int DamageTaken { get; set; }

    public int InitialHealth { get; set; } = 500;
    public int WarFundCost { get; set; } = 1000;
    public int WarFundSellValue { get; set; } = 500;

    public int UpgradeInitialHealth { get; set; } = 1000;
    public int UpgradeWarFundCost { get; set; } = 2000;
    public int UpgradeWarFundSellValue { get; set; } = 1000;

    [SerializeField] private GameObject _towerRange;

    [SerializeField] private int _warheadDamage;
    [SerializeField] private float _lockOnDelayTime;

    private bool _isDying;

    public static event Action<GameObject> onDeath;
    public static event Action<GameObject, float> onHealthUpdate;

    private void OnEnable()
    {
        Aim.onTargetInRange += FireMissiles;
        TowerLocation.onViewingCurrentTower += ToggleTowerRange;
        TowerLocation.onSetNewTowerHealth += UpdateHealthBar;
        TowerManager.onStopViewingTower += ToggleTowerRange;
        Enemy.onAttack += TakeDamage;

        if (this.gameObject.tag.Contains("Upgrade"))
            Health = UpgradeInitialHealth;
        else
            Health = InitialHealth;
        _health = Health;

        _isDying = false;
    }

    private void OnDisable()
    {
        Aim.onTargetInRange -= FireMissiles;
        TowerLocation.onViewingCurrentTower -= ToggleTowerRange;
        TowerLocation.onSetNewTowerHealth -= UpdateHealthBar;
        TowerManager.onStopViewingTower -= ToggleTowerRange;
        Enemy.onAttack -= TakeDamage;

        _launched = false;
        _towerRange.SetActive(false);
    }

    private void Update()
    {

    }

    IEnumerator FireRocketsRoutine(GameObject currentTarget)
    {
        yield return new WaitForSeconds(_lockOnDelayTime);

        for (int i = 0; i < _misslePositions.Length; i++) //for loop to iterate through each missle position
        {
            //Creat Missle Pool system
            GameObject rocket = PoolManager.Instance.RequestMissile();
            //GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

            rocket.transform.parent = _misslePositions[i].transform; //set the rockets parent to the missle launch position 
            rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
            rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
            rocket.transform.parent = PoolManager.Instance.missileContainer.transform;

            rocket.GetComponent<Missile>().AssignMissleRules(_launchSpeed, _power, _fuseDelay, _destroyTime, this.gameObject, currentTarget, _warheadDamage); //assign missle properties 

            _misslePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired
            yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
        }

        for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
        {
            yield return new WaitForSeconds(_reloadTime); //wait for reload time
            _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
        }

        _launched = false; //set launch bool to false
    }

    private void FireMissiles(GameObject currentTower, GameObject currentTarget)
    {
        if (currentTower == this.gameObject && currentTarget.tag == "Mech2" && _launched == false) //check if we launched the rockets
        {
            _launched = true; //set the launch bool to true
            StartCoroutine(FireRocketsRoutine(currentTarget)); //start a coroutine that fires the rockets. 
        }
    }

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
}

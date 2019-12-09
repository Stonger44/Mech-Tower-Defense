using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Launcher : MonoBehaviour, ITower
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

    //Extended Code
    public int WarFundCost { get; set; } = 1000;
    public int WarFundSellValue { get; set; } = 500;

    public int UpgradeWarFundCost { get; set; } = 2000;
    public int UpgradeWarFundSellValue { get; set; } = 1000;

    public bool IsActive { get; set; } = false;

    [SerializeField] private GameObject _towerRange;

    [SerializeField] private int _damageAmount;
    [SerializeField] private float _lockOnDelayTime;

    private void OnEnable()
    {
        Aim.onTargetInRange += FireMissiles;
        TowerLocation.onViewingCurrentTower += ToggleTowerRange;
        TowerManager.onStopViewingTower += ToggleTowerRange;
        TowerLocation.onUpgradedCurrentTower += ToggleTowerRange;
    }

    private void OnDisable()
    {
        Aim.onTargetInRange -= FireMissiles;
        TowerLocation.onViewingCurrentTower -= ToggleTowerRange;
        TowerManager.onStopViewingTower -= ToggleTowerRange;
        TowerLocation.onUpgradedCurrentTower -= ToggleTowerRange;

        _launched = false;
        _towerRange.SetActive(false);
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

            rocket.GetComponent<Missile>().AssignMissleRules(_launchSpeed, _power, _fuseDelay, _destroyTime, currentTarget, _damageAmount); //assign missle properties 

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
        if (currentTower == this.gameObject && _launched == false) //check if we launched the rockets
        {
            _launched = true; //set the launch bool to true
            StartCoroutine(FireRocketsRoutine(currentTarget)); //start a coroutine that fires the rockets. 
        }
    }
}

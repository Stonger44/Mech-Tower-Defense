using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLocation : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    [SerializeField] private GameObject _vacantParticleEffect;
    [SerializeField] private GameObject _currentPlacedTower;
    private ITower _currentPlacedTowerInterface;

    public static event Action<Vector3> onVacantLocationMouseOver_Vector3;
    public static event Action onOccupiedLocationMouseOver;

    public static event Action<bool> onLocationMouseOver;
    public static event Action onLocationMouseExit;
    
    public static event Action onPlaceTower;
    public static event Action<int> onPurchaseTower;
    public static event Action onInsufficientWarFunds;

    public static event Action<GameObject> onViewingCurrentTower;
    public static event Action<int> onDismantledCurrentTower;

    private void OnEnable()
    {
        TowerManager.onBrowsingTowerLocations += ToggleVacantParticleEffect;
        TowerManager.onDismantleTower += DismantleTower;
    }

    private void OnDisable()
    {
        TowerManager.onBrowsingTowerLocations -= ToggleVacantParticleEffect;
        TowerManager.onDismantleTower -= DismantleTower;
    }

    private void DismantleTower(GameObject towerToBeDismantled)
    {
        if (towerToBeDismantled == _currentPlacedTower)
        {
            //Dismantle Tower:
            PoolManager.Instance.ResetTower(_currentPlacedTower);

            //For Game manager to update WarFunds
            if (_currentPlacedTower.tag.Contains("Upgrade"))
                onDismantledCurrentTower?.Invoke(_currentPlacedTowerInterface.UpgradeWarFundSellValue);
            else
                onDismantledCurrentTower?.Invoke(_currentPlacedTowerInterface.WarFundSellValue);

            _isOccupied = false;
            _currentPlacedTower = null;
            _currentPlacedTowerInterface = null;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _vacantParticleEffect.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (TowerManager.Instance.IsPlacingTower)
        {
            //Move Tower image accordingly
            if (!_isOccupied)
                onVacantLocationMouseOver_Vector3?.Invoke(this.transform.position);
            else
                onOccupiedLocationMouseOver?.Invoke();

            //Check if enough WarFunds are available (basically in order to change the tower range color appropriately)
            ITower currentTower = TowerManager.Instance.CurrentTower.GetComponent<ITower>();
            bool hasSufficientWarFunds = false;
            if (currentTower != null)
                hasSufficientWarFunds = GameManager.Instance.totalWarFunds >= currentTower.WarFundCost;
            onLocationMouseOver?.Invoke(hasSufficientWarFunds && !_isOccupied);
        }
    }

    private void OnMouseExit()
    {
        onLocationMouseExit?.Invoke();
    }

    private void OnMouseUp()
    {
        if (TowerManager.Instance.IsPlacingTower == true && _isOccupied == false)
        {
            PlaceTower();
        }
    }

    private void OnMouseDown()
    {
        if (_isOccupied == true && _currentPlacedTower != null)
        {
            onViewingCurrentTower?.Invoke(_currentPlacedTower);
        }
    }

    private void ToggleVacantParticleEffect(bool isPlacingTower)
    {
        if (isPlacingTower && !_isOccupied)
            _vacantParticleEffect.SetActive(true);
        else
            _vacantParticleEffect.SetActive(false);
    }

    private void PlaceTower()
    {
        //Check 1: Is a tower selected?
        ITower currentTower = TowerManager.Instance.CurrentTower.GetComponent<ITower>();
        if (currentTower == null)
        {
            Debug.Log("No Tower Selected!");
            return;
        }

        //Check 2: Is there enough WarFund available?
        if (currentTower.WarFundCost > GameManager.Instance.totalWarFunds)
        {
            Debug.Log("Insufficient WarFunds. Cost: " + currentTower.WarFundCost + ". Total War Funds: " + GameManager.Instance.totalWarFunds + ".");
            onInsufficientWarFunds?.Invoke();
            return;
        }

        //Check 3: Make sure the Tower Image (mouse pointer) is still in position
        if (this.transform.position != TowerManager.Instance.CurrentTowerImage.transform.position)
        {
            Debug.Log("Tower not in Position");
            return;
        }
        

        //PASS: If we get here, we passed all the pre-requisites and may place a tower.
        _isOccupied = true;
        ToggleVacantParticleEffect(false);

        _currentPlacedTower = PoolManager.Instance.RequestTower(TowerManager.Instance.CurrentTower);
        _currentPlacedTower.transform.position = this.transform.position;
        _currentPlacedTower.SetActive(true);

        _currentPlacedTowerInterface = _currentPlacedTower.GetComponent<ITower>();
        if (_currentPlacedTowerInterface == null)
            Debug.LogError("_currentPlacedTowerInterface is NULL.");

        onPlaceTower?.Invoke();
        onPurchaseTower?.Invoke(currentTower.WarFundCost);
    }
}

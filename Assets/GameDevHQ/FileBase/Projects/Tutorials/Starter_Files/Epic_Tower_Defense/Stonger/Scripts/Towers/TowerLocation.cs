using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLocation : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    [SerializeField] private GameObject _vacantParticleEffect;
    [SerializeField] private GameObject _currentPlacedTower;

    public static event Action<Vector3> onVacantLocationMouseOver_Vector3;
    public static event Action<bool> onVacantLocationMouseOver;
    public static event Action onVacantLocationMouseExit;
    public static event Action onOccupiedLocationMouseOver;
    public static event Action onPlaceTower;
    public static event Action<int> onPurchaseTower;
    public static event Action onInsufficientWarFunds;

    private void OnEnable()
    {
        TowerManager.onBrowsingTowerLocations += ToggleVacantParticleEffect;
    }

    private void OnDisable()
    {
        TowerManager.onBrowsingTowerLocations -= ToggleVacantParticleEffect;
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
            if (!_isOccupied)
                onVacantLocationMouseOver_Vector3?.Invoke(this.transform.position);
            else
                onOccupiedLocationMouseOver?.Invoke();

            ITower currentTower = TowerManager.Instance.CurrentTower.GetComponent<ITower>();
            bool hasSufficientWarFund = false;
            if (currentTower != null)
                hasSufficientWarFund = GameManager.Instance.totalWarFund >= currentTower.WarFundCost;

            onVacantLocationMouseOver?.Invoke(hasSufficientWarFund && !_isOccupied);
        }
    }

    private void OnMouseExit()
    {
        onVacantLocationMouseExit?.Invoke();
    }

    private void OnMouseDown()
    {
        PlaceTower();
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
        if (TowerManager.Instance.IsPlacingTower == false)
        {
            Debug.Log("Not in placing tower mode");
            return;
        }

        if (_isOccupied == true)
        {
            Debug.Log("TowerLocation is occupied with " + _currentPlacedTower.name + ".");
            return;
        }

        ITower currentTower = TowerManager.Instance.CurrentTower.GetComponent<ITower>();
        if (currentTower == null)
        {
            Debug.Log("No Tower Selected!");
            return;
        }

        if (currentTower.WarFundCost > GameManager.Instance.totalWarFund)
        {
            Debug.Log("Insufficient WarFunds. Cost: " + currentTower.WarFundCost + ". Total War Funds: " + GameManager.Instance.totalWarFund + ".");
            onInsufficientWarFunds?.Invoke();
            return;
        }

        //If we get here, we passed all the pre-requisites and may place a tower.
        _isOccupied = true;
        ToggleVacantParticleEffect(false);
        _currentPlacedTower = Instantiate(TowerManager.Instance.CurrentTower, this.transform.position, Quaternion.Euler(0, 90, 0));

        onPlaceTower?.Invoke();
        onPurchaseTower?.Invoke(currentTower.WarFundCost);
    }
}

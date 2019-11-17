using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    [SerializeField] private GameObject _towerRangeGreen;
    [SerializeField] private GameObject _towerRangeRed;

    private void OnEnable()
    {
        TowerManager.onBrowsingTowerLocations += ToggleTowerRangeColor;
        TowerLocation.onVacantLocationMouseOver += ShowTowerRange_Green;
        TowerLocation.onVacantLocationMouseExit += ShowTowerRange_Red;
    }

    private void OnDisable()
    {
        TowerManager.onBrowsingTowerLocations -= ToggleTowerRangeColor;
        TowerLocation.onVacantLocationMouseOver -= ShowTowerRange_Green;
        TowerLocation.onVacantLocationMouseExit -= ShowTowerRange_Red;
    }

    private void ToggleTowerRangeColor(bool isPlacingTower)
    {
        if (isPlacingTower)
        {
            ShowTowerRange_Red();
        }
        else
        {
            HideTowerRange();
        }

    }

    private void HideTowerRange()
    {
        _towerRangeGreen.SetActive(false);
        _towerRangeRed.SetActive(false);
    }

    private void ShowTowerRange_Green()
    {
        _towerRangeGreen.SetActive(true);
        _towerRangeRed.SetActive(false);
    }

    private void ShowTowerRange_Red()
    {
        _towerRangeRed.SetActive(true);
        _towerRangeGreen.SetActive(false);
    }
}

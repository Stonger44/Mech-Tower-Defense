using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    [SerializeField] private GameObject _towerRangeGreen;
    [SerializeField] private GameObject _towerRangeRed;

    private void OnEnable()
    {
        TowerBrowsing.onBrowsingTowerLocations += ToggleTowerRangeColor;
    }

    private void OnDisable()
    {
        TowerBrowsing.onBrowsingTowerLocations -= ToggleTowerRangeColor;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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

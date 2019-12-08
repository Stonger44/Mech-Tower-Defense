using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoSingleton<UI_Manager>
{
    [SerializeField] private Text _warFunds;

    [SerializeField] private GameObject _UI_GatlingGun;
    [SerializeField] private GameObject _UI_GatlingGun_Disabled;
    [SerializeField] private GameObject _UI_MissileLauncher;
    [SerializeField] private GameObject _UI_MissileLauncher_Disabled;

    [SerializeField] private GameObject _UI_DismantleTower;
    [SerializeField] private GameObject _UI_UpgradeGatlingGun;
    [SerializeField] private GameObject _UI_UpgradeGatlingGun_Disabled;
    [SerializeField] private GameObject _UI_UpgradeMissileLauncher;
    [SerializeField] private GameObject _UI_UpgradeMissileLauncher_Disabled;

    private ITower _currentTowerInterface;

    private void OnEnable()
    {
        TowerLocation.onViewingCurrentTower += ShowCurrentTowerOptions;
        TowerManager.onStopViewingTowerUI += ResetArmoryToDefaultState;
    }

    private void OnDisable()
    {
        TowerLocation.onViewingCurrentTower -= ShowCurrentTowerOptions;
        TowerManager.onStopViewingTowerUI -= ResetArmoryToDefaultState;
    }

    private void ShowCurrentTowerOptions(GameObject currentlyViewedTower)
    {
        _currentTowerInterface = currentlyViewedTower.GetComponent<ITower>();
        if (_currentTowerInterface == null)
            Debug.LogError("_currentTowerInterface is NULL.");

        HideAllTowerPlacementUI();

        _UI_DismantleTower.SetActive(true);
        //Set WarFunds amount on UI

        switch (currentlyViewedTower.tag)
        {
            case "Gatling_Gun":
                if (GameManager.Instance.totalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
                {
                    _UI_UpgradeGatlingGun.SetActive(true);
                }
                else
                {
                    _UI_UpgradeGatlingGun_Disabled.SetActive(true);
                }

                break;
            case "Missile_Launcher":
                if (GameManager.Instance.totalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
                {
                    _UI_UpgradeMissileLauncher.SetActive(true);
                }
                else
                {
                    _UI_UpgradeMissileLauncher_Disabled.SetActive(true);
                }

                break;
            default:
                Debug.LogError("currentlyViewedTower.tag not recognized.");
                break;
        }
    }

    private void ResetArmoryToDefaultState()
    {
        HideAllTowerOptionsUI();
        UpdateTowerPlacementUI(GameManager.Instance.totalWarFunds);
    }

    private void HideAllTowerOptionsUI()
    {
        _UI_DismantleTower.SetActive(false);
        _UI_UpgradeGatlingGun.SetActive(false);
        _UI_UpgradeGatlingGun_Disabled.SetActive(false);
        _UI_UpgradeMissileLauncher.SetActive(false);
        _UI_UpgradeMissileLauncher_Disabled.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateWarFundsText(int warFunds)
    {
        _warFunds.text = warFunds.ToString();
        UpdateTowerPlacementUI(warFunds);
    }

    public void UpdateTowerPlacementUI(int warFunds)
    {
        if (TowerManager.Instance.IsViewingTower == false)
        {
            if (warFunds >= 1000)
            {
                //Enabled Images
                _UI_GatlingGun.SetActive(true);
                _UI_MissileLauncher.SetActive(true);

                //Disabled Images
                _UI_GatlingGun_Disabled.SetActive(false);
                _UI_MissileLauncher_Disabled.SetActive(false);
            }
            else if (warFunds >= 500)
            {
                //Enabled Images
                _UI_GatlingGun.SetActive(true);
                _UI_MissileLauncher.SetActive(false);

                //Disabled Images
                _UI_GatlingGun_Disabled.SetActive(false);
                _UI_MissileLauncher_Disabled.SetActive(true);
            }
            else if (warFunds < 500)
            {
                //Enabled Images
                _UI_GatlingGun.SetActive(false);
                _UI_MissileLauncher.SetActive(false);

                //Disabled Images
                _UI_GatlingGun_Disabled.SetActive(true);
                _UI_MissileLauncher_Disabled.SetActive(true);
            } 
        }
    }

    private void HideAllTowerPlacementUI()
    {
        //Enabled Images
        _UI_GatlingGun.SetActive(false);
        _UI_MissileLauncher.SetActive(false);

        //Disabled Images
        _UI_GatlingGun_Disabled.SetActive(false);
        _UI_MissileLauncher_Disabled.SetActive(false);
    }
}

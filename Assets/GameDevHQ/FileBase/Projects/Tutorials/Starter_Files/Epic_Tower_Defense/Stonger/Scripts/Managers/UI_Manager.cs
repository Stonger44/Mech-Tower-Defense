using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoSingleton<UI_Manager>
{
    [SerializeField] private GameObject _levelStatus;
    [SerializeField] private Text _status;

    [SerializeField] private Text _waveCount;
    [SerializeField] private Text _enemyCount;

    [SerializeField] private Text _warFunds;
    [SerializeField] private Text _dismantledWarFundsRecieved;

    private float _healthPercent;
    private Dictionary<Image, Sprite[]> _uiPanelDictionary = new Dictionary<Image, Sprite[]>();

    [SerializeField] private Image _armoryPanel;
    [SerializeField] private Sprite[] _armoryPanelArray = new Sprite[3];
    [SerializeField] private Image _warFundsPanel;
    [SerializeField] private Sprite[] _warFundsPanelArray = new Sprite[3];
    [SerializeField] private Image _playBackSpeedPanel;
    [SerializeField] private Sprite[] _playBackSpeedPanelArray = new Sprite[3];
    [SerializeField] private Image _restartPanel;
    [SerializeField] private Sprite[] __restartPanelArray = new Sprite[3];
    [SerializeField] private Image _waveEnemiesPanel;
    [SerializeField] private Sprite[] _waveEnemiesPanelArray = new Sprite[3];
    [SerializeField] private Image _levelStatusPanel;
    [SerializeField] private Sprite[] _levelStatusPanelArray = new Sprite[3];

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
        GameManager.onGainedWarFundsFromEnemyDeath += ShowCurrentTowerOptions;
        GameManager.onWaveUpdate += UpdateWaveCount;
        GameManager.onEnemyCountUpdate += UpdateEnemyCount;
        GameManager.onHealthUpdateUI += UpdateHealthUI;
        GameManager.onUpdateLevelStatusCountDown += UpdateLevelStatusCountDown;
    }

    private void OnDisable()
    {
        TowerLocation.onViewingCurrentTower -= ShowCurrentTowerOptions;
        TowerManager.onStopViewingTowerUI -= ResetArmoryToDefaultState;
        GameManager.onGainedWarFundsFromEnemyDeath -= ShowCurrentTowerOptions;
        GameManager.onWaveUpdate -= UpdateWaveCount;
        GameManager.onEnemyCountUpdate -= UpdateEnemyCount;
        GameManager.onHealthUpdateUI -= UpdateHealthUI;
        GameManager.onUpdateLevelStatusCountDown -= UpdateLevelStatusCountDown;
    }

    // Start is called before the first frame update
    void Start()
    {
        _uiPanelDictionary.Add(_armoryPanel, _armoryPanelArray);
        _uiPanelDictionary.Add(_warFundsPanel, _warFundsPanelArray);
        _uiPanelDictionary.Add(_playBackSpeedPanel, _playBackSpeedPanelArray);
        _uiPanelDictionary.Add(_restartPanel, __restartPanelArray);
        _uiPanelDictionary.Add(_waveEnemiesPanel, _waveEnemiesPanelArray);
        _uiPanelDictionary.Add(_levelStatusPanel, _levelStatusPanelArray);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateLevelStatusCountDown(int countDownTime)
    {
        switch (countDownTime)
        {
            case 3:
            case 2:
            case 1:
                if (_levelStatus.activeSelf == false) //Show for countDown
                    _levelStatus.SetActive(true);

                _status.text = countDownTime.ToString();

                break;
            case 0:
                if (_levelStatus.activeSelf == false) //Show for countDown
                    _levelStatus.SetActive(true);

                _status.text = "WAVE STARTED";

                break;
            case -1:
                _levelStatus.SetActive(false);
                _status.text = "";

                break;
            default:
                Debug.LogError("countDownTime value not recognized.");

                break;
        }
    }

    private void UpdateLevelStatus()
    {

    }

    private void UpdateHealthUI(int currentHealth, int initialHealth)
    {
        _healthPercent = (float)currentHealth / (float)initialHealth;

        foreach (var panel in _uiPanelDictionary)
        {
            if (_healthPercent <= GameManager.Instance.HealthWarningThreshold)
            {
                panel.Key.sprite = panel.Value[2];
            }
            else if (_healthPercent <= GameManager.Instance.HealthCautionThreshold)
            {
                panel.Key.sprite = panel.Value[1];
            }
            else
            {
                panel.Key.sprite = panel.Value[0];
            }
        }
    }

    private void UpdateWaveCount(int currentWaveCount, int finalWaveCount)
    {
        _waveCount.text = currentWaveCount + " / " + finalWaveCount;
    }

    private void UpdateEnemyCount(int currentEnemyCount, int totalEnemyCount)
    {
        _enemyCount.text = currentEnemyCount + " / " + totalEnemyCount;
    }

    /*----------Tower UI----------*/
    private void ShowCurrentTowerOptions(GameObject currentlyViewedTower)
    {
        _currentTowerInterface = currentlyViewedTower.GetComponent<ITower>();
        if (_currentTowerInterface == null)
            Debug.LogError("_currentTowerInterface is NULL.");

        HideAllTowerPlacementUI();
        HideAllTowerOptionsUI();

        switch (currentlyViewedTower.tag)
        {
            case "Tower_Gatling_Gun":

                if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
                    _UI_UpgradeGatlingGun.SetActive(true);
                else
                    _UI_UpgradeGatlingGun_Disabled.SetActive(true);

                _dismantledWarFundsRecieved.text = _currentTowerInterface.WarFundSellValue.ToString();

                break;
            case "Tower_Missile_Launcher":

                if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
                    _UI_UpgradeMissileLauncher.SetActive(true);
                else
                    _UI_UpgradeMissileLauncher_Disabled.SetActive(true);

                _dismantledWarFundsRecieved.text = _currentTowerInterface.WarFundSellValue.ToString();

                break;
            case "Tower_Missile_Launcher_Upgrade":
            case "Tower_Gatling_Gun_Upgrade":

                _dismantledWarFundsRecieved.text = _currentTowerInterface.UpgradeWarFundSellValue.ToString();

                break;
            default:
                Debug.LogError("currentlyViewedTower.tag not recognized.");
                break;
        }

        _UI_DismantleTower.SetActive(true);
    }

    private void ResetArmoryToDefaultState()
    {
        HideAllTowerOptionsUI();
        UpdateTowerPlacementUI(GameManager.Instance.TotalWarFunds);
    }

    private void HideAllTowerOptionsUI()
    {
        _UI_DismantleTower.SetActive(false);
        _UI_UpgradeGatlingGun.SetActive(false);
        _UI_UpgradeGatlingGun_Disabled.SetActive(false);
        _UI_UpgradeMissileLauncher.SetActive(false);
        _UI_UpgradeMissileLauncher_Disabled.SetActive(false);
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
    /*----------Tower UI----------*/
}

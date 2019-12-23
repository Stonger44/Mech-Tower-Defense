using System;
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

    private float _healthPercent;
    private Sprite _healthSprite;
    [SerializeField] private bool _isHealthUpdateRoutineRunning;
    [SerializeField] private bool _isLevelStatusNextWaveRoutineRunning;
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

    //Place New Towers
    [SerializeField] private GameObject _UI_GatlingGun;
    [SerializeField] private GameObject _UI_GatlingGun_Disabled;
    [SerializeField] private GameObject _UI_MissileLauncher;
    [SerializeField] private GameObject _UI_MissileLauncher_Disabled;

    //Tower Options
    [SerializeField] private TowerOptions _dismantleTowerOption;
    [SerializeField] private TowerOptions _repairTowerOption;
    [SerializeField] private TowerOptions _repairTowerOption_NA;
    [SerializeField] private TowerOptions _repairTowerOption_Disabled;
    [SerializeField] private TowerOptions _upgradeTowerOption;
    [SerializeField] private TowerOptions _upgradeTowerOption_Disabled;
    [SerializeField] private List<TowerOptions> _towerOptionList = new List<TowerOptions>();

    private ITower _currentTowerInterface;

    [SerializeField] private GameObject _pauseActive;
    [SerializeField] private GameObject _playActive;
    [SerializeField] private GameObject _ffActive;

    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _ffButton;

    public static event Action onResetEnemiesFotNextWave;

    private void OnEnable()
    {
        TowerLocation.onViewingCurrentTower += ShowCurrentTowerOptions;
        TowerManager.onStopViewingTowerUI += ResetArmoryToDefaultState;
        GameManager.onGainedWarFundsFromEnemyDeath += ShowCurrentTowerOptions;
        GameManager.onWaveUpdate += UpdateWaveCount;
        GameManager.onEnemyCountUpdate += UpdateEnemyCount;
        GameManager.onTakeDamage += DamageHealthUI;
        GameManager.onWaveFailed += WaveFailedhealthUI;
        GameManager.onUpdateLevelStatusCountDown += UpdateLevelStatusCountDown;
        GameManager.onUpdateLevelStatus += UpdateLevelStatus;
    }

    private void OnDisable()
    {
        TowerLocation.onViewingCurrentTower -= ShowCurrentTowerOptions;
        TowerManager.onStopViewingTowerUI -= ResetArmoryToDefaultState;
        GameManager.onGainedWarFundsFromEnemyDeath -= ShowCurrentTowerOptions;
        GameManager.onWaveUpdate -= UpdateWaveCount;
        GameManager.onEnemyCountUpdate -= UpdateEnemyCount;
        GameManager.onTakeDamage -= DamageHealthUI;
        GameManager.onWaveFailed -= WaveFailedhealthUI;
        GameManager.onUpdateLevelStatusCountDown -= UpdateLevelStatusCountDown;
        GameManager.onUpdateLevelStatus -= UpdateLevelStatus;
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

        _towerOptionList.Add(_dismantleTowerOption);
        _towerOptionList.Add(_repairTowerOption);
        _towerOptionList.Add(_repairTowerOption_NA);
        _towerOptionList.Add(_repairTowerOption_Disabled);
        _towerOptionList.Add(_upgradeTowerOption);
        _towerOptionList.Add(_upgradeTowerOption_Disabled);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateLevelStatusCountDown(int countDownTime)
    {
        switch (countDownTime)
        {
            case 4:
                if (_isHealthUpdateRoutineRunning == false)
                {
                    _isHealthUpdateRoutineRunning = true;
                    StartCoroutine(HealthUpdateRoutine(GameManager.Instance.WaveSuccess == false));
                    onResetEnemiesFotNextWave?.Invoke();
                }
                break;
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

                _status.text = "BEGIN!";
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
        _levelStatus.SetActive(true);

        if (GameManager.Instance.WaveRunning == false)
        {
            _pauseActive.SetActive(true);
            _playActive.SetActive(false);
            _ffActive.SetActive(false);

            if (GameManager.Instance.WaveSuccess == true)
            {
                _status.text = "WAVE  " + (GameManager.Instance.Wave - 1) + " COMPLETE!";
            }
            else if (GameManager.Instance.WaveSuccess == false)
            {
                _status.text = "WAVE  " + GameManager.Instance.Wave + " FAILED";
            }

            if (_isLevelStatusNextWaveRoutineRunning == false)
            {
                _isLevelStatusNextWaveRoutineRunning = true;
                _playButton.SetActive(false);
                _ffButton.SetActive(false);
                StartCoroutine(LevelStatus_NextWaveRoutine());
            }
        }
    }

    private IEnumerator LevelStatus_NextWaveRoutine()
    {
        yield return new WaitForSeconds(2);

        if (GameManager.Instance.WaveSuccess == false)
        {
            _status.text = "RETRY WAVE  " + GameManager.Instance.Wave;
        }
        else if (GameManager.Instance.WaveSuccess == true)
        {
            if (GameManager.Instance.Wave == (GameManager.Instance.FinalWave + 1))
            {
                _status.text = "LEVEL COMPLETE";
            }
            else if (GameManager.Instance.Wave == GameManager.Instance.FinalWave)
            {
                _status.text = "FINAL WAVE";
            }
            else
            {
                _status.text = "WAVE  " + GameManager.Instance.Wave + "  INCOMING";
            }
        }


        _playButton.SetActive(true);
        _ffButton.SetActive(true);
        _isLevelStatusNextWaveRoutineRunning = false;
    }

    private void WaveFailedhealthUI(int currentHealth)
    {
        if (currentHealth <= 0)
        {
            StartCoroutine(HealthUpdateRoutine());
        }
    }



    private void DamageHealthUI(int currentHealth, int initialHealth)
    {
        if (GameManager.Instance.WaveRunning == true && _isHealthUpdateRoutineRunning == false)
        {
            _healthPercent = (float)currentHealth / (float)initialHealth;

            _isHealthUpdateRoutineRunning = true;
            StartCoroutine(HealthUpdateRoutine());
        }
    }

    private IEnumerator HealthUpdateRoutine(bool isRebooting = false)
    {

        if (GameManager.Instance.WaveRunning == false && isRebooting == false)
            yield return null;


        if (GameManager.Instance.WaveRunning == true || isRebooting == true)
        {
            for (int i = 1; i <= 4; i++)
            {
                switch (i)
                {
                    case 1:
                    case 3:
                        //Change to damage color
                        foreach (var panel in _uiPanelDictionary)
                        {
                            if ((_healthPercent <= GameManager.Instance.HealthWarningThreshold) || isRebooting == true)
                            {
                                panel.Key.sprite = panel.Value[2];
                            }
                            else
                            {
                                panel.Key.sprite = panel.Value[1];
                            }
                        }
                        break;
                    case 2:
                    case 4:
                        //Change to default color
                        foreach (var panel in _uiPanelDictionary)
                        {
                            panel.Key.sprite = panel.Value[0];
                        }
                        break;
                    default:
                        break;
                }

                if (i < 2)
                    yield return new WaitForSeconds(0.15f);
                else
                    yield return new WaitForSeconds(0.1f);
            }

            //If Wave Failed
            if (GameManager.Instance.WaveRunning == false && GameManager.Instance.WaveSuccess == false && isRebooting == false)
            {
                foreach (var panel in _uiPanelDictionary)
                {
                    panel.Key.sprite = panel.Value[2]; //Change UI color to red
                }
            } 
        }

        _isHealthUpdateRoutineRunning = false;
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

        #region Old Code
        //switch (currentlyViewedTower.tag)
        //{
        //    case "Tower_Gatling_Gun":

        //        if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
        //            _UI_UpgradeGatlingGun.SetActive(true);
        //        else
        //            _UI_UpgradeGatlingGun_Disabled.SetActive(true);

        //        _dismantleTowerWarFundsRecieved.text = _currentTowerInterface.WarFundSellValue.ToString();

        //        break;
        //    case "Tower_Missile_Launcher":

        //        if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
        //            _UI_UpgradeMissileLauncher.SetActive(true);
        //        else
        //            _UI_UpgradeMissileLauncher_Disabled.SetActive(true);

        //        _dismantleTowerWarFundsRecieved.text = _currentTowerInterface.WarFundSellValue.ToString();

        //        break;
        //    case "Tower_Missile_Launcher_Upgrade":
        //    case "Tower_Gatling_Gun_Upgrade":

        //        _dismantleTowerWarFundsRecieved.text = _currentTowerInterface.UpgradeWarFundSellValue.ToString();

        //        break;
        //    default:
        //        Debug.LogError("currentlyViewedTower.tag not recognized.");
        //        break;
        //}
        #endregion

        SetAllTowerOptionTextAndImages();

        ShowAvailableTowerOptions();
    }

    private void SetAllTowerOptionTextAndImages()
    {
        foreach (var towerOption in _towerOptionList)
        {
            if (_currentTowerInterface.Name.ToUpper().Contains("UPGRADE"))
            {
                if (towerOption.UIObject.name.ToUpper().Contains("DISMANTLE"))
                {
                    towerOption.WarFunds.text = _currentTowerInterface.UpgradeWarFundSellValue.ToString(); //Update WarFund
                }
                else if (towerOption.UIObject.name.ToUpper().Contains("REPAIR"))
                {
                    towerOption.WarFunds.text = _currentTowerInterface.UpgradeWarFundRepairCost.ToString();  //Update WarFund
                }
                towerOption.Image.sprite = _currentTowerInterface.TowerSprites.UpgradeTowerSprite; //Update Image
            }
            else //Not Upgraded Towers
            {
                if (towerOption.UIObject.name.ToUpper().Contains("DISMANTLE"))
                {
                    towerOption.WarFunds.text = _currentTowerInterface.WarFundSellValue.ToString(); //Update WarFund
                    towerOption.Image.sprite = _currentTowerInterface.TowerSprites.TowerSprite; //Update Image
                }
                else if (towerOption.UIObject.name.ToUpper().Contains("REPAIR"))
                {
                    towerOption.WarFunds.text = _currentTowerInterface.WarFundRepairCost.ToString();  //Update WarFund
                    towerOption.Image.sprite = _currentTowerInterface.TowerSprites.TowerSprite; //Update Image
                }
                else if (towerOption.UIObject.name.ToUpper().Contains("UPGRADE"))
                {
                    towerOption.WarFunds.text = _currentTowerInterface.UpgradeWarFundCost.ToString();  //Update WarFund
                    towerOption.Image.sprite = _currentTowerInterface.TowerSprites.UpgradeTowerSprite; //Update Image
                }
            }
        }
    }

    private void ShowAvailableTowerOptions()
    {
        _dismantleTowerOption.UIObject.SetActive(true);

        if (_currentTowerInterface.Name.ToUpper().Contains("UPGRADE"))
        {
            //Show Appropriate Repair Tower Option
            if (_currentTowerInterface.Health == _currentTowerInterface.UpgradeInitialHealth) //Tower is at full health
            {
                _repairTowerOption_NA.UIObject.SetActive(true);
            }
            else if (GameManager.Instance.TotalWarFunds < _currentTowerInterface.UpgradeWarFundRepairCost)
            {
                _repairTowerOption_Disabled.UIObject.SetActive(true);
            }
            else if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.UpgradeWarFundRepairCost)
            {
                _repairTowerOption.UIObject.SetActive(true);
            }
        }
        else
        {
            //Show Appropriate Repair Tower Option
            if (_currentTowerInterface.Health == _currentTowerInterface.InitialHealth) //Tower is at full health
            {
                _repairTowerOption_NA.UIObject.SetActive(true);
            }
            else if (GameManager.Instance.TotalWarFunds < _currentTowerInterface.WarFundRepairCost)
            {
                _repairTowerOption_Disabled.UIObject.SetActive(true);
            }
            else if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.WarFundRepairCost)
            {
                _repairTowerOption.UIObject.SetActive(true);
            }

            //Show Appropriate Upgrade Tower Option
            if (GameManager.Instance.TotalWarFunds >= _currentTowerInterface.UpgradeWarFundCost)
            {
                _upgradeTowerOption.UIObject.SetActive(true);
            }
            else
            {
                _upgradeTowerOption_Disabled.UIObject.SetActive(true);
            }
        }
    }

    private void ResetArmoryToDefaultState()
    {
        HideAllTowerOptionsUI();
        UpdateTowerPlacementUI(GameManager.Instance.TotalWarFunds);
    }

    private void HideAllTowerOptionsUI()
    {
        _dismantleTowerOption.UIObject.SetActive(false);
        _repairTowerOption.UIObject.SetActive(false);
        _repairTowerOption_NA.UIObject.SetActive(false);
        _repairTowerOption_Disabled.UIObject.SetActive(false);
        _upgradeTowerOption.UIObject.SetActive(false);
        _upgradeTowerOption_Disabled.UIObject.SetActive(false);
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
            if (warFunds >= 1500)
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

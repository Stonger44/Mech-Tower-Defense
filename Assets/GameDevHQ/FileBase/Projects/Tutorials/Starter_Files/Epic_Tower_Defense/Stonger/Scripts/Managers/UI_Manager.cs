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

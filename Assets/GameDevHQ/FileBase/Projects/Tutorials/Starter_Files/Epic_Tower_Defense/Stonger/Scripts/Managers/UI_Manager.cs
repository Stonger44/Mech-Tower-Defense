using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoSingleton<UI_Manager>
{
    [SerializeField] private Text _warFunds;

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
    }
}

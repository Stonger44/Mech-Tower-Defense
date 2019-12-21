using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    string Name { get; set; }

    int Health { get; set; }
    int DamageTaken { get; set; }

    int InitialHealth { get; set; }
    int WarFundCost { get; set; }
    int WarFundSellValue { get; set; }

    int UpgradeInitialHealth { get; set; }
    int UpgradeWarFundCost { get; set; }
    int UpgradeWarFundSellValue { get; set; }

    void ToggleTowerRange(GameObject currentlyViewedTower);
    void SelfDestruct();
}

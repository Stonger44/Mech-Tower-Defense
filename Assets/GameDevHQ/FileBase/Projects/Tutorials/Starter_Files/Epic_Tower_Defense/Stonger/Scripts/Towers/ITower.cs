using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    int WarFundCost { get; set; }
    int WarFundSellValue { get; set; }

    int UpgradeWarFundCost { get; set; }
    int UpgradeWarFundSellValue { get; set; }

    bool IsActive { get; set; }

    void ToggleTowerRange(GameObject currentlyViewedTower);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    TowerSprites TowerSprites { get; set; }

    string Name { get; set; }

    int Health { get; set; }
    int InitialHealth { get; set; }
    int UpgradeInitialHealth { get; set; }

    int DamageTaken { get; set; }
    
    int WarFundCost { get; set; }
    int WarFundSellValue { get; set; }
    int WarFundRepairCost { get; set; }
    
    int UpgradeWarFundCost { get; set; }
    int UpgradeWarFundSellValue { get; set; }
    int UpgradeWarFundRepairCost { get; set; }

    void ToggleTowerRange(GameObject currentlyViewedTower);
    void SelfDestruct();
}

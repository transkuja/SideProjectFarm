using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WoodcutterUpgrade : Upgrade
{
    // Upgrade core
    public float respawnReductionTime = 0.0f;
    public int numberIncrease = 0;
    public float speedIncrease = 0.0f;
    public float instantRespawnChance = 0.0f;

    public WoodcutterUpgrade() { }
    public WoodcutterUpgrade(WoodcutterUpgrade _from) : base(_from) { }

    public string GetDescription()
    {
        string description = "";

        if (isUnlocked)
            description += "ALREADY UNLOCKED\n";

        if (respawnReductionTime > 0)
            description += "Reduce respawn time by " + respawnReductionTime * 100.0f + "%\n";

        if (numberIncrease > 0)
            description += "Increase wood units cut by " + numberIncrease + "\n";

        if (speedIncrease > 0)
            description += "Increase cutting speed by " + speedIncrease * 100 + "%\n";

        if (instantRespawnChance > 0)
            description += "Increase chance to have the tree respawn instantly by " + instantRespawnChance * 100.0f + "%\n";

        return description;
    }

    public bool CanPayTheCost()
    {
        if (PlayerDataManager.Instance.Gold < goldCost)
            return false;

        for (int i = 0; i < costs.Length; i++)
        {
            InventoryItem itemInInventory = PlayerDataManager.Instance.Inventory.Find(x => x.itemData.id == costs[i].itemId);
            if (itemInInventory == null)
                return false;
            if (itemInInventory.quantity < costs[i].quantity)
                return false;
        }

        return true;
    }

    public void PayCosts()
    {
        PlayerDataManager.Instance.Gold -= goldCost;

        for (int i = 0; i < costs.Length; i++)
        {
            PlayerDataManager.Instance.RemoveItemFromInventory(costs[i].itemId, costs[i].quantity);
        }

        isUnlocked = true;
    }
}

public class WoodcutterUpgrades
{
    public float respawnReductionTime = 0.0f;
    public int numberIncrease = 0;
    public float speedIncrease = 0.0f;
    public float instantRespawnChance = 0.0f;

    public WoodcutterUpgrades() { }
    public WoodcutterUpgrades(WoodcutterUpgrades _from) { }

    public void AddNewUpgrade(WoodcutterUpgrade _upgrade)
    {
        respawnReductionTime += _upgrade.respawnReductionTime;
        numberIncrease += _upgrade.numberIncrease;
        speedIncrease -= _upgrade.speedIncrease;
        instantRespawnChance += _upgrade.instantRespawnChance;
    }
}

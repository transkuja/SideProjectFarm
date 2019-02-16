using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishUpgrade : Upgrade
{
    [System.Serializable]
    public class Cost
    {
        public string itemId;
        public int quantity;
    }

    // Upgrade settings -> editor setup? scriptable object? hard coded in UI?
    public string upgradeName;
    public bool isUnlocked;
    public Sprite visual;
    public int goldCost;
    public Cost[] costs;
    public int requiredJobLevel;
    public GameObject[] dependency;

    // Upgrade core
    public float catchChance = 0.0f;
    public int numberOfFishIncrease = 0;
    public float speedIncrease = 0.0f;
    public float rareFishChance = 0.0f;

    public FishUpgrade() { }
    public FishUpgrade(FishUpgrade _from) : base(_from) { }

    public string GetDescription()
    {
        string description = "";

        if (isUnlocked)
            description += "ALREADY UNLOCKED\n";

        if (catchChance > 0)
            description += "Increase chance to catch a fish by " + catchChance * 100.0f + "%\n";

        if (numberOfFishIncrease > 0)
            description += "Increase fish unit caught by " + numberOfFishIncrease + "\n";

        if (speedIncrease > 0)
            description += "Increase fishing speed by " + speedIncrease * 100 + "%\n";

        if (rareFishChance > 0)
            description += "Increase chance to catch a rare fish by " + rareFishChance * 100.0f + "%\n";

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

public class FishUpgrades
{
    public float catchChance = 0.6f;
    public int numberOfFishIncrease = 0;
    public float speedIncrease = 1.0f;
    public float rareFishChance = 0.01f;

    public FishUpgrades() { }
    public FishUpgrades(FishUpgrades _from) { }

    public void AddNewUpgrade(FishUpgrade _upgrade)
    {
        catchChance += _upgrade.catchChance;
        numberOfFishIncrease += _upgrade.numberOfFishIncrease;
        speedIncrease -= _upgrade.speedIncrease;
        rareFishChance += _upgrade.rareFishChance;
    }
}

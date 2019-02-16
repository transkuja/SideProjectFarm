using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Jobs { Fisher, Woodcutter, Count }

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    int[] jobsLevel = new int[(int)Jobs.Count];
    int[] jobsExpToNextLevel = new int[(int)Jobs.Count];
    List<InventoryItem> inventory = new List<InventoryItem>();
    private int gold = 0;
    public FishUpgrades fishUpgrades = new FishUpgrades();

    public bool controlsLock = false;

    public delegate void OnJobExp();
    public OnJobExp OnJobExpCallback;

    public delegate void OnUpdateInventory();
    public OnUpdateInventory OnUpdateInventoryCallback;

    public delegate void OnUpdateGold();
    public OnUpdateGold OnUpdateGoldCallback;

    public List<InventoryItem> Inventory
    {
        get
        {
            Debug.Log(inventory.Count);
            return inventory;
        }
    }

    public int[] JobsLevel
    {
        get
        {
            return jobsLevel;
        }
    }

    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;

            if (OnUpdateGoldCallback != null)
                OnUpdateGoldCallback();
        }
    }

    private void Start()
    {
        for (int i = 0; i < JobsLevel.Length; i++)
            JobsLevel[i] = 1;

        for (int i = 0; i < jobsExpToNextLevel.Length; i++)
            jobsExpToNextLevel[i] = ExperienceManager.GetExpToNextLevel(1);


    }

    public int AddItemToInventory<T>(string _itemId, int _quantity = 1) where T : HarvestableData
    {
        if (IsInventoryFull(_itemId, _quantity))
            return 0;

        int returnValue = -1;

        InventoryItem itemInInventory = Inventory.Find(x => x.itemData.id == _itemId && x.quantity < GlobalDesigner.maxInventoryStack);
        int remaining = _quantity;
        if (itemInInventory != null)
        {
            remaining = itemInInventory.quantity + _quantity - GlobalDesigner.maxInventoryStack;
            itemInInventory.quantity = Mathf.Clamp(itemInInventory.quantity + _quantity, 0, GlobalDesigner.maxInventoryStack);

            while (remaining > 0 && Inventory.Count < GetInventorySize())
            {
                InventoryItem newItem = new InventoryItem((T)itemInInventory.itemData, remaining);
                newItem.quantity = remaining;
                remaining = remaining - GlobalDesigner.maxInventoryStack;
                Inventory.Add(newItem);

                if (Inventory.Count == GetInventorySize())
                {
                    if (remaining < GlobalDesigner.maxInventoryStack && remaining > 0)
                        returnValue = remaining;
                }
            }
        }
        else
        {
            remaining = _quantity - GlobalDesigner.maxInventoryStack;

            if (remaining < 0)
            {
                if (Inventory.Count < GetInventorySize())
                {
                    InventoryItem newItem = new InventoryItem(DatabaseManager.GetRowFromId<T>(_itemId), _quantity);
                    Inventory.Add(newItem);
                    returnValue = _quantity;
                }
            }
            else
            {
                if (Inventory.Count < GetInventorySize())
                {
                    InventoryItem newItem = new InventoryItem(DatabaseManager.GetRowFromId<T>(_itemId), GlobalDesigner.maxInventoryStack);
                    Inventory.Add(newItem);
                    if (remaining < GlobalDesigner.maxInventoryStack)
                        returnValue = remaining;

                    while (remaining > 0 && Inventory.Count < GetInventorySize())
                    {
                        newItem = new InventoryItem((T)itemInInventory.itemData, remaining);
                        newItem.quantity = remaining;
                        remaining = remaining - GlobalDesigner.maxInventoryStack;
                        Inventory.Add(newItem);
                        if (remaining < GlobalDesigner.maxInventoryStack && returnValue == -1)
                            returnValue = remaining;
                    }
                }
                else
                    returnValue = 0;
            }
        }

        if (OnUpdateInventoryCallback != null)
            OnUpdateInventoryCallback();

        return returnValue;
    }

    public bool IsInventoryFull(string _nextIdToInsert = "", int quantity = 0)
    {
        if (string.IsNullOrEmpty(_nextIdToInsert))
        {
            return Inventory.Count >= GetInventorySize();
        }
        else
        {
            if (Inventory.Count < GetInventorySize())
                return false;

            List<InventoryItem> itemInInventory = Inventory.FindAll(x => x.itemData.id == _nextIdToInsert && x.quantity < GlobalDesigner.maxInventoryStack);
            if (itemInInventory == null)
                return true;
        }

        return false;
    }

    public void RemoveItemFromInventory(string _itemIdToRemove, int _quantity = 99)
    {
        InventoryItem itemInInventory = Inventory.Find(x => x.itemData.id == _itemIdToRemove);
        if (itemInInventory != null)
        {
            itemInInventory.quantity = Mathf.Clamp(itemInInventory.quantity - _quantity, 0, GlobalDesigner.maxInventoryStack);
            if (itemInInventory.quantity <= 0)
                Inventory.Remove(itemInInventory);

            if (OnUpdateInventoryCallback != null)
                OnUpdateInventoryCallback();

            return;
        }
    }

    public bool HasTheLevelToHarvest(Jobs _job, int _requiredLevel)
    {
        return JobsLevel[(int)_job] >= _requiredLevel;
    }

    public void GainJobExp(Jobs _job, int _harvestableLevel)
    {
        int xpGain = (_harvestableLevel + 1) * 10;

        if (jobsExpToNextLevel[(int)_job] >= _harvestableLevel)
            jobsExpToNextLevel[(int)_job] -= xpGain;
        else
        {
            while (xpGain > 0)
            {
                xpGain -= jobsExpToNextLevel[(int)_job];
                JobLevelUp(_job);
            }

            jobsExpToNextLevel[(int)_job] -= xpGain;
        }

        if (jobsExpToNextLevel[(int)_job] == 0)
            JobLevelUp(_job);

        if (OnJobExpCallback != null)
            OnJobExpCallback();
    }

    public void JobLevelUp(Jobs _job)
    {
        JobsLevel[(int)_job]++;
        jobsExpToNextLevel[(int)_job] = ExperienceManager.GetExpToNextLevel(JobsLevel[(int)_job]);
    }

    public float GetJobRatioToLvlUp(Jobs _job)
    {
        int maxXp = ExperienceManager.GetExpToNextLevel(JobsLevel[(int)_job]);
        return (maxXp - jobsExpToNextLevel[(int)_job]) / (float)maxXp;
    }

    public int GetInventorySize()
    {
        return GlobalDesigner.startingInventorySize;
    }
}

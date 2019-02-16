using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem {

    public HarvestableData itemData;
    public int quantity;

    public InventoryItem()
    {

    }

    public InventoryItem(InventoryItem _data)
    {
        itemData = _data.itemData;
        quantity = _data.quantity;
    }

    public InventoryItem(HarvestableData _itemData, int _quantity)
    {
        itemData = _itemData;
        quantity = _quantity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlInclude(typeof(BuildingData))]
[XmlInclude(typeof(TreeData))]
[XmlInclude(typeof(FishData))]
public class HarvestableData : Data {
    public int initialQuantity;
    public string spriteId;
    public float respawnTime;
    public int requiredLevel;
    public float initialProcessTime;
    public string name;
    public string description;
    public int sellPrice = -1;

    public HarvestableData()
    {

    }

    public HarvestableData(HarvestableData _data) : base(_data)
    {
        initialQuantity = _data.initialQuantity;
        spriteId = _data.spriteId;
        respawnTime = _data.respawnTime;
        requiredLevel = _data.requiredLevel;
        initialProcessTime = _data.initialProcessTime;
        name = _data.name;
        description = _data.description;
        sellPrice = _data.sellPrice;
    }

    public virtual Sprite LoadSprite()
    {
        return null;
    }

}

public class TreeData : HarvestableData
{

    public TreeData()
    {

    }

    public TreeData(TreeData _data) : base(_data)
    {

    }

    public TreeData(HarvestableData _data)
    {
        initialQuantity = _data.initialQuantity;
        spriteId = _data.spriteId;
        respawnTime = _data.respawnTime;
        requiredLevel = _data.requiredLevel;
        initialProcessTime = _data.initialProcessTime;
    }

    public override Sprite LoadSprite()
    {
        if (string.IsNullOrEmpty(spriteId))
            return null;
        return AssetsBundlesManager.GetAssetBundle(BundleName.WoodIcons).LoadAsset<Sprite>(spriteId);
    }
}

public class BuildingData : HarvestableData
{

    public BuildingData()
    {

    }

    public BuildingData(BuildingData _data) : base(_data)
    {

    }

    public BuildingData(HarvestableData _data)
    {
        initialQuantity = _data.initialQuantity;
        spriteId = _data.spriteId;
        respawnTime = _data.respawnTime;
        requiredLevel = _data.requiredLevel;
        initialProcessTime = _data.initialProcessTime;
    }

    public override Sprite LoadSprite()
    {
        if (string.IsNullOrEmpty(spriteId))
            return null;
        return AssetsBundlesManager.GetAssetBundle(BundleName.CommonIcons).LoadAsset<Sprite>(spriteId);
    }
}


public class FishData : HarvestableData
{

    public FishData()
    {

    }

    public FishData(FishData _data) : base(_data)
    {

    }

    public FishData(HarvestableData _data)
    {
        initialQuantity = _data.initialQuantity;
        spriteId = _data.spriteId;
        respawnTime = _data.respawnTime;
        requiredLevel = _data.requiredLevel;
        initialProcessTime = _data.initialProcessTime;
    }

    public override Sprite LoadSprite()
    {
        if (string.IsNullOrEmpty(spriteId))
            return null;
        return AssetsBundlesManager.GetAssetBundle(BundleName.FishIcons).LoadAsset<Sprite>(spriteId);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlInclude(typeof(WoodcutterUpgrade))]
[XmlInclude(typeof(FishUpgrade))]
public class Upgrade : Data {
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

    public Upgrade() { }
    public Upgrade(Upgrade _from) : base(_from) { }
}

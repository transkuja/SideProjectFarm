using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlInclude(typeof(FishUpgrade))]
public class Upgrade : Data {

    public Upgrade() { }
    public Upgrade(Upgrade _from) : base(_from) { }
}

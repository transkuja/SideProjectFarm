using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FishUpgradeUI : UISlotBase
{

    public FishUpgrade upgrade;

    protected override void SetFeedbackData()
    {
        handler.feedbackData = new string[] { upgrade.upgradeName, upgrade.GetDescription() };
        handler.feedbackVisual = upgrade.visual;
    }

    public override void Refresh()
    {
        GetComponent<Button>().interactable = upgrade.CanPayTheCost() && !upgrade.isUnlocked;
    }

    public override void OnClickAction()
    {
        upgrade.PayCosts();
        PlayerDataManager.Instance.fishUpgrades.AddNewUpgrade(upgrade);
        if (GetComponentInParent<UpgraderUI>().RefresherCallback != null)
            GetComponentInParent<UpgraderUI>().RefresherCallback();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Refresh();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHandler : UIHandlerBase {

    public GameObject feedbackQty;
    public GameObject feedbackSellPrice;

    public override void ChangeFeedbackData()
    {
        base.ChangeFeedbackData();

        if (feedbackData != null)
        {
            feedbackQty.SetActive(!string.IsNullOrEmpty(feedbackData[2]) && int.Parse(feedbackData[2]) > 1);
            feedbackQty.GetComponent<Text>().text = feedbackData[2];

            feedbackSellPrice.SetActive(!string.IsNullOrEmpty(feedbackData[3]) && int.Parse(feedbackData[3]) > 0);
            feedbackSellPrice.GetComponent<Text>().text = "Sell for " + feedbackData[3];
        }
        else
            HideFeedback();
    }

    public override void HideFeedback()
    {
        base.HideFeedback();
        feedbackQty.SetActive(false);
        feedbackSellPrice.SetActive(false);
    }

    private void OnEnable()
    {
        for (int i = PlayerDataManager.Instance.GetInventorySize(); i < entries.transform.childCount; i++)
            entries.transform.GetChild(i).gameObject.SetActive(false);
    }

    public void CleanUpInventory()
    {
        for (int i = 0; i < PlayerDataManager.Instance.GetInventorySize(); i++)
        {
            entries.transform.GetChild(i).GetComponentInChildren<InventoryUISlot>().ResetData();
        }
    }
}

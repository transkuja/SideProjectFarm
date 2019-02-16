using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUISlot : UISlotBase {

    private InventoryItem data;
    [SerializeField]
    protected GameObject quantity;

    public InventoryItem Data
    {
        get
        {
            return data;
        }

        set
        {
            data = value;
            Refresh();
        }
    }

    protected override void SetFeedbackData()
    {
        if (data == null)
        {
            handler.feedbackData = null;
            handler.feedbackVisual = null;
        }
        else
        {
            handler.feedbackData = new string[] { Data.itemData.name, Data.itemData.description, Data.quantity.ToString(), Data.itemData.sellPrice.ToString() };
            handler.feedbackVisual = Data.itemData.LoadSprite();
        }
    }

    public override void Refresh()
    {
        if (data == null)
        {
            quantity.SetActive(false);
            visual.SetActive(false);
            transform.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        else
        {
            visual.SetActive(true);
            quantity.SetActive(Data.quantity > 1);
            quantity.GetComponent<Text>().text = Data.quantity.ToString();

            transform.GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetComponent<Button>().onClick.AddListener(OnClickAction);
        }
    }

    public override void OnClickAction()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Data.itemData.sellPrice > 0)
        {
            PlayerDataManager.Instance.RemoveItemFromInventory(Data.itemData.id, 1);
            PlayerDataManager.Instance.Gold += Data.itemData.sellPrice;

            if (Data.quantity == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
                ((InventoryUIHandler)handler).CleanUpInventory();
                SetFeedbackData();
            }
            else
            {
                Refresh();
            }

            handler.ChangeFeedbackData();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetData();
    }

    public void ResetData()
    {
        if (transform.GetSiblingIndex() < PlayerDataManager.Instance.Inventory.Count)
            Data = PlayerDataManager.Instance.Inventory[transform.GetSiblingIndex()];
        else
            Data = null;
        visual.GetComponent<Image>().sprite = UpdateSlotVisual(); 
    }

    protected override Sprite UpdateSlotVisual()
    {
        if (Data != null)
            return Data.itemData.LoadSprite();
        return null;
    }
}

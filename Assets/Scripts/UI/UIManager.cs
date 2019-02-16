using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    public GameObject fisherXp;
    public GameObject woodcutterXp;

    public Text inventory;
    public Text gold;
    public GameObject inventoryPanel;

    void Start () {
        PlayerDataManager.Instance.OnUpdateInventoryCallback += UpdateInventoryUI;
        PlayerDataManager.Instance.OnJobExpCallback += UpdateJobUI;
        PlayerDataManager.Instance.OnUpdateGoldCallback += UpdateGold;

        Invoke("UpdateJobUI", 0.2f);
        Invoke("UpdateInventoryUI", 0.2f);
        Invoke("UpdateGold", 0.2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
        }
    }

    public void UpdateJobUI()
    {
        fisherXp.transform.GetChild(1).GetComponent<Image>().fillAmount = PlayerDataManager.Instance.GetJobRatioToLvlUp(Jobs.Fisher);
        fisherXp.transform.GetComponentInChildren<Text>().text = "Lvl " + PlayerDataManager.Instance.JobsLevel[(int)Jobs.Fisher];

        woodcutterXp.transform.GetChild(1).GetComponent<Image>().fillAmount = PlayerDataManager.Instance.GetJobRatioToLvlUp(Jobs.Woodcutter);
        woodcutterXp.transform.GetComponentInChildren<Text>().text = "Lvl " + PlayerDataManager.Instance.JobsLevel[(int)Jobs.Woodcutter];

    }

    public void UpdateInventoryUI()
    {
        inventory.text = "";
        foreach (var item in PlayerDataManager.Instance.Inventory)
        {
            inventory.text += item.quantity + "x " + item.itemData.id + "\n";
        }
    }

    public void UpdateGold()
    {
        gold.text = PlayerDataManager.Instance.Gold.ToString();        
    }

}


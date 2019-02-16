using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    public GameObject fisherXp;
    public GameObject woodcutterXp;

    public Text gold;
    public GameObject inventoryPanel;

    void Start () {
        PlayerDataManager.Instance.OnJobExpCallback += UpdateJobUI;
        PlayerDataManager.Instance.OnUpdateGoldCallback += UpdateGold;

        Invoke("UpdateJobUI", 0.2f);
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

    public void UpdateGold()
    {
        gold.text = PlayerDataManager.Instance.Gold.ToString();        
    }

}


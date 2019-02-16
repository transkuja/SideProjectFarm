using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fish : Interactable {

    protected override void Start()
    {
        base.Start();
        LoadData<FishData>();
    }


    protected override void OnClickAction()
    {
        if (!PlayerDataManager.Instance.HasTheLevelToHarvest(Jobs.Fisher, ((FishData)interactableData).requiredLevel)
                || PlayerDataManager.Instance.controlsLock)
            return;

        base.OnClickAction();

        fishing = StartCoroutine(FishCoroutine());
    }

    Coroutine fishing;
    IEnumerator FishCoroutine()
    {
        PlayerDataManager.Instance.controlsLock = true;
        isCoroutineActive = true;

        while (true)
        {
            visual.GetComponentInChildren<Renderer>().material.color = Color.blue;
            float processTime = ((FishData)interactableData).initialProcessTime * PlayerDataManager.Instance.fishUpgrades.speedIncrease;
            yield return new WaitForSeconds(processTime);
            windowOpened = true;
            visual.GetComponentInChildren<Renderer>().material.color = Color.yellow;
            yield return new WaitForSeconds(2.0f);

            if (Random.Range(0.0f, 1.0f) > PlayerDataManager.Instance.fishUpgrades.catchChance)
                continue;

            windowOpened = false;

            int quantity = ((FishData)interactableData).initialQuantity + PlayerDataManager.Instance.fishUpgrades.numberOfFishIncrease;

            // Pop feedback
            GameObject feedback = Instantiate(Resources.Load<GameObject>("Feedback"), UIManager.Instance.transform);
            feedback.GetComponent<AscendingFeedback>().InitFeedback(((FishData)interactableData).LoadSprite(), quantity);
            feedback.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            visual.GetComponentInChildren<Renderer>().material.color = Color.white;

            PlayerDataManager.Instance.AddItemToInventory<FishData>(interactableData.id, quantity);
            PlayerDataManager.Instance.GainJobExp(Jobs.Fisher, ((FishData)interactableData).requiredLevel);
        }
    }

    bool stopProcessCalled;
    bool windowOpened = false;
    bool isCoroutineActive = false;
    private void Update()
    {
        if (windowOpened)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("start minigame");
                isCoroutineActive = false;
                PlayerDataManager.Instance.controlsLock = false;
                visual.GetComponentInChildren<Renderer>().material.color = Color.white;
                windowOpened = false;
                StopCoroutine(fishing);
            }
        }

        if (isCoroutineActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Stop process");
                isCoroutineActive = false;
                PlayerDataManager.Instance.controlsLock = false;
                visual.GetComponentInChildren<Renderer>().material.color = Color.white;
                windowOpened = false;
                StopCoroutine(fishing);
            }
        }
    }
}

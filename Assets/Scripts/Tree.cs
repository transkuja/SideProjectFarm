using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tree : Interactable {

    protected override void Start()
    {
        base.Start();
        LoadData<TreeData>();
    }


    protected override void OnClickAction()
    {
        if (currentRespawnTimer > 0.0f
                || !PlayerDataManager.Instance.HasTheLevelToHarvest(Jobs.Woodcutter, ((TreeData)interactableData).requiredLevel))
            return;

        base.OnClickAction();

        StartCoroutine(CutCoroutine());
    }

    IEnumerator CutCoroutine()
    {
        PlayerDataManager.Instance.controlsLock = true;
        visual.GetComponentInChildren<Renderer>().material.color = Color.green;

        yield return new WaitForSeconds(((TreeData)interactableData).initialProcessTime);

        // Pop feedback
        GameObject feedback = Instantiate(Resources.Load<GameObject>("Feedback"), UIManager.Instance.transform);
        feedback.GetComponent<AscendingFeedback>().InitFeedback(((TreeData)interactableData).LoadSprite(), ((TreeData)interactableData).initialQuantity);
        feedback.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        visual.GetComponentInChildren<Renderer>().material.color = Color.white;

        GetComponent<Collider>().enabled = false;
        visual.SetActive(false);

        PlayerDataManager.Instance.AddItemToInventory<TreeData>(interactableData.id, ((TreeData)interactableData).initialQuantity);
        currentRespawnTimer = ((TreeData)interactableData).respawnTime;
        PlayerDataManager.Instance.GainJobExp(Jobs.Woodcutter, ((TreeData)interactableData).requiredLevel);

        PlayerDataManager.Instance.controlsLock = false;
    }

    private void Update()
    {
        if (currentRespawnTimer > 0.0f)
            currentRespawnTimer -= Time.deltaTime;
        else
        {
            if (!visual.activeInHierarchy)
            {
                visual.SetActive(true);
                GetComponent<Collider>().enabled = true;
            }
        }
    }
}

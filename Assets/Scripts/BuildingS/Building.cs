using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Interactable {

    protected override void Start()
    {
        base.Start();
        LoadData<BuildingData>();
        if (currentRespawnTimer <= 0.0f)
            visual.GetComponentInChildren<Renderer>().material.color = Color.yellow;
    }


    protected override void OnClickAction()
    {
        if (currentRespawnTimer > 0.0f)
            return;

        base.OnClickAction();

        GameObject feedback = Instantiate(Resources.Load<GameObject>("Feedback"), UIManager.Instance.transform);
        Sprite spriteToAssign = AssetsBundlesManager.GetAssetBundle(BundleName.CommonIcons).LoadAsset<Sprite>("gold");
        feedback.GetComponent<AscendingFeedback>().InitFeedback(spriteToAssign, ((BuildingData)interactableData).initialQuantity);
        visual.GetComponentInChildren<Renderer>().material.color = Color.white;

        PlayerDataManager.Instance.Gold += ((BuildingData)interactableData).initialQuantity;
        currentRespawnTimer = ((BuildingData)interactableData).respawnTime;
    }

    private void Update()
    {
        if (currentRespawnTimer > 0.0f)
        {
            currentRespawnTimer -= Time.deltaTime;
            if (currentRespawnTimer <= 0.0f)
                visual.GetComponentInChildren<Renderer>().material.color = Color.yellow;
        }
    }

}

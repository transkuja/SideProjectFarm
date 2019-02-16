using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShop : Interactable
{

    [SerializeField]
    GameObject canvas;

    protected override void Start()
    {
        base.Start();
        LoadData<BuildingData>();
    }


    protected override void OnClickAction()
    {
        base.OnClickAction();

        PlayerDataManager.Instance.controlsLock = true;
        canvas.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerDataManager.Instance.controlsLock = false;
            canvas.SetActive(false);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public string interactableId;
    public float currentRespawnTimer;
    public GameObject visual;

    protected Data interactableData;

    protected virtual void Start()
    {
        if (visual == null)
            visual = GetComponentInChildren<Renderer>().gameObject;
    }

    protected void LoadData<T>() where T : Data
    {
        DatabaseManager.Init();
        interactableData = DatabaseManager.GetRowFromId<T>(interactableId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PlayerDataManager.Instance.controlsLock)
            return;

        OnClickAction();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    protected virtual void OnClickAction()
    {

    }

    protected void PopFeedback<T>()
    {

    }
}

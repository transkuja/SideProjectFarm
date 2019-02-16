using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class UISlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    protected UIHandlerBase handler;

    [SerializeField]
    protected GameObject visual;

    public void OnPointerEnter(PointerEventData eventData)
    {      
        SetFeedbackData();
        handler.ChangeFeedbackData();
    }

    /// <summary>
    ///     Sends data to handler to use it OnPointerEnter
    /// </summary>
    protected virtual void SetFeedbackData()
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        handler.HideFeedback();
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Start()
    {
        handler = GetComponentInParent<UIHandlerBase>();

        visual.GetComponent<Image>().sprite = UpdateSlotVisual();
        transform.GetComponent<Button>().onClick.RemoveAllListeners();
        transform.GetComponent<Button>().onClick.AddListener(OnClickAction);
    }

    protected virtual Sprite UpdateSlotVisual()
    {
        return null;
    }

    protected virtual void OnEnable()
    {
        handler = GetComponentInParent<UIHandlerBase>();
        handler.RefresherCallback += Refresh;
    }

    public virtual void Refresh()
    {

    }

    public virtual void OnClickAction()
    {

    }
}

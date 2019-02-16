using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandlerBase : MonoBehaviour {

    public delegate void Refresher();
    public Refresher RefresherCallback;

    public GameObject entries;

    [Header("Feedback")]
    public GameObject image;
    public GameObject feedbackTitle;
    public GameObject feedbackDescription;

    [HideInInspector]
    public string[] feedbackData;
    [HideInInspector]
    public Sprite feedbackVisual;

    public virtual void ChangeFeedbackData()
    {
        image.SetActive((feedbackVisual != null));
        image.GetComponent<Image>().sprite = feedbackVisual;

        if (feedbackData != null)
        {
            if (feedbackData.Length > 0)
            {
                feedbackTitle.SetActive(!string.IsNullOrEmpty(feedbackData[0]));
                feedbackTitle.GetComponent<Text>().text = feedbackData[0];
            }
            else
                feedbackTitle.SetActive(false);

            if (feedbackData.Length > 1)
            {
                feedbackDescription.SetActive(!string.IsNullOrEmpty(feedbackData[1]));
                feedbackDescription.GetComponent<Text>().text = feedbackData[1];
            }
            else
                feedbackDescription.SetActive(false);
        }
    }

    public virtual void HideFeedback()
    {
        image.SetActive(false);
        feedbackTitle.SetActive(false);
        feedbackDescription.SetActive(false);
    }

    private void OnDisable()
    {
        RefresherCallback = null;
    }
}

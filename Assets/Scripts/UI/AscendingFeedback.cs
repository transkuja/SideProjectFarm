using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AscendingFeedback : MonoBehaviour {

    public float speed = 3.0f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
	public void InitFeedback(Sprite _sprite, int _quantity)
    {
        GetComponentInChildren<Image>().sprite = _sprite;
        GetComponentInChildren<Text>().text = "x" + _quantity;
    }
	
	void Update () {
        transform.position += Time.deltaTime * speed * Vector3.up;       
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLocationToButtonClick : MonoBehaviour {
	
	void Update () {
		if (!gameObject.activeInHierarchy)
			return;
		if (Input.GetMouseButtonDown(0)) {
			if (GetComponent<RectTransform>().rect.Contains(GetComponent<RectTransform>().InverseTransformPoint(Input.mousePosition))) {
				GetComponent<Button>().onClick.Invoke();
			}
		}
		//prevLoc = Input.mousePosition;
	}
}

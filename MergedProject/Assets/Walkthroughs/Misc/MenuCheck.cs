using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCheck : MonoBehaviour {
	public Color disabledColor, currentColor, clickableColor;
	public List<BasicInteractable> menuList;
	public Sprite arrow;
	public Color arrowColor;
	public List<menuItem> arrowItemList;

	[System.Serializable]
	public struct menuItem{
		public GameObject go;
		public Vector2 range;
	}
	[HideInInspector]
	public int checkingInt;

	void Start()
	{
		foreach (menuItem a in arrowItemList) {
			a.go.transform.FindChild ("Arrow1").gameObject.GetComponent<Image> ().sprite = arrow;
			a.go.transform.FindChild ("Arrow2").gameObject.GetComponent<Image> ().sprite = arrow;
			a.go.transform.FindChild ("Arrow1").gameObject.GetComponent<Image> ().color = arrowColor;
			a.go.transform.FindChild ("Arrow2").gameObject.GetComponent<Image> ().color = arrowColor;
		}
	}

	public void DatabaseLoadInt()
	{
		if (GameObject.FindGameObjectWithTag ("MessageHolder").GetComponent<DataLoader> ())
			checkingInt = GameObject.FindGameObjectWithTag ("MessageHolder").GetComponent<DataLoader> ().procedureStatus;
		else
			Debug.LogAssertion ("Menu cannot find DataLoader");
	}

	public void CheckInt()
	{
		DatabaseLoadInt ();
		for (int i = 0; i < checkingInt; i++) {
			menuList [i].isInteractiable = true;
			menuList [i].gameObject.GetComponent<Image> ().color = clickableColor;
		}
		menuList [checkingInt].isInteractiable = true;
		menuList [checkingInt].gameObject.GetComponent<Image> ().color = currentColor;
		for (int i = checkingInt+1; i < menuList.Count; i++) {
			menuList [i].isInteractiable = false;
			menuList [i].gameObject.GetComponent<Image> ().color = disabledColor;
		}
		ArrowCheck ();
	}

	void ArrowCheck()
	{
		foreach (menuItem a in arrowItemList) {
			if (a.range.x <= checkingInt && a.range.y >= checkingInt) {
				a.go.transform.FindChild ("Arrow1").gameObject.SetActive (true);
				a.go.transform.FindChild ("Arrow2").gameObject.SetActive (true);
			} else {
				a.go.transform.FindChild ("Arrow1").gameObject.SetActive (false);
				a.go.transform.FindChild ("Arrow2").gameObject.SetActive (false);
			}
		}
	}
}

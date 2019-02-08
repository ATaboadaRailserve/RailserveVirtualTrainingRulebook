using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia_Quester : MonoBehaviour {
	[System.Serializable]
	public struct EncyclopediaItem {
		public GameObject item;
		public bool viewed;
		public InteractionHandler.InvokableState onViewStart;
	}
	public Text currentInstructions;
	public EncyclopediaItem[] items;
	public InteractionHandler.InvokableState onAllItemsViewed;
	// Use this for initialization
	public void View(GameObject self)
	{
		for (int i = 0; i < items.Length; i++) {
			if (items [i].item.name == self.name) {
				items [i].viewed = true;
				items [i].onViewStart.Invoke ();
			}
		}
		bool tempB = true;
		for (int i = 0; i < items.Length; i++) {
			if (items [i].viewed == false) {
				tempB = false;
			}
		}
		if (tempB == true) {
			onAllItemsViewed.Invoke ();
		}
	}
}

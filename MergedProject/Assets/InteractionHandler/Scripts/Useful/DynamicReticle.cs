using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicReticle : MonoBehaviour {
	public enum interactionMethod {Tag, Layer, Prefix};
	[System.Serializable]
	public struct ReticleItem {
		public interactionMethod interaction; //Where to look for the interactionTitle
		public string interactionTitle; 
		public Sprite reticleImage; //Image to change to
		public Vector2 scale;
		public Vector2 offset;
		public Color[] colorList;

		[HideInInspector]
		public Color color;
	}

	public InteractionHandler interactionHandler;
	[Tooltip("The max distance at which the reticle will change")]
	public float RaycastHitDistance; //Raycast Hit Distance
	public GameObject reticle; //The gameobject that holds the original reticle
	public ReticleItem[] reticleList; 

	private ReticleItem defaultReticle; //Original Reticle
	private int currentIndex;
	// Use this for initialization
	void Start () {
		defaultReticle.interaction = interactionMethod.Tag;
		defaultReticle.reticleImage = reticle.GetComponent<Image>().sprite;
		defaultReticle.scale = reticle.transform.localScale;
		defaultReticle.color = reticle.GetComponent<Image> ().color;
		defaultReticle.offset = new Vector2 (0, 0);
		for (int i = 0; i < reticleList.Length; i++) {
			if (reticleList [i].colorList.Length == 0) {
				reticleList [i].colorList = new Color[1] { Color.white };
			}
			currentIndex = i;
			ChangeColor (0);
		}
	}

	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Ray ray = interactionHandler.interactionCamera.ScreenPointToRay(Input.mousePosition); 
		if (Physics.Raycast (ray, out hit, RaycastHitDistance)) { //If the raycast hits an object at the RaycastHitDistance
			bool changeReticle = false;
			for (int i = 0; i < reticleList.Length; i++) {
				switch(reticleList[i].interaction){
				case interactionMethod.Tag:
					if (hit.collider.tag == reticleList [i].interactionTitle) {
						reticle.GetComponent<Image> ().sprite = reticleList [i].reticleImage;
						reticle.transform.localScale = new Vector3 (reticleList [i].scale.x, reticleList [i].scale.y, 1);
						reticle.transform.localPosition = new Vector3 (reticleList [i].offset.x, reticleList [i].offset.y, 0);
						reticle.GetComponent<Image> ().color = reticleList [i].color;
						changeReticle = true;
						if (currentIndex != i)
							ChangeColor (0);
						currentIndex = i;
					}
					break;
				case interactionMethod.Layer:
					if (LayerMask.LayerToName (hit.collider.gameObject.layer) == reticleList [i].interactionTitle) {
						reticle.GetComponent<Image> ().sprite = reticleList [i].reticleImage;
						reticle.transform.localScale = new Vector3 (reticleList [i].scale.x, reticleList [i].scale.y, 1);
						reticle.transform.localPosition = new Vector3 (reticleList [i].offset.x, reticleList [i].offset.y, 0);
						reticle.GetComponent<Image> ().color = reticleList [i].color;
						changeReticle = true;
						if (currentIndex != i)
							ChangeColor (0);
						currentIndex = i;
					}
					break;
				case interactionMethod.Prefix:
					if (hit.collider.gameObject.name.Substring(0,reticleList[i].interactionTitle.Length) == reticleList [i].interactionTitle) {
						reticle.GetComponent<Image> ().sprite = reticleList [i].reticleImage;
						reticle.transform.localScale = new Vector3 (reticleList [i].scale.x, reticleList [i].scale.y, 1);
						reticle.transform.localPosition = new Vector3 (reticleList [i].offset.x, reticleList [i].offset.y, 0);
						reticle.GetComponent<Image> ().color = reticleList [i].color;
						changeReticle = true;
						if (currentIndex != i)
							ChangeColor (0);
						currentIndex = i;
					}
					break;
				}
			}
			if (!changeReticle) {
				ChangeColor (0);
				reticle.GetComponent<Image> ().sprite = defaultReticle.reticleImage;
				reticle.transform.localScale = new Vector3 (defaultReticle.scale.x, defaultReticle.scale.y, 1);
				reticle.transform.localPosition =  new Vector3 (defaultReticle.offset.x, defaultReticle.offset.x, 0);
				reticle.GetComponent<Image> ().color = defaultReticle.color;
			}
		} else {
			ChangeColor (0);
			reticle.GetComponent<Image> ().sprite = defaultReticle.reticleImage;
			reticle.transform.localScale = new Vector3 (defaultReticle.scale.x, defaultReticle.scale.y, 1);
			reticle.transform.localPosition =  new Vector3 (defaultReticle.offset.x, defaultReticle.offset.x, 0);
			reticle.GetComponent<Image> ().color = defaultReticle.color;
		}
	}
	public void ChangeColor (int index)
	{
		if (index <= reticleList [currentIndex].colorList.Length - 1) {
			reticleList [currentIndex].color = reticleList [currentIndex].colorList [index];
		}
	}

}

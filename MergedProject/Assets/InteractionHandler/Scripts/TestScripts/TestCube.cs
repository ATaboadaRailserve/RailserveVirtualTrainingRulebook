using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : Interactable {
	
	public float maxDistance;
	public LookPoints lookPoints;
	
	private Material mat;
	
	void Start () {
		mat = GetComponent<Renderer>().material;
	}

	public override void IHButton (InteractionHandler.InteractionParameters iParam) {
		if (iParam.distance > maxDistance) {
			if (iParam.isFirst)
				mat.color = Color.blue;
			else
				mat.color = Color.cyan;
			return;
		}
		if (mat.color != Color.red) {
			if (iParam.isFirst)
				mat.color = new Color(0.5f,0,0.75f);
			else
				mat.color = Color.black;
		}
	}

	public override void IHButtonDown (InteractionHandler.InteractionParameters iParam) {
		if (iParam.distance > maxDistance)
			return;
		interactionHandler.InvokeAction("PlayerClickedCube");
		lookPoints.LookAtPoint(false, 1);
		mat.color = Color.red;
	}
	
	public override void IHButtonEnter (InteractionHandler.InteractionParameters iParam) {
		if (iParam.distance > maxDistance)
			return;
		if (iParam.isFirst)
			mat.color = new Color(0.5f,0,0.75f);
		else
			mat.color = Color.black;
	}
	
	public override void IHButtonExit (InteractionHandler.InteractionParameters iParam) {
		if (iParam.isFirst)
			mat.color = Color.blue;
		else
			mat.color = Color.cyan;
	}
	
	public override void IHButtonUp (InteractionHandler.InteractionParameters iParam) {
		if (iParam.distance > maxDistance)
			return;
		if (iParam.isFirst)
			mat.color = Color.green;
		else
			mat.color = Color.yellow;
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			interactionHandler.InvokeAction("PlayerTouchedCube");
			lookPoints.LookAtPoint(false, 0);
		}
	}
}
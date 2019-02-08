using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouplingDevice : MonoBehaviour {
	
	public InteractionHandler.InvokableState onCoupled;
	public InteractionHandler.InvokableState onStretched;
	
	public BaseLogic[] stretchLogics;
	
	public CarCounter carCounter;
	public int[] distances;
	
	private int logicIndex;
	private int distanceIndex;
	private bool distanceSet;
	
	void Start () {
		SetDistance();
	}
	
	public void Couple () {
		transform.localPosition += new Vector3(0,0,11.65f);
		onCoupled.Invoke();
	}
	
	public void Stretch () {
		onStretched.Invoke();
		if (logicIndex < stretchLogics.Length) {
			stretchLogics[logicIndex].IsTrue = true;
			logicIndex++;
		}
	}
	
	public void SetDistance () {
		if (distanceSet)
			return;
		if (distanceIndex < stretchLogics.Length) {
			//print(distanceIndex + " | " + distances[distanceIndex]);
			//carCounter.DistanceInCars = distances[distanceIndex];
			distanceIndex++;
			distanceSet = true;
			StartCoroutine(Cooldown());
		} else {
			//print(distanceIndex + " | " + distances[distanceIndex]);
			//carCounter.DistanceInCars = distances[distances.Length-1];
			distanceSet = true;
			StartCoroutine(Cooldown());
		}
	}
	
	IEnumerator Cooldown () {
		yield return new WaitForSeconds(3);
		distanceSet = false;
	}
}

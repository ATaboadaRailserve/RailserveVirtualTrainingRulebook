using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarGenerator : MonoBehaviour {
	
	public Transform bezier;
	public GameObject[] carTypes;
	public bool intermittent = false;
	[Range(0,1)]
	public float gapBias = 0.3f;
	public float addedGap = 2.0f;
	public int minCount = 0;
	public int maxCount = 40;
	public int countCurvePower = 2;
	public AnimationCurve carWeight = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public AnimationCurve intermittentVariable = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public List<int> carList;
	public List<int> pullList;
	
	private GameObject temp;
	
	void Start(){
		carList = new List<int>();
		pullList = new List<int>();
	}
	
	void Generate () {
		int count = (int)Mathf.Round((1-Mathf.Pow(((Mathf.Pow(Random.value*2-1, 3)+1)/2), countCurvePower))*maxCount);
		count = Mathf.Clamp(count, minCount, maxCount);
		if (intermittent) {
			for (int i = 0; i < count; i++) {
				if (intermittentVariable.Evaluate(Random.value) > gapBias) {
					int carIndex = (int)Mathf.Round(carWeight.Evaluate(Random.value)*carTypes.Length-1);
					carIndex = (int)Mathf.Max(carIndex, 0);
					Vector3 loc = transform.position;
					temp = (GameObject)Instantiate(carTypes[carIndex], loc + new Vector3(0.67f,-0.2f,i*(12.7f+addedGap)), transform.rotation);
					temp.transform.parent = transform;
					if(temp){
						if(bezier){
							temp.BroadcastMessage("SetBezier", bezier);
							temp.BroadcastMessage("First");
						}
					}
				}
			}
		} else {
			for (int i = 0; i < count; i++) {
				int carIndex = (int)Mathf.Round(carWeight.Evaluate(Random.value)*carTypes.Length-1);
				carIndex = (int)Mathf.Max(carIndex, 0);
				Vector3 loc = transform.position;
				temp = (GameObject)Instantiate(carTypes[carIndex], loc + new Vector3(0.67f,-0.2f,i*12.7f), transform.rotation);
				temp.transform.parent = transform;			
				if(temp){
					if(bezier){
						temp.BroadcastMessage("SetBezier", bezier);
						temp.BroadcastMessage("First");
					}
				}
				
			}
		}
	}
	
	void AddNumber(int number){
		carList.Add(number);
	}
	
	void AddPull (int index) {
		pullList.Add(index);
	}
}

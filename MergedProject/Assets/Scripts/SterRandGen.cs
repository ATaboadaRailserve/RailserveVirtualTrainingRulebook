using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SterRandGen : MonoBehaviour {
	public RandomActivator[] randomActivators;
	
	List<int> numbers;
	
	int index = 0;
	
	void Start () {
		numbers = new List<int>();
		for (int i = 0; i < randomActivators.Length; i++) {
			int tempNum = Random.Range(0, 10);
			while (numbers.Contains(tempNum)) {
				tempNum = Random.Range(0, 10);
			}
			numbers.Add(tempNum);
			randomActivators[i].num = tempNum;
		}
	}
}

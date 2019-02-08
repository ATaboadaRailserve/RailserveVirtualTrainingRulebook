using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableMatching : MonoBehaviour {

	public GameObject source;
	public GameObject target;
	public bool doInverse;
	
	void Update()
	{
		target.SetActive(source.activeSelf != doInverse);
	}
}

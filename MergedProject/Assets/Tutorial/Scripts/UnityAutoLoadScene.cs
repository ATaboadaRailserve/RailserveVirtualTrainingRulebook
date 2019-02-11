using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAutoLoadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	#if UNITY_EDITOR
		gameObject.SetActive(true);
	#endif
	}
}

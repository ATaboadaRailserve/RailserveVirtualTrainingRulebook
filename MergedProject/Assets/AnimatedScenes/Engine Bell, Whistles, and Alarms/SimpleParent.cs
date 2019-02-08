using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParent : MonoBehaviour {

    public Transform parent;
    Vector3 localPosition;

	
	void Start () {
        localPosition = transform.position - parent.position;
	}
	
	
	void FixedUpdate () {
        transform.position = parent.position + localPosition;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTrigger : MonoBehaviour {

    public string triggerName;
    public GameObject obstacleHit;
	// Use this for initialization
	void Start () {
        obstacleHit.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider col)
    {
        if (col.name == triggerName)
        {
            obstacleHit.SetActive(true);
        }
    }
}

using UnityEngine;
using System.Collections;

public class ItemSelect : MonoBehaviour {
    public GameObject name;
    public GameObject background;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Explode(GameObject target)
    {
        name.SetActive(true);
        //background.SetActive(true);
    }
}

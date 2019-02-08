using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uber_Asset_Controller : MonoBehaviour {

    public GameObject[] redZones;
    public CrossingSter_Mngr tCrossingSystem;

	// Use this for initialization
	void Start () {
        redZones = GameObject.FindGameObjectsWithTag("RedZone");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #region UBER_SCENE_FUNCTIONS
    public void DisableRedZones ()
    {
        foreach (GameObject redzone in redZones)
        {
            redzone.SetActive(false);
        }
    }

    public void EnableRedZones ()
    {
        foreach (GameObject redzone in redZones)
        {
            redzone.SetActive(true);
        }
    }
    #endregion
}

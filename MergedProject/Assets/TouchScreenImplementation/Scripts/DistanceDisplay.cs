using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DistanceDisplay : MonoBehaviour {
    public GameObject mainCameraObject;
    public GameObject targetObject;
    public bool enableTouchCamera = false;
    public bool readyTouchCamera = true;

    private float distance;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        distance = Vector3.Distance(mainCameraObject.transform.position, targetObject.transform.position);
        if(distance <= 10)
        {
            GameObject.Find("DistanceDisplay").GetComponentInChildren<Text>().text = distance.ToString();
            if (distance <= 3 && readyTouchCamera)
            {
                enableTouchCamera = true;
                readyTouchCamera = false;
            }
            else if(distance > 3)
            {
                enableTouchCamera = false;
            }
        }  
        else
        {
            GameObject.Find("DistanceDisplay").GetComponentInChildren<Text>().text = "N/A";
        }

        if (distance > 5)
        {
            readyTouchCamera = true;
        }
        //Debug.Log(enableTouchCamera);

    }
}

using UnityEngine;
using System.Collections;

public class Clickable : MonoBehaviour {
    //Point to the desired UI element
    public GameObject UI_Element;

    LineRenderer line;
    Camera maincamera;
    
	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
        maincamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, maincamera.ScreenToWorldPoint(new Vector3(UI_Element.GetComponent<RectTransform>().position.x, UI_Element.GetComponent<RectTransform>().position.y, 10)));
	}

    void ShowLine()
    {
    }

    void HideLine()
    {
    }
}

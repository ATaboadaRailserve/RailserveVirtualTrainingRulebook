using UnityEngine;
using System.Collections;

public class CustomButton : MonoBehaviour {
    TurnTable turnTable;
    GameObject nextItem;
	// Use this for initialization
	void Start () {
        turnTable = GameObject.Find("TurnTableCamera").GetComponent<TurnTable>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SelectItem()
    {
        if (nextItem != null)
        {
            turnTable.SelectItem(nextItem);
            //turnTable.ZoomIn(nextItem);        
        }
    }

    public void SetItem(GameObject g)
    {
        nextItem = g;
    }
}

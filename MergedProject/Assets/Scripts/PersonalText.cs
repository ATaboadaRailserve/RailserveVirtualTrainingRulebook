using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalText : MonoBehaviour {

	public Text text;
	public Convention convention;
	
	[Header("exact concat, include spaces")]
	public string beforeName;
	public string afterName;
	
	public enum Convention
	{
		FirstOnly,
		LastOnly,
		FirstLast
	}

	static DataLoader dataLoader;
	
	// Use this for initialization
	void Start () {
		if(dataLoader == null)
		{
			dataLoader = GameObject.FindObjectOfType<DataLoader>();
			if(dataLoader == null)
			{
				Debug.LogError("No DataLoader Present for PersonalText");
				return;
			}
		}
		
		if(text == null)
		{
			text = GetComponent<Text>();
			if(text == null)
			{
				Debug.LogError("No Text Object Assigned to PersonalText");
				return;
			}
		}
		
		string firstName = dataLoader.CurrentUser.FirstName;
		string lastName = dataLoader.CurrentUser.LastName;
		
		switch(convention)
		{
			case Convention.FirstOnly:
				text.text = beforeName + firstName + afterName;
				break;
			case Convention.LastOnly:
				text.text = beforeName + lastName + afterName;
				break;
			case Convention.FirstLast:
				text.text = beforeName + firstName + " " + lastName + afterName;
				break;
		}
	}
}

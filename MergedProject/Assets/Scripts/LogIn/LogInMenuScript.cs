using UnityEngine;
using System.Collections;

public class LogInMenuScript : MonoBehaviour {

	//public GameObject dataLoaderObj, dataLoaderPrefab;
	DataLoader dataLoader;

	// Use this for initialization
	void Start () 
	{
		dataLoader = GameObject.FindWithTag("MessageHolder").GetComponent<DataLoader>();
		/*
		dataLoaderObj = GameObject.Find("DataLoadObject");
		if(dataLoaderObj == null)
		{
			Instantiate(dataLoaderPrefab);
			dataLoaderObj = GameObject.Find("DataLoadObject(Clone)");
			dataLoaderObj.transform.name = "DataLoadObject";
			dataLoader = dataLoaderObj.GetComponent<DataLoader>();
		}
		else
		{
			dataLoader = dataLoaderObj.GetComponent<DataLoader>();
		}
		*/
		
	}
	
	public void LogIn()
	{
		dataLoader.LogIn();
	}
	
	public void SignUp()
	{
		dataLoader.SignUp();
	}
	
	public void Reset()
	{
		dataLoader.Reset();
	}
	
	public void EnterLogInScreen()
	{
		dataLoader.EnterLogInScreen();
	}
	
	public void ExitLogInScreen()
	{
		dataLoader.ExitLogInScreen();
	}
	
	public void EnterSignUp()
	{
		dataLoader.EnterSignUpScreen();
	}
	
	public void ExitSignUp()
	{
		dataLoader.ExitSignUpScreen();
	}
	
	public void EnterResetScreen()
	{
		dataLoader.EnterResetScreen();
	}
	
	public void ExitResetScreen()
	{
		dataLoader.ExitResetScreen();
	}
	
}

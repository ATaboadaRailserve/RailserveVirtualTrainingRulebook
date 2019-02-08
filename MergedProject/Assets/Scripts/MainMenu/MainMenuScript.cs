using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

	int SelectedModule = 0;
	Text descriptionText, welcomeText;
	GameObject descriptionTextObject, modulePanel,CameraHolderOne,CameraHolderTwo,CameraHolderThree,moduleDescriptionPanel, welcomeTextObj, dataLoaderObject;
	DataLoader dataLoader;
	int cameraPosition = 0;
	bool animatingCamera = true;
	Animator animator;
	float speed = 30.0f;
	// Use this for initialization
	void Start () 
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		
		animator = this.GetComponent<Animator>();
		welcomeTextObj = GameObject.Find("WelcomeText");
		welcomeText = welcomeTextObj.GetComponent<Text>();
		
		dataLoaderObject = GameObject.Find("DataLoadObject");
		dataLoader = dataLoaderObject.GetComponent<DataLoader>();
		
		descriptionTextObject = GameObject.Find("ModuleDescription");
		descriptionText = descriptionTextObject.GetComponent<Text>();
		
		modulePanel = GameObject.Find("ModulePanel");
		modulePanel.SetActive(false);
		
		moduleDescriptionPanel = GameObject.Find("ModuleDescriptionPanel");
		moduleDescriptionPanel.SetActive(false);
		
		CameraHolderOne = GameObject.Find("CameraHolderOne");
		CameraHolderTwo = GameObject.Find("CameraHolderTwo");
		CameraHolderThree = GameObject.Find("CameraHolderThree");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(animatingCamera == false)
		{
			animator.enabled = false;
		}
		else
		{
			animator.enabled = true;
		}
		if(cameraPosition == 0 && animatingCamera == false)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position,CameraHolderOne.transform.position,speed*Time.deltaTime);
		}
		if(cameraPosition == 1 && animatingCamera == false)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position,CameraHolderTwo.transform.position,speed*Time.deltaTime);
		}
		if(cameraPosition == 2 && animatingCamera == false)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position,CameraHolderThree.transform.position,speed*Time.deltaTime);
		}
		
		if(welcomeTextObj.activeSelf == true)
		{
            if(dataLoader.onlineMode)
			    welcomeText.text = "Welcome, " + dataLoader.CurrentUser.FirstName + " " + dataLoader.CurrentUser.LastName;
            else
                welcomeText.text = "Welcome, " + dataLoader.CurrentUser.Username;
        }
		
	}
	
	public void LogOut()
	{
		dataLoader.LogOut();
	}
	
	public void GoToDemo()
	{
		Application.LoadLevel(2);
	}
	
	public void SelectSectionOne()
	{
		//descriptionText.text = "Section One";
	}
	
	public void SelectModuleOne()
	{
		descriptionText.text = "Module One: Saftey\n\n*Module Description Here*";
		SelectedModule = 1;
	}
	
	public void SelectModuleTwo()
	{
		descriptionText.text = "Module Two: Incident Reporting\n\n*Module Description Here*";
		SelectedModule = 2;
	}
	
	public void SelectModuleThree()
	{
		descriptionText.text = "Module Three: Take Two\n\n*Module Description Here*";
		SelectedModule = 3;
	}
	
	public void SelectModuleFour()
	{
		descriptionText.text = "Module Four: Speed\n\n*Module Description Here*";
		SelectedModule = 4;
	}
	
	public void SelectSectionTwo()
	{
		//descriptionText.text = "Section Two";
	}
	
	public void SelectModuleFive()
	{
		descriptionText.text = "Module Five: Working On Tracks\n\n*Module Description Here*";
		SelectedModule = 5;
	}
	
		public void SelectModuleSix()
	{
		descriptionText.text = "Module Six: Switches\n\n*Module Description Here*";
		SelectedModule = 6;
	}
	
		public void SelectModuleSeven()
	{
		descriptionText.text = "Module Seven: Equipment Protection\n\n*Module Description Here*";
		SelectedModule = 7;
	}
	
		public void SelectModuleEight()
	{
		descriptionText.text = "Module Eight: Flags\n\n*Module Description Here*";
		SelectedModule = 8;
	}
	
	public void SelectSectionThree()
	{
		//descriptionText.text = "Section Three";
	}
	
	public void SelectModuleNine()
	{
		descriptionText.text = "Module Nine: Loading Racks\n\n*Module Description Here*";
		SelectedModule = 9;
	}
	
	public void SelectModuleTen()
	{
		descriptionText.text = "Module Ten: Clearance Marks\n\n*Module Description Here*";
		SelectedModule = 10;
	}
	
	public void SelectModuleEleven()
	{
		descriptionText.text = "Module Eleven: Hazmat\n\n*Module Description Here*";
		SelectedModule = 11;
	}
	
	public void SelectModuleTwelve()
	{
		descriptionText.text = "Module Twelve: Equipment Subsections\n\n*Module Description Here*";
		SelectedModule = 12;
	}
	
	public void SelectSectionFour()
	{
		//descriptionText.text = "Section Four";
	}
	
	public void SelectModuleThirteen()
	{
		descriptionText.text = "Module Thirteen: Communication Signal\n\n*Module Description Here*";
		SelectedModule = 13;
	}
	
	public void SelectModuleFourteen()
	{
		descriptionText.text = "Module Fourteen: Train Movement\n\n*Module Description Here*";
		SelectedModule = 14;
	}
	
	public void SelectModuleFifteen()
	{
		descriptionText.text = "Module Fifteen: Coupling\n\n*Module Description Here*";
		SelectedModule = 15;
	}
	
	public void SelectModuleSixteen()
	{
		descriptionText.text = "Module Sixteen: Locomotive Operations\n\n*Module Description Here*";
		SelectedModule = 16;
	}
	
	public void MoveToCameraPositionOne()
	{
		cameraPosition = 0;
	}
	public void MoveToCameraPositionTwo()
	{
		cameraPosition = 1;
	}
	public void MoveToCameraPositionThree()
	{
		cameraPosition = 2;
	}
	
	public void StartCameraAnimate()
	{
		animatingCamera = true;
	}
	public void StopCameraAnimate()
	{
		animatingCamera = false;
	}
	
	public void LoadScene (string scene) {
		Application.LoadLevel(scene);
	}
}

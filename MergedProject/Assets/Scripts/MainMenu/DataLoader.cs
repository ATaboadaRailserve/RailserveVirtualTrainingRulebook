using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.NetworkInformation;


public class DataLoader : MonoBehaviour 
{
		
    public bool onlineMode = true;
    public GameObject xmlManager;
	[System.Serializable]
	public class User
	{
		public string userID;
		public string FirstName;
		public string LastName;
        public int Gender;
		public string Email;
		public string Username;
		public string Password;
		public string userType;
        public string sessionID;
        public int trainingPorcedure;
        public string factor;
		
		public User(string ID, string FN, string LN, int GN, string EM, string UN, string PW, string UT, string ssID, int tStat, string FCT)
		{
			userID = ID;
			Username = UN;
			Password = PW;
			FirstName = FN;
			LastName = LN;
            Gender = GN;
            userType = UT;
			Email = EM;
            sessionID = ssID;
            trainingPorcedure = tStat;
            factor = FCT;
		}
		
		public User()
		{
			Username = "";
			Password = "";
			FirstName = "";
			LastName = "";
            Gender = 3;
			userType = "";
			Email = "";
            sessionID = "";
            trainingPorcedure = 0;
            factor = "";
		}
		
	}
	
	Text outputText;
	GameObject loginHoldScreen;
	GameObject loginObj, signupObj;
	
	Text usernameLogInText, passwordLogInText, signUpUserNameText, signUpPasswordText, signUpFirstNameText, signUpLastNameText, signUpEmailText, emailResetText;
	GameObject usernameLogInObj, passwordLogInObj,signUpUserNameObj,signUpPasswordObj, signUpVerifyPasswordObj, signUpFirstNameObj, signUpLastNameObj, signUpEmailObj, signUpVerifyEmailObj, emailResetObj;
	InputField usernameLoginInputField, passwordLoginInputField, usernameSignUpInputField, passwordSignUpInputField, passwordSignUpVerifyInputField;
    InputField signUpFirstNameField, signUpLastNameField, signUpEmailField, signUpVerifyEmailField;
    public User CurrentUser = new User();
	public bool initialize;
	private bool alreadyInitialized;
	string formNick = "";
	string formPassword = "";
	string formText = "";
	
	string formSignUpUsername = "";
	int formSignUpUserID;
	int formSignUpLocationID;
	string formSignUpPassword = "";
	string formSignUpVerifyPassword = "";
	string formSignUpFirstName = "";
	string formSignUpLastName = "";
	string formSignUpEmail ="";
	string formSignUpVerifyEmail ="";
	int formSignUpUserType;
	
	string formEmailReset ="";

	string URL = "https://railservetraining.biz/UserData.php";
	string signUpURL = "https://railservetraining.biz/InsertUser.php";
	string resetURL = "https://railservetraining.biz/sentnewpassword.php";
	string checkMachineStatusURL = "https://railservetraining.biz/machineStatus.php";
    
    bool loggingIn = false;
	bool signingUp = false;
	bool resetting = false;
	
	public int procedureStatus;
    public int gender;
    public string gameFactors;
	public string[] targetScenes;
	
	public string uberCompletions;
	public Retrain retrain;


	//Check if this machine is updated
	string machineID = "";
	bool machineUpdated = false;

	private string GetMacAddress()
	{
		string macAddresses = string.Empty;

		foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
		{
			if (nic.OperationalStatus == OperationalStatus.Up)
			{
				macAddresses += nic.GetPhysicalAddress().ToString();
				break;
			}
		}

		return macAddresses;
	}


	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(this.gameObject);
		if (GameObject.FindWithTag("OutputText") && GameObject.FindWithTag("OutputText").GetComponent<Text>())
			outputText = GameObject.FindWithTag("OutputText").GetComponent<Text>();
		
		loginHoldScreen = GameObject.FindWithTag("LoginHoldScreen");
		loginHoldScreen.SetActive(false);
		
		loginObj = GameObject.FindWithTag("LogInPanel");
		signupObj = GameObject.FindWithTag("SignUpPanel");
		loginObj.SetActive(false);
		signupObj.SetActive(false);
		alreadyInitialized = true;

        xmlManager = GameObject.FindGameObjectWithTag("XMLManager");

    }

	public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
	    bool isOk = true;
	    // If there are errors in the certificate chain,
	    // look at each error to determine the cause.
	    if (sslPolicyErrors != SslPolicyErrors.None) {
	        for (int i=0; i<chain.ChainStatus.Length; i++) {
	            if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
	                continue;
	            }
	            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
	            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
	            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
	            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
	            bool chainIsValid = chain.Build ((X509Certificate2)certificate);
	            if (!chainIsValid) {
	                isOk = false;
	                break;
	            }
	        }
	    }
	    return isOk;
	}

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    string GetDataValue(string data, string index)
	{
		//print (data + " | " + index);
		string value = data.Substring(data.IndexOf(index)+index.Length);
		value = value.Remove(value.IndexOf("|"));
		return value;
	}

    string GetUberCompletions(string data)
	{
		//print (data + " | " + index);
		string value = data.Substring(data.IndexOf("Uber Completions:")+("Uber Completions:").Length);
		return value;
	}
	
	
	// Update is called once per frame
	void Update () {
		if (initialize && !alreadyInitialized && Application.loadedLevelName == "LogIn") {
			outputText = GameObject.Find("OutputText").GetComponent<Text> ();
			loginHoldScreen = GameObject.Find("LoginHoldScreen");
			loginHoldScreen.SetActive(false);
			loginObj = GameObject.FindWithTag("LogInPanel");
			signupObj = GameObject.FindWithTag("SignUpPanel");
			loginObj.SetActive(false);
			signupObj.SetActive(false);
			loggingIn = false;
			signingUp = false;
			resetting = false;
			initialize = false;
			alreadyInitialized = true;
		} else if (Application.loadedLevelName != "LogIn") {
			alreadyInitialized = false;
		}
		
		//if (xmlManager == null && GameObject.FindGameObjectWithTag("XMLManager")) {
		//	xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
		//}
		
        //Debug.Log(System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond.ToString());
		if(Application.loadedLevelName == "LogIn" && loggingIn == true)
		{
			//usernameLogInObj = GameObject.Find("UsernameLogInText");
			//passwordLogInObj = GameObject.Find("PasswordLogInText");
			usernameLogInObj = GameObject.Find("SignUpUserField");
			passwordLogInObj = GameObject.Find("SignUpPWField");
			//usernameLogInText = usernameLogInObj.GetComponent<Text>();
			//passwordLogInText = passwordLogInObj.GetComponent<Text>();
			if (usernameLogInObj != null && usernameLogInObj.GetComponent<InputField> () != null)
				usernameLoginInputField = usernameLogInObj.GetComponent<InputField> ();
			if (passwordLogInObj != null && passwordLogInObj.GetComponent<InputField> () != null)
				passwordLoginInputField = passwordLogInObj.GetComponent<InputField> ();
		}
		
		if(Application.loadedLevelName == "LogIn" && signingUp == true)
		{
			signUpUserNameObj = GameObject.Find("SignUpUserField");
			signUpPasswordObj = GameObject.Find("SignUpPWField");
			signUpVerifyPasswordObj = GameObject.Find("SignUpVerifyPWField");
			signUpFirstNameObj = GameObject.Find("SignUpFirstNameField");
			signUpLastNameObj = GameObject.Find("SignUpLastNameField");
			signUpEmailObj = GameObject.Find("SignUpEmailField");
			signUpVerifyEmailObj = GameObject.Find("SignUpVerifyEmailField");


            //signUpUserNameText = signUpUserNameObj.GetComponent<Text>();
            //signUpPasswordText = signUpPasswordObj.GetComponent<Text>();

            //signUpFirstNameText = signUpFirstNameObj.GetComponent<Text>();
            //signUpLastNameText = signUpLastNameObj.GetComponent<Text>();
            //signUpEmailText = signUpEmailObj.GetComponent<Text>();

            usernameSignUpInputField = signUpUserNameObj.GetComponent<InputField> ();
			passwordSignUpInputField = signUpPasswordObj.GetComponent<InputField> ();
			passwordSignUpVerifyInputField = signUpVerifyPasswordObj.GetComponent<InputField> ();

            signUpFirstNameField = signUpFirstNameObj.GetComponent<InputField>();
            signUpLastNameField = signUpLastNameObj.GetComponent<InputField>();
            signUpEmailField = signUpEmailObj.GetComponent<InputField>();
            signUpVerifyEmailField = signUpVerifyEmailObj.GetComponent<InputField>();

        }
		
		if(Application.loadedLevelName == "LogIn" && resetting == true) //changes
		{
			emailResetObj = GameObject.Find("emailResetText");
			
			emailResetText = emailResetObj.GetComponent<Text>();
			
		}
		
	
	}

    IEnumerator SyncXmlToDB()
    {
		//Check if machine is updated or not
		machineID = GetMacAddress();
		//Debug.Log ("MAC Address: " + machineID);

		WWWForm form = new WWWForm();
		if(machineID == "")
			yield break;

		form.AddField("myform_MAC", machineID);
		form.AddField ("myform_status", 1);
		WWW w = new WWW(checkMachineStatusURL,form);
		yield return w;
		formText = w.data;
		Debug.Log("Got machine update status info: " + w.data);

		//w.data = "1" (string) means this machine has been updated on the new server
		//w.data != "1" means this machine has not been updated on the new server
		//Please comment this if out if you don't want recheck all of the local files. 
		if (!w.data.Equals ("1")) {
			//Re-Sync the local xml files for every one 
			//remove all of the _s from the files
			DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo (Application.dataPath + "/XML/");
			FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles ("*.xml", SearchOption.AllDirectories);

			if (filesInDir.Length > 0) {
				//Sort the file reservely, so the most recent date will be the first one
				Array.Reverse (filesInDir);

				//Debug.Log(filesInDirName[filesInDirName.Length - 1]);
				foreach (FileInfo fileInfo in filesInDir) {
					string filePath = fileInfo.FullName;
					//Debug.Log ("Checking " + filePath);
					if (filePath.Contains ("_s.xml")) {
						string newFilename = filePath.Split ('.') [0].Substring (0, filePath.Split ('.') [0].Length - 2) + ".xml";
						//Debug.Log (newFilename);
						System.IO.File.Move (filePath, newFilename);
						filePath = newFilename;
					}
						
					Debug.Log ("Uploading " + filePath);
					System.Net.ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
					System.Net.WebClient Client = new System.Net.WebClient ();
					Client.Headers.Add ("Content-Type", "binary/octet-stream");

					//byte[] result = Client.UploadFile("http://127.0.0.1/railserve/upload.php", "POST", fileInfo.ToString());
					byte[] result;
					result = Client.UploadFile ("https://railservetraining.biz/upload_first.php", "POST", filePath);

					String s = System.Text.Encoding.UTF8.GetString (result, 0, result.Length);

					Debug.Log (s);

					string[] filesInDirName = filePath.Split ('\\');

					//check all previous dates file names to end with _s.xml, except the current date one
					if (s.Contains ("uploaded successfully. File is valid, and was successfully moved.")) {
						//Change file name after sync
						string newFilename = filePath.Split ('.') [0] + "_s.xml";
						//Debug.Log (newFilename);
						System.IO.File.Move (filePath, newFilename);
					}

					yield return null;
				}

				DateTime now = DateTime.Now;

				string individualRecordFolderName = CreateMD5 (formNick.ToLower ());
				string individualRecordFileName = individualRecordFolderName + "_" + String.Format ("{0:yyyyMMdd}", now);

				bool exists = System.IO.Directory.Exists (Application.dataPath + "/XML/" + individualRecordFolderName + "_s.xml");
				if(exists)
					System.IO.File.Move (Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName + "_s.xml", Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName + ".xml");

			}
		} else {

			//Sync all the filenames without _s at the end
			DateTime now = DateTime.Now;

			string individualRecordFolderName = CreateMD5 (formNick.ToLower ());
			string individualRecordFileName = individualRecordFolderName + "_" + String.Format ("{0:yyyyMMdd}", now) + ".xml";

			bool exists = System.IO.Directory.Exists (Application.dataPath + "/XML/" + individualRecordFolderName);

			if (exists) {
				DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo (Application.dataPath + "/XML/" + individualRecordFolderName + "/");
				FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles ("*" + individualRecordFolderName + "*.xml");

				bool skipFirst = false;
				if (File.Exists (Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName)) {
					skipFirst = true;
				}

				if (filesInDir.Length > 0) {
					//Sort the file reservely, so the most recent date will be the first one
					Array.Reverse (filesInDir);

					//Debug.Log(filesInDirName[filesInDirName.Length - 1]);
					foreach (FileInfo fileInfo in filesInDir) {
						string filePath = fileInfo.FullName;
						Debug.Log ("Checking " + filePath);

						//Assume after sorted, any file after *_s.xml should be all _s.xml. 
						if (filePath.Contains ("_s.xml")) {
							break;
							//if wants to double check if there were more earlier files not uploaded yet
							//Continue; //instead of break; it will check all files
						}

						Debug.Log ("Uploading " + filePath);
						//System.Diagnostics.Process.Start("mozroots","--import --quiet");
						System.Net.ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
						//System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

						//System.Net.ServicePointManager.ServerCertificateValidationCallback +=
						//	delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
						//		System.Security.Cryptography.X509Certificates.X509Chain chain,
						//		System.Net.Security.SslPolicyErrors sslPolicyErrors)
						//{
						//	return true; // **** Always accept
						//};

						System.Net.WebClient Client = new System.Net.WebClient ();
						Client.Headers.Add ("Content-Type", "binary/octet-stream");

						//byte[] result = Client.UploadFile("http://127.0.0.1/railserve/upload.php", "POST", fileInfo.ToString());
						byte[] result;
						if (skipFirst)
							result = Client.UploadFile ("https://railservetraining.biz/upload_first.php", "POST", filePath);
						else
							result = Client.UploadFile ("https://railservetraining.biz/upload.php", "POST", filePath);

						String s = System.Text.Encoding.UTF8.GetString (result, 0, result.Length);

						Debug.Log (s);

						//Skip the current date file
						if (skipFirst) {
							skipFirst = false;
							continue;
						}


						string[] filesInDirName = filePath.Split ('\\');

						//check all previous dates file names to end with _s.xml, except the current date one
						if (s.Contains ("uploaded successfully. File is valid, and was successfully moved.") && !filePath.Contains (individualRecordFileName)) {
							//Change file name after sync

							individualRecordFileName = filesInDirName [filesInDirName.Length - 1].Split ('.') [0];
							System.IO.File.Move (Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName + ".xml", Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName + "_s.xml");
						}

						yield return null;
					}
				}
			}
		}
    }
	
    public IEnumerator LogInFunction()
	{
		loginHoldScreen.SetActive(true);
		loginObj.SetActive(false);
		
		//formNick = usernameLogInText.text;
		//formPassword = passwordLogInText.text;
		formNick = usernameLoginInputField.text;
		formNick.Replace("\\n","");
		formPassword = passwordLoginInputField.text;
		formPassword.Replace("\\n","");
		
		if(formNick == "" || formPassword == "")
			yield break;
		
		Debug.Log("Creating Form...");
		WWWForm form = new WWWForm();
		Debug.Log("Form Created, Adding Fields...");
		form.AddField("myform_nick",formNick);
		form.AddField("myform_pass",formPassword);
		Debug.Log("Fields Added");
		
		Debug.Log("Opening WWW...");
		WWW w = new WWW(URL,form);
		float timer = 0;
		bool failed = false;
		
		while(!w.isDone)
		{
			if(timer > 10000){ failed = true; break; }
			timer += Time.deltaTime;
			yield return null;
		}
		if(failed)
		{
			w.Dispose();
			onlineMode = false;
            Debug.Log("timeout exception: Did not connect to server");
		    outputText.text = "timeout exception: Did not connect to server";
            Debug.Log("Offline Mode");
            procedureStatus = 0;
            gender = 3;
            gameFactors = "15-0, 0; 21-0; 28-0, 0; 36-0; 43-0, 0";

            //Check if any existed local data record 
            DateTime now = DateTime.Now;

            string individualRecordFolderName = CreateMD5(formNick.ToLower());
            string individualRecordFileName = individualRecordFolderName + "_" + String.Format("{0:yyyyMMdd}", now) + ".xml";

            bool exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);

            if (exists)
            {
                if (File.Exists(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName))
                {
                    Debug.Log("The file " + individualRecordFileName + " exists.");
                    //Load existed XML
                    xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                    xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

                    //Get the first record, which is also the most recent one
                    procedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
                    gender = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gender;
                    gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
                    //Debug.Log(gameFactors);
                }
                else
                {
                    Debug.Log("The file " + individualRecordFileName + " not exists.");
                    DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Application.dataPath + "/XML/" + individualRecordFolderName + "/");
                    FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + individualRecordFolderName + "*.xml");

                    if (filesInDir.Length > 0)
                    {
                        Array.Reverse(filesInDir);
                        //foreach (FileInfo fileInfo in filesInDir)
                        //{
                        //    Debug.Log(fileInfo);
                        //}
                        string[] filesInDirName = filesInDir[0].ToString().Split('\\');
                        //Debug.Log(filesInDirName[filesInDirName.Length - 1]);

                        //Load existed XML
                        individualRecordFileName = filesInDirName[filesInDirName.Length - 1];
                        xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                        xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

                        //Get the first record, which is also the most recent one
                        procedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
                        gender = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gender;
                        gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
                    }
                }
            }

            UserOffLineLogin(formNick.ToLower());
			
			SceneManager.LoadScene(targetScenes[(int)Mathf.Clamp(procedureStatus, 0, targetScenes.Length-1)]);
		}
		else
		{
			yield return w;
		}
		//WWW w = new WWW(URL,form);
		//yield return w;
		
		// Rongkai needs to review this bit to have XML load in based on username in the event of Offline Mode.
		// Rongkai needs to review this bit to have XML load in based on username in the event of Offline Mode.
		// Rongkai needs to review this bit to have XML load in based on username in the event of Offline Mode.
		// Rongkai needs to review this bit to have XML load in based on username in the event of Offline Mode.
		// Rongkai needs to review this bit to have XML load in based on username in the event of Offline Mode.
		
		if(w.error != null)
		{
            onlineMode = false;
            Debug.Log(w.error);
		    outputText.text = w.error;
            Debug.Log("Offline Mode");
            procedureStatus = 0;
            gender = 3;
            gameFactors = "15-0, 0; 21-0; 28-0, 0; 36-0; 43-0, 0";

            //Check if any existed local data record 
            DateTime now = DateTime.Now;

            string individualRecordFolderName = CreateMD5(formNick.ToLower());
            string individualRecordFileName = individualRecordFolderName + "_" + String.Format("{0:yyyyMMdd}", now) + ".xml";

            bool exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);

            if (exists)
            {
                if (File.Exists(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName))
                {
                    Debug.Log("The file " + individualRecordFileName + " exists.");
                    //Load existed XML
                    xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                    xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

                    //Get the first record, which is also the most recent one
                    procedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
                    gender = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gender;
                    gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
                    //Debug.Log(gameFactors);
                }
                else
                {
                    Debug.Log("The file " + individualRecordFileName + " not exists.");
                    DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Application.dataPath + "/XML/" + individualRecordFolderName + "/");
                    FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + individualRecordFolderName + "*.xml");

                    if (filesInDir.Length > 0)
                    {
                        Array.Reverse(filesInDir);
                        //foreach (FileInfo fileInfo in filesInDir)
                        //{
                        //    Debug.Log(fileInfo);
                        //}
                        string[] filesInDirName = filesInDir[0].ToString().Split('\\');
                        //Debug.Log(filesInDirName[filesInDirName.Length - 1]);

                        //Load existed XML
                        individualRecordFileName = filesInDirName[filesInDirName.Length - 1];
                        xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                        xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

                        //Get the first record, which is also the most recent one
                        procedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
                        gender = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gender;
                        gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
                    }
                }
            }

            UserOffLineLogin(formNick.ToLower());
			
			SceneManager.LoadScene(targetScenes[(int)Mathf.Clamp(procedureStatus, 0, targetScenes.Length-1)]);
        }
		else
		{
			Debug.Log("Test Ok");

            //SyncXML to Database
            StartCoroutine(SyncXmlToDB());

            w = new WWW(URL, form);
            yield return w;
            formText = w.data;
            Debug.Log("Got this user -- " + w.data);
            //CurrentUsername = GetDataValue(w.data, "Username:")
            //User
            if (w.data.Split('|')[0].Split(':')[0] == "Successfully connected!UserID")
            {
                UserHandOff(w.data);
                procedureStatus = CurrentUser.trainingPorcedure;
                gender = CurrentUser.Gender;
                gameFactors = CurrentUser.factor;
                w.Dispose();
                //procedureStatus = 0;
                //gameFactors = "15-0, 0; 28-0, 0; 43-0, 0";

                //Check if any existed local data record 
                DateTime now = DateTime.Now;

                string individualRecordFolderName = CreateMD5(formNick.ToLower());
                string individualRecordFileName = individualRecordFolderName + "_" + String.Format("{0:yyyyMMdd}", now) + ".xml";

                bool exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);
				
				int xmlProcedureStatus = procedureStatus;

                if (exists)
                {
                    if (File.Exists(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName))
                    {
                        Debug.Log("The file " + individualRecordFileName + " exists.");
                        //Load existed XML
                        xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                        xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

                        //Get the first record, which is also the most recent one
                        xmlProcedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
						if(xmlProcedureStatus > procedureStatus)
						{	procedureStatus = xmlProcedureStatus;
							gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
						}
                        //Debug.Log(gameFactors);
                    }
                    else
                    {
                        Debug.Log("The file " + individualRecordFileName + " not exists.");
                        
                        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Application.dataPath + "/XML/" + individualRecordFolderName + "/");
                        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + individualRecordFolderName + "*.xml");

                        if (filesInDir.Length > 0)
                        {
                            Array.Reverse(filesInDir);
                            //foreach (FileInfo fileInfo in filesInDir)
                            //{
                            //    Debug.Log(fileInfo);
                            //}
                            string[] filesInDirName = filesInDir[0].ToString().Split('\\');
                            //Debug.Log(filesInDirName[filesInDirName.Length - 1]);

                            //Load existed XML
                            individualRecordFileName = filesInDirName[filesInDirName.Length - 1];
                            xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                            xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

                            //Get the first record, which is also the most recent one
                            xmlProcedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
							if(xmlProcedureStatus > procedureStatus)
							{
								procedureStatus = xmlProcedureStatus;
								gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
							}
                        }
                    }
                }

                SceneManager.LoadScene(targetScenes[(int)Mathf.Clamp(procedureStatus, 0, targetScenes.Length - 1)]);

                //Check if any existed local data record 
                Debug.Log("Retrieving Data from Local");
                //DateTime now = DateTime.Now;

                //string individualRecordFolderName = CreateMD5(formNick.ToLower());
                //string individualRecordFileName = individualRecordFolderName + "_" + String.Format("{0:yyyyMMdd}", now) + ".xml";

                //bool exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);

                if (exists)
                {
                    if (File.Exists(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName))
                    {
                        Debug.Log("The file " + individualRecordFileName + " exists.");
                        //Load existed XML
                        xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
                        xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);
                    }
                }
            }
            else
            {
                print("||||||||||" + w.data.Split('|')[0]);
                if (w.data.Split('|')[0] == "Successfully connected!Password is wrong") {
					loginHoldScreen.SetActive(false);
					loginObj.SetActive(true);
					outputText.text = "Wrong username or password";
				} else  if (w.data.Split('|')[0].Split('!')[1].Substring(0,5) == "Error") {
					loginHoldScreen.SetActive(false);
					loginObj.SetActive(true);
					outputText.text = "Username does not exist";
				} else {
					onlineMode = false;
					Debug.Log(w.error);
					outputText.text = w.error;
					Debug.Log("Offline Mode");
					procedureStatus = 0;
					gender = 3;
					gameFactors = "15-0, 0; 21-0; 28-0, 0; 36-0; 43-0, 0";

					//Check if any existed local data record 
					DateTime now = DateTime.Now;

					string individualRecordFolderName = CreateMD5(formNick.ToLower());
					string individualRecordFileName = individualRecordFolderName + "_" + String.Format("{0:yyyyMMdd}", now) + ".xml";

					bool exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);

					if (exists)
					{
						if (File.Exists(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName))
						{
							Debug.Log("The file " + individualRecordFileName + " exists.");
							//Load existed XML
							xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
							xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

							//Get the first record, which is also the most recent one
							procedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
							gender = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gender;
							gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
							//Debug.Log(gameFactors);
						}
						else
						{
							Debug.Log("The file " + individualRecordFileName + " not exists.");
							DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Application.dataPath + "/XML/" + individualRecordFolderName + "/");
							FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + individualRecordFolderName + "*.xml");

							if (filesInDir.Length > 0)
							{
								Array.Reverse(filesInDir);
								//foreach (FileInfo fileInfo in filesInDir)
								//{
								//    Debug.Log(fileInfo);
								//}
								string[] filesInDirName = filesInDir[0].ToString().Split('\\');
								//Debug.Log(filesInDirName[filesInDirName.Length - 1]);

								//Load existed XML
								individualRecordFileName = filesInDirName[filesInDirName.Length - 1];
								xmlManager = GameObject.FindGameObjectWithTag("XMLManager");
								xmlManager.GetComponentInChildren<LoadFromXML>().LoadOfflineXML(individualRecordFolderName + "/" + individualRecordFileName);

								//Get the first record, which is also the most recent one
								procedureStatus = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].procedureStatus;
								gender = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gender;
								gameFactors = xmlManager.GetComponentInChildren<LoadFromXML>().xmlInfo[0].gameFactor;
							}
						}
					}

					UserOffLineLogin(formNick.ToLower());
					
					SceneManager.LoadScene(targetScenes[(int)Mathf.Clamp(procedureStatus, 0, targetScenes.Length-1)]);
				}
            }
		}
		
		formNick = "";
		formPassword = "";
		
		
	}
	
	public IEnumerator SignUpFunction()
	{
		//formSignUpUsername = signUpUserNameText.text;
		//formSignUpPassword = signUpPasswordText.text;
		formSignUpUsername = usernameSignUpInputField.text;
		formSignUpPassword = passwordSignUpInputField.text;
		formSignUpVerifyPassword = passwordSignUpVerifyInputField.text;
		formSignUpFirstName = signUpFirstNameField.text;
		formSignUpLastName = signUpLastNameField.text;
		formSignUpEmail = signUpEmailField.text;
		formSignUpVerifyEmail = signUpVerifyEmailField.text;
        formSignUpLocationID = 1;
		
		bool succeed = true;
		if (formSignUpPassword != formSignUpVerifyPassword) {
			outputText.text = "Passwords do not match!";
		} else if (formSignUpEmail != formSignUpVerifyEmail) {
			outputText.text = "Emails do not match!";
		} else {
			
			/*
				1: First Time User
				2: User
				100: Admin
			*/
			formSignUpUserType = 1;
			
			Debug.Log("Creating Form...");
			WWWForm form = new WWWForm();
			Debug.Log("Form Created, Adding Fields...");
			
			form.AddField("locationid",formSignUpLocationID);
			form.AddField("firstname",formSignUpFirstName);
			form.AddField("lastname",formSignUpLastName);
			form.AddField("username",formSignUpUsername);
			form.AddField("password",formSignUpPassword);
			form.AddField("email",formSignUpEmail);
			form.AddField("usertype", formSignUpUserType);
			Debug.Log("Fields Added");
			
			bool checkName = true;
			Debug.Log("Opening WWW...");
			WWW w = new WWW(signUpURL,form);
			yield return w;
			if(w.error != null)
			{
				Debug.Log(w.error);
			}
			else
			{
				if (w.data.Contains("The User Name has been used")) {
					outputText.text = "That username is taken.";
					checkName = false;
				} else if (w.data.Contains("The Email has been registered")) {
					outputText.text = "That email is in use.";
					checkName = false;
				} else {
					Debug.Log("Inserted Data!");
					Debug.Log(w.data);
				}
				w.Dispose();
			}
			
			formSignUpUsername = "";
			formSignUpPassword = "";
			formSignUpFirstName = "";
			formSignUpLastName = "";
			formSignUpEmail = "";
			
			if (checkName) {
				outputText.text = "Account created, you may now log in.";
				ExitSignUpScreen();
				EnterLogInScreen();
				loginObj.SetActive(true);
				signupObj.SetActive(false);
			}
		}
    }

	
	public IEnumerator ResetFunction() //changes
	{
		
		formEmailReset = emailResetText.text;
		
		
		Debug.Log("Creating Form...");
		WWWForm form = new WWWForm();
		Debug.Log("Form Created, Adding Fields...");
		
		
		form.AddField("remail",formEmailReset);
		form.AddField("username",formSignUpUsername);
		
		
		Debug.Log("Opening WWW...");
		WWW w = new WWW(resetURL,form);
		yield return w;
		if(w.error != null)
		{
			Debug.Log(w.error);
		}
		else
		{
			Debug.Log("email sent!");
			Debug.Log(w.data);
			w.Dispose();
		}
		
		
		formEmailReset = "";
	}

    public void LogIn()
	{
		StartCoroutine(LogInFunction());
	}
	
	public void SignUp()
	{
		StartCoroutine(SignUpFunction());
	}
	
	public void Reset() //changes
	{
		StartCoroutine(ResetFunction());
	}

    public void LogOut()
	{
		//CurrentUsername = "";
		Application.LoadLevel("LogIn");
	}
	
	public void UserHandOff(string u)
	{
		//CurrentUser = u;
		string userInfo = u;
		Debug.Log(userInfo);
        string sessionID = System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond.ToString();
		uberCompletions = GetUberCompletions(userInfo);
		retrain.CheckForRetrain(uberCompletions);
        CurrentUser = new User(GetDataValue(userInfo, "UserID:"), GetDataValue(userInfo, "First Name:"),GetDataValue(userInfo, "Last Name:"), int.Parse(GetDataValue(userInfo, "Gender:")), GetDataValue(userInfo, "Email:"),GetDataValue(userInfo, "Username:").ToLower(),GetDataValue(userInfo, "Password:"),GetDataValue(userInfo, "UserType:"), sessionID, int.Parse(GetDataValue(userInfo, "Training Status: ")), GetDataValue(userInfo, "Game Factors:"));
		Debug.Log("Current Username: " + CurrentUser.Username + " Current First Name: " + CurrentUser.FirstName + " Current Last Name: " + CurrentUser.LastName);
		
	}

    public void UserOffLineLogin(string u)
    {
        string sessionID = System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond.ToString();
        CurrentUser = new User("Offline", null, null, 2, null, u, null, "0", sessionID, procedureStatus, gameFactors);
        //Debug.Log("Offline Logged in: " + u + " | " + gameFactors);
    }


    public void EnterLogInScreen()
	{
		loggingIn = true;
	}
	public void ExitLogInScreen()
	{
		loggingIn = false;
	}
	
	public void EnterSignUpScreen()
	{
		signingUp = true;
	}
	public void ExitSignUpScreen()
	{
		signingUp = false;
	}
	
	
	public void EnterResetScreen()  //changes
	{
		resetting = true;
	}
	public void ExitResetScreen()//changes
	{
		resetting = false;
	}
	public void GoToMainMenu()
	{
		Application.LoadLevel("Menu");
	}
}

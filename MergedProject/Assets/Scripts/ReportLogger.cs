using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System.Text;
using System.Linq;
using Rewired;

public class ReportLogger : MonoBehaviour {
	
	private FileStream log;
	private string path;
	private string logStuff;
	
	#region Rewired Stuff
	private Player player;
	void Awake() {
		player = ReInput.players.GetPlayer(0); // get the Rewired Player
	}
	#endregion
	
	void Start () {
		Application.logMessageReceived += HandleLog;
		SceneManager.activeSceneChanged += ChangedActiveScene;
		
		path = Application.dataPath + "/logs";
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		AdjustFileCount(path);
		path += "/OutputLog_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".log";
		
		/*
		log = new FileStream(path, System.IO.FileMode.Create);
		log.Dispose();
		*/
		
		if (!File.Exists(path)) {
			// Create a file to write to.
			string initialText = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
			File.AppendAllText(path, initialText, Encoding.UTF8);
		}
	}
	
	void AdjustFileCount(string path) {
		DirectoryInfo info = new DirectoryInfo(path);
		FileInfo[] fileInfo = info.GetFiles();
		List<FileInfo> orderedInfo = fileInfo.OrderBy(x => x.CreationTime).ToList();
		while (orderedInfo.Count > 2) {
			File.Delete(orderedInfo[0].FullName);
			orderedInfo.RemoveAt(0);
		}
	}
	
	void Update () {
		logStuff = "";
		if (player.GetAxis("Vertical") != 0 || player.GetAxis("Horizontal") != 0) {
			if (GameObject.FindWithTag("Player")) {
				logStuff += '\n' + "Position: " + GameObject.FindWithTag("Player").transform.position;
			}
		}
		if (player.GetAxis("LookX") != 0 || player.GetAxis("LookY") != 0) {
			if (GameObject.FindWithTag("Player")) {
				logStuff += '\n' + "Rotation: " + GameObject.FindWithTag("Player").transform.rotation;
			}
		}
		if (player.GetButtonDown("Fire1")) {
			if (GameObject.FindWithTag("Player")) {
				RaycastHit hit;
				Physics.Raycast(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("Player").transform.forward, out hit);
				logStuff += '\n' + "Left clicked object: " + hit.collider.gameObject.name + " at position " + hit.collider.transform.position;
			} else {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 100))
					logStuff += '\n' + "Left clicked object: " + hit.collider.gameObject.name + " at position " + hit.collider.transform.position;
			}
		}
		if (player.GetButtonDown("Fire2")) {
			if (GameObject.FindWithTag("Player")) {
				RaycastHit hit;
				Physics.Raycast(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("Player").transform.forward, out hit);
				logStuff += '\n' + "Right clicked object: " + hit.collider.gameObject.name + " at position " + hit.collider.transform.position;
			} else {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 100))
					logStuff += '\n' + "Right clicked object: " + hit.collider.gameObject.name + " at position " + hit.collider.transform.position;
			}
		}
		if (player.GetButtonDown("Use")) {
			if (GameObject.FindWithTag("Player")) {
				RaycastHit hit;
				Physics.Raycast(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("Player").transform.forward, out hit);
				logStuff += '\n' + "Hit 'use' while aimed at object: " + hit.collider.gameObject.name + " at position " + hit.collider.transform.position;
			} else {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 100))
					logStuff += '\n' + "Hit 'use' while aimed at object: " + hit.collider.gameObject.name + " at position " + hit.collider.transform.position;
			}
		}
		if (player.GetButtonDown("RadioWheel")) {
			if (GameObject.FindWithTag("Player")) {
				logStuff += '\n' + "Opened radio wheel";
			}
		}
		if (player.GetButtonDown("RadioWheel")) {
			if (GameObject.FindWithTag("Player")) {
				logStuff += '\n' + "Opened radio wheel";
			}
		}
		
		if (GetEventSystemRaycastResults().Count != 0) {
			foreach (RaycastResult r in GetEventSystemRaycastResults()) {
				logStuff += '\n' + "UI Raycast Result: " + r.gameObject.name;
			}
		}
		
		if (logStuff != "") {
			WriteLog();
		}
	}
	
	static List<RaycastResult> GetEventSystemRaycastResults() {   
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position =  Input.mousePosition;
		List<RaycastResult> raysastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll( eventData, raysastResults );
		return raysastResults;
	}
	
	void ChangedActiveScene (Scene current, Scene next) {
		WriteLine("Leaving Scene: " + current.name);
		WriteLine("Loading Scene: " + next.name);
		
		if (next.name == "MainMenu") {
			StartCoroutine(SetBugReportInstructions());
		}
	}
	
	IEnumerator SetBugReportInstructions () {
		yield return new WaitForSeconds(0.25f);
		GameObject.FindWithTag("BugReportInstructions").GetComponent<Text>().text = "Report any bugs, crashes, or errors to TrainingSupport@Railserve.biz with a copy of the output log found in " + '\n' + Application.dataPath + "/logs";
	}
	
	void HandleLog (string logString, string stackTrace, LogType type) {
		WriteLine(logString);
	}
	
	void WriteLine (string line) {
		line = "" + '\n' + line;
		if (CompareLastLine(line))
			return;
		
		File.AppendAllText(path, line, Encoding.UTF8);
		
		/*
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(line);
		writer.Close();
		*/
	}
	
	void WriteLog () {
		if (CompareLastLine(logStuff.Split('\n').Last()))
			return;
		
		File.AppendAllText(path, logStuff, Encoding.UTF8);
		
		/*
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(logStuff);
		writer.Close();
		*/
	}
	
	bool CompareLastLine (string line) {
		string lastLine = File.ReadAllLines(path).Last();
		return lastLine.Contains(line) || line.Contains(lastLine);
	}
}
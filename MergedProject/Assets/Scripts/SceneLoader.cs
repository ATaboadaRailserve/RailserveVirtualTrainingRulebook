using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour {
	
	private GameObject messageHolder;
	
	void Start () {
		messageHolder = null;
		messageHolder = GameObject.FindWithTag("MessageHolder");
	}
	
	public void LoadScene (string scene) {
		if (GameObject.FindWithTag("Player"))
			GameObject.FindWithTag("Player").SendMessage("CursorState", true, SendMessageOptions.DontRequireReceiver);
		if (GameObject.FindWithTag("RotatorCamera"))
			GameObject.FindWithTag("RotatorCamera").SendMessage("CursorState", true, SendMessageOptions.DontRequireReceiver);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		
		if (messageHolder != null) {
			StartCoroutine(WaitForPush(scene));
			return;
		}
		
		SceneManager.LoadScene(scene);
		if (scene == "LogIn") {
			GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>().initialize = true;
		}
	}
	
	IEnumerator WaitForPush (string scene) {
		messageHolder.GetComponent<DatabaseMessageHolder>().PushingMessages();
		while (!messageHolder.GetComponent<PushToDB>().finishedPushing) {
			yield return null;
		}
		
		messageHolder.GetComponent<PushToDB>().finishedPushing = false;
		
		SceneManager.LoadScene(scene);
		if (scene == "LogIn") {
			GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>().initialize = true;
		}
	}
}
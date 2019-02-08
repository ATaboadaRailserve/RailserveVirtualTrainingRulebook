using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToMenu : MonoBehaviour {
    DatabaseMessageHolder messageHolder;
    // Use this for initialization
    void Start () {
	    messageHolder = GameObject.FindGameObjectWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>();
    }

    public void LoadMainMenu()
    {
        //This is how to write message and push message
		StartCoroutine(WaitForPush());
    }
	
	IEnumerator WaitForPush () {
        int grade_int = GameObject.FindGameObjectWithTag("Grader").GetComponent<Grader>().grade;
        if (grade_int >= 100)
            messageHolder.moduleFinished = true;
        string grade = grade_int.ToString();
        messageHolder.WriteMessage(grade, 4);
        messageHolder.PushingMessages();
		
		while (!messageHolder.GetComponent<PushToDB>().finishedPushing) {
			yield return null;
		}
		
		messageHolder.GetComponent<PushToDB>().finishedPushing = false;
		SceneManager.LoadScene("Tutorial/FirstStartTutorial");
	}
}

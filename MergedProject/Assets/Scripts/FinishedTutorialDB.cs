using UnityEngine;
using System.Collections;

public class FinishedTutorialDB : MonoBehaviour {

    string finishedURL = "http://rsconnect.biz/FinishedTutorial.php";
    string formText = "";

    // Use this for initialization
    void Start () {
	
	}

    public IEnumerator FinishedTutorialFunction()
    {
        Debug.Log("Creating Form...");
        WWWForm form = new WWWForm();
        int regularUser = 2;
        form.AddField("usertyp", regularUser);

        Debug.Log("Opening WWW...");
        WWW w = new WWW(finishedURL, form);
        yield return w;
        if (w.error != null)
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log("Test Ok");
            formText = w.data;
            Debug.Log("Got this user -- " + w.data);
            if (w.data.Split('|')[0].Split(':')[0] == "Successfully connected!UserID")
            {

                w.Dispose();
            }
            else {
                print(w.data.Split('|')[0]);
            }
        }
    }

    public void FinishedTutorial()
    {
        StartCoroutine(FinishedTutorialFunction());
    }

    // Update is called once per frame
    void Update () {
	
	}
}

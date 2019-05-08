using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PushToDB : MonoBehaviour {
	[HideInInspector]
    public bool finishedPushing = false;
    string insertScoreURL = "http://rsconnect.biz/InsertScore.php";
    string insertEventURL = "http://rsconnect.biz/InsertEvent.php";
    string updateProcedureURL = "http://rsconnect.biz/UpdateProcedure.php";
    string ubercheckpointURL = "http://rsconnect.biz/ComprehensiveCheckpoint.php";
    int messageCount = 0;
    int procedureStatus;
    int moduleIndex = 0;
    char subModuleID = '0';
	//PushToXML xmlPusher;
	
    // Use this for initialization
    void Start () {
        procedureStatus = GetComponent<DatabaseMessageHolder>().procedureStatus;
    }

    IEnumerator PushingToDatabase(List<DatabaseMessageHolder.MessageHolder> messages, string startTime)
    {
        Debug.Log("Creating Form...");
        WWWForm form = new WWWForm();
        Debug.Log("Form Created, Adding Fields...");
        WWW w_grade;
        WWW w_event;
        WWW w_procedure;
		WWW w_ubercheckpoint;
        WWWForm ubersaveform = new WWWForm();

        Debug.Log("Opening WWW...");

        foreach (DatabaseMessageHolder.MessageHolder m in messages)
        {
			if (m.sessionID == "")
				continue;

            // Note to Rongkai - Push to XML here
            form.AddField("userid", m.userID);
            form.AddField("sessionid", m.sessionID);
            //form.AddField("moduleid", m.moduleID + "-" + m.subModuleID);
            form.AddField("moduleid", m.module);
            //form.AddField("starttime", startTime);
            form.AddField("messagetypeid", m.messageTypeID);
			
			//print(m.message);
			
            if (m.message.Split('|')[1].Trim().Equals("Training Started"))
            {
                //Debug.Log(m.message);
                startTime = m.message.Split('|')[0].Trim();
                form.AddField("message", m.message);
            }
            else if (m.messageTypeID.Equals("4"))
            {
				
				// Update the module if a passing grade
				print(m.message.Split('|')[1]);
				//if (Int32.Parse(m.message.Split('|')[1]) == 100)
				//	procedureStatus++;
				
				//GetComponent<DatabaseMessageHolder>().UpdateStatus(procedureStatus);
				procedureStatus = GetComponent<DatabaseMessageHolder>().procedureStatus;
                Debug.Log(procedureStatus);
				Debug.Log("Sending: " + procedureStatus);
				form.AddField("procedureStatus", procedureStatus);
				w_procedure = new WWW(updateProcedureURL, form);
				yield return w_procedure;
				if (w_procedure.error != null)
				{
					Debug.Log(w_procedure.error);
				}
				else
				{
					Debug.Log("score was inserted!");
					Debug.Log(w_procedure.data);
					w_procedure.Dispose();
				}
				
				// Send grade
				form.AddField("finalgrade", m.message.Split('|')[1]);
                form.AddField("starttime", m.message.Split('|')[0].Trim());
				
				w_grade = new WWW(insertScoreURL, form);
                yield return w_grade;
                if (w_grade.error != null)
                {
                    Debug.Log(w_grade.error);
                }
                else
                {
                    Debug.Log("score was inserted!");
                    Debug.Log(w_grade.data);
                    w_grade.Dispose();
                }
				
				// Log message that event occurred
				form.AddField("message", m.message.Split('|')[0].Trim() + " | The final grade is " + m.message.Split('|')[1].Trim());
            }  else if (m.messageTypeID.Equals("8")) {
				// Store the current Uber Module progress
				string point = m.message.Split('|')[1];
				Debug.Log("Sending: " + point);
				ubersaveform.AddField("userid", m.userID);
				ubersaveform.AddField("uberSavePoint", point);
				w_ubercheckpoint = new WWW(ubercheckpointURL, ubersaveform);
				yield return w_ubercheckpoint;
				if (w_ubercheckpoint.error != null)
				{
					Debug.Log(w_ubercheckpoint.error);
				}
				else
				{
					Debug.Log("checkpoint was inserted!");
					Debug.Log(w_ubercheckpoint.data);
					w_ubercheckpoint.Dispose();
				}
            }
            else
            {
                form.AddField("message", m.message);
            }
            
            w_event = new WWW(insertEventURL, form);
            yield return w_event;
            if (w_event.error != null)
            {
                Debug.Log(w_event.error);
            }
            else
            {
                Debug.Log("events were inserted!");
                Debug.Log(w_event.data);
                w_event.Dispose();
                //if (messageCount == (messages.Count-1))
                //    finishedPushing = true;
            }
            messageCount++;
        }
		GetComponent<DatabaseMessageHolder>().messages.Clear();
		finishedPushing = true;
    }

    public void PushToDatabase(List<DatabaseMessageHolder.MessageHolder> messages, string userName,string startTime)
    {
		if (messages.Count == 0) {
			finishedPushing = false;
			return;
		}
		//if(xmlPusher == null)
		//	xmlPusher = GetComponent<PushToXML>();
		//if(xmlPusher != null)
		//	xmlPusher.PushToXMLFile(messages, userName, startTime);
        StartCoroutine(PushingToDatabase(messages, startTime));
    }
}

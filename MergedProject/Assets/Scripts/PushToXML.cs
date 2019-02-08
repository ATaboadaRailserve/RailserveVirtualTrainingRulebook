using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class PushToXML : MonoBehaviour {
    private GameObject xmlController;
    int messageCount = 0;
    int procedureStatus;
    int gender;
    string gameFactors;
    int moduleIndex = 0;
    char subModuleID;
	
    // Use this for initialization
    void Start () {
        xmlController = GameObject.FindWithTag("XMLController");
        procedureStatus = GetComponent<DatabaseMessageHolder>().procedureStatus;
        gender = GetComponent<DatabaseMessageHolder>().gender;
        gameFactors = GetComponent<DatabaseMessageHolder>().gameFactors;
    }

    public void PushToXMLFile(List<DatabaseMessageHolder.MessageHolder> messages, string userName, string startTime)
    {
        int count = 0;
        //Debug.Log("1: " + userName + "; Star Time: " + startTime + "; Message: " + messages);
        foreach (DatabaseMessageHolder.MessageHolder m in messages)
        {
            //Debug.Log("1: " + userName + "; Star Time: " + startTime + "; Message: " + m.message);
            //Debug.Log(procedureStatus);
            // Note to Rongkai - Push to XML here
            XMLInfoStruct.XMLInfo xmlInfo = new XMLInfoStruct.XMLInfo();
            //if (m.message.Split('|')[1].Trim().Equals("Training Started"))
            {
                startTime = m.message.Split('|')[0].Trim();
            }

            procedureStatus = GetComponent<DatabaseMessageHolder>().procedureStatus;
            gender = GetComponent<DatabaseMessageHolder>().gender;
            gameFactors = GetComponent<DatabaseMessageHolder>().gameFactors;
            xmlInfo.username = userName;
            xmlInfo.userid = m.userID;
            xmlInfo.gender = gender;
            xmlInfo.moduleid = m.module;
            xmlInfo.sessionid = m.sessionID;
            xmlInfo.starttime = startTime;
            xmlInfo.messagetypeid = m.messageTypeID;
            xmlInfo.procedureStatus = procedureStatus;
            xmlInfo.gameFactor = gameFactors;
            //Debug.Log(xmlInfo.gameFactor);
            //Update if the user has finished this module
            if (GetComponent<DatabaseMessageHolder>().moduleFinished)		// Needs updating to new Module + SubModule stuff
            {
				/*
                //Checking if the current module is new module. 
                char[] procedureChars = procedureStatus.ToCharArray();
                moduleIndex = Int32.Parse(m.moduleID);
                subModuleID = m.subModuleID;
                //Debug.Log(subModuleID);
                //Debug.Log(moduleIndex + ": " + procedureChars.Length);
				
				if (procedureChars.Length > moduleIndex)
				{
					if (Char.IsNumber(procedureChars[moduleIndex]) || (Char.ToLower(subModuleID) == Char.ToLower(procedureChars[moduleIndex]) && !Char.IsUpper(procedureChars[moduleIndex])) || (Char.ToLower(subModuleID) > Char.ToLower(procedureChars[moduleIndex]))) {
						if (GetComponent<DatabaseMessageHolder>().moduleFinished)
							procedureChars[moduleIndex] = Char.ToUpper(subModuleID);
						else
							procedureChars[moduleIndex] = Char.ToLower(subModuleID);
					}
					procedureStatus = new string(procedureChars);
					//Debug.Log(procedureStatus);
				}
				else
				{
					for (int i = 0; i < moduleIndex - procedureChars.Length; i++)
					{
						procedureStatus += "1"; // THIS IS ONE BECAUSE OF SIGNIFICANT FIGURES!!!!!!!!!! DON'T NOT DO SOMETHING THAT'S NOT ZERO....  Err... Don't do zero.
					}
					if (GetComponent<DatabaseMessageHolder>().moduleFinished)
						procedureStatus += Char.ToUpper(subModuleID).ToString();
					else
						procedureStatus += Char.ToLower(subModuleID).ToString();
					//Debug.Log(procedureStatus);
				}
				//Debug.Log(procedureStatus);
				
				GetComponent<DatabaseMessageHolder>().UpdateStatus(procedureStatus);
				GetComponent<DatabaseMessageHolder>().UpdateModules();
                xmlInfo.procedureStatus = procedureStatus;
				*/
            }

            if (m.messageTypeID.Equals("4"))
            {
                xmlInfo.message = m.message;
            }
            else {
                xmlInfo.message = m.message;
            }

            //Debug.Log(xmlInfo.message);
            xmlController.GetComponent<XML_Saver>().xmlInfoList.Add(xmlInfo);
        } 

        xmlController.GetComponent<XML_Saver>().SaveToFile();
        //xmlController.GetComponent<XML_Saver>().SaveToIndividualFile();
        GetComponent<PushToDB>().finishedPushing = true;
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class WriteToXML : MonoBehaviour {
    public GameObject xmlController;
    public XMLInfoStruct.XMLInfo xmlInfo;
    public List<XMLInfoStruct.XMLInfo> xmlInfoList;
    public string procedureStatus;
    int moduleIndex = 0;
    // Use this for initialization
    void Start () {
        //procedureStatus = "000000";
        //xmlInfoList = new List<XMLInfoStruct.XMLInfo>();
        //xmlInfo = new XMLInfoStruct.XMLInfo();

        //xmlInfo.userid = "17";
        //xmlInfo.moduleid = "1";
        //xmlInfo.sessionid = "61620162333333";
        //xmlInfo.starttime = "6/16/2016 1:36:00 AM ";
        //xmlInfo.messagetypeid = "1";
        //xmlInfo.message = "warning";

        //xmlInfoList.Add(xmlInfo);

        //xmlInfo.userid = "17";
        //xmlInfo.moduleid = "1";
        //xmlInfo.sessionid = "61620162333333";
        //xmlInfo.starttime = "6/16/2016 1:36:00 AM ";
        //xmlInfo.messagetypeid = "2";
        //xmlInfo.message = "credit";

        //xmlInfoList.Add(xmlInfo);

        //xmlInfo.userid = "17";
        //xmlInfo.moduleid = "1";
        //xmlInfo.sessionid = "61620162333333";
        //xmlInfo.starttime = "6/16/2016 1:36:00 AM ";
        //xmlInfo.messagetypeid = "3";
        //xmlInfo.message = "83";

        //xmlInfoList.Add(xmlInfo);

        //xmlController.GetComponent<XML_Saver>().xmlInfoList = xmlInfoList;
        //xmlController.GetComponent<XML_Saver>().SaveToFile();

    }
	
	// Update is called once per frame
	void Update () {
        //char[] procedureChars = procedureStatus.ToCharArray();
        //moduleIndex = Int32.Parse("10");

        //if (procedureChars.Length > moduleIndex)
        //{
        //    procedureChars[moduleIndex] = '1';
        //    procedureStatus = new string(procedureChars);
        //}
        //else
        //{
        //    for (int i = 0; i < moduleIndex-procedureChars.Length-1; i++)
        //    {
        //        procedureStatus += "0";
        //    }
        //    procedureStatus += "1";
        //}
        //Debug.Log(procedureStatus);
    }
}

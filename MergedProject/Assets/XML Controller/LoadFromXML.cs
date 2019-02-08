using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using System.Text;

public class LoadFromXML : XMLInfoStruct {
    private XmlDocument getXML = new XmlDocument();
    public GameObject xmlController;
    string username = "";
    string userid = "";
    string gender = "";
    string moduleid = "";
    string sessionid = "";
    string starttime = "";
    string messagetypeid = "";
    string message = "";
    string procedure = "";
    string gamefactor = "";

    string[] usernames;
    string[] userids;
    string[] genders;
    string[] moduleids;
    string[] sessionids;
    string[] starttimes;
    string[] messagetypeids;
    string[] messages;
    string[] procedures;
    string[] gamefactors;

    public XMLInfo[] xmlInfo;

    // Use this for initialization
    void Start () {
        ////Load the XML
        //xmlController.GetComponent<XML_Loader>().LoadExtXML();

        //getXML = xmlController.GetComponent<XML_Loader>().xmlExt;
        //LoadXMLNodes(getXML);

        ////Extract the data from xml and consider if the XML contains more than one item
        //usernames = ExtractXML(username);
        //userids = ExtractXML(userid);
        //moduleids = ExtractXML(moduleid);
        //sessionids = ExtractXML(sessionid);
        //starttimes = ExtractXML(starttime);
        //messagetypeids = ExtractXML(messagetypeid);
        //messages = ExtractXML(message);
        //procedures = ExtractXML(procedure);

        //string[] tmp_submoduleids = ExtractXML(submoduleid.ToString());
        //for (int i = 0; i < tmp_submoduleids.Length; i++)
        //{
        //    submoduleids[i] = Convert.ToChar(tmp_submoduleids[i]);
        //}

        //xmlInfo = new XMLInfo[usernames.Length];

        //for (int i = 0; i < usernames.Length; i++)
        //{
        //    xmlInfo[i].username = usernames[i]; 
        //    xmlInfo[i].userid = userids[i];
        //    xmlInfo[i].moduleid = moduleids[i];
        //    xmlInfo[i].submoduleid = submoduleids[i];
        //    xmlInfo[i].sessionid = sessionids[i];
        //    xmlInfo[i].starttime = starttimes[i];
        //    xmlInfo[i].messagetypeid = messagetypeids[i];
        //    xmlInfo[i].message = messages[i];
        //    xmlInfo[i].procedureStatus = procedures[i];

        //    //Debug.Log("userid: " + xmlInfo[i].userid);
        //    //Debug.Log("moduleid: " + xmlInfo[i].moduleid);
        //    //Debug.Log("sessionid: " + xmlInfo[i].sessionid);
        //    //Debug.Log("starttime: " + xmlInfo[i].starttime);
        //    //Debug.Log("messagetypeid: " + xmlInfo[i].messagetypeid);
        //    //Debug.Log("message: " + xmlInfo[i].message);
        //}
    }

    public void LoadOfflineXML(string individualFileName)
    {
        //Load the XML
        xmlController.GetComponent<XML_Loader>().LoadExtIndividualXML(individualFileName);

        getXML = xmlController.GetComponent<XML_Loader>().xmlExt;
        //Debug.Log(getXML.InnerText.ToString() + " | ");
        LoadXMLNodes(getXML);

        //Extract the data from xml and consider if the XML contains more than one item
        usernames = ExtractXML(username);
        userids = ExtractXML(userid);
        genders = ExtractXML(gender);
        moduleids = ExtractXML(moduleid);
        sessionids = ExtractXML(sessionid);
        starttimes = ExtractXML(starttime);
        messagetypeids = ExtractXML(messagetypeid);
        messages = ExtractXML(message);
        procedures = ExtractXML(procedure);
        gamefactors = ExtractXML(gamefactor);

        xmlInfo = new XMLInfo[usernames.Length];

        for (int i = 0; i < usernames.Length; i++)
        {
            xmlInfo[i].username = usernames[i];
            xmlInfo[i].userid = userids[i];
            xmlInfo[i].gender = Int32.Parse(genders[i]);
            xmlInfo[i].moduleid = Int32.Parse(moduleids[i]);
            xmlInfo[i].sessionid = sessionids[i];
            xmlInfo[i].starttime = starttimes[i];
            xmlInfo[i].messagetypeid = messagetypeids[i];
            xmlInfo[i].message = messages[i];
            xmlInfo[i].procedureStatus = Int32.Parse(procedures[i]);
            xmlInfo[i].gameFactor = gamefactors[i];

            //Debug.Log("username: " + xmlInfo[i].username);
            //Debug.Log("moduleid: " + xmlInfo[i].moduleid);
            //Debug.Log("sessionid: " + xmlInfo[i].sessionid);
            //Debug.Log("starttime: " + xmlInfo[i].starttime);
            //Debug.Log("messagetypeid: " + xmlInfo[i].messagetypeid);
            //Debug.Log("message: " + xmlInfo[i].message);
        }
    }
    //Get the data from the XML nodes and assign them to the variables
    void LoadXMLNodes(XmlNode root)
    {
        foreach (XmlNode node in root.ChildNodes)
        {
            switch (node.Name)
            {
                case "username":
                    username += node.InnerText.ToLower() + "<";
                    break;
                case "userid":
                    userid += node.InnerText + "<";
                    break;
                case "gender":
                    gender += node.InnerText + "<";
                    break;
                case "moduleid":
                    moduleid += node.InnerText + "<";
                    break;
                case "sessionid":
                    sessionid += node.InnerText + "<";
                    break;
                case "starttime":
                    starttime += node.InnerText + "<";
                    break;
                case "messagetypeid":
                    messagetypeid += node.InnerText + "<";
                    break;
                case "message":
                    message += node.InnerText + "<";
                    break;
                case "procedure":
                    procedure += node.InnerText + "<";
                    break;
                case "gamefactors":
                    gamefactor += node.InnerText + "<";
                    break;
            }

            if (node.ChildNodes.Count > 0)
                LoadXMLNodes(node);
        }
    }

    //If there are several items saved in the XML file, ExtractXML() will get each of them
    string[] ExtractXML(string infoString)
    {
        string[] extractedXML;
        infoString = infoString.TrimEnd('<');
        extractedXML = infoString.Split('<');
        return extractedXML;
    }
}

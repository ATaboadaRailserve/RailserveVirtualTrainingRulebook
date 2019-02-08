using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class XML_Saver : XMLInfoStruct {

    XmlDocument xmlTemplate = new XmlDocument();
    public XmlDocument xmlSave= new XmlDocument();
    XmlNode extRoot;
    int templateNodeIndex = 1;

    public string templateFileName;
    private string individualRecordFileName;
    private string individualRecordFolderName;
    private string userID;

    List<string> nodeNames = new List<string>();

    private string[] templateXMLs;
    private string[] extXMLs;

    private string tabSpace = ""+'\t';
    private string prifix = "";

    public XMLInfo xmlInfo;
    public List<XMLInfo> xmlInfoList;

    void Start()
    {
        xmlInfoList = new List<XMLInfo>();
        xmlInfo = new XMLInfo();
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

//Original XML_Saver: Save to only one XML file
public void SaveToFile()
    {
        LoadTemplateXML();
        nodeNames.Add(xmlTemplate.FirstChild.Name);
        LoadTemplate(xmlTemplate.FirstChild);
        xmlSave.RemoveAll();
        extRoot = xmlSave.CreateNode(XmlNodeType.Element, nodeNames[0], "");

        foreach (XMLInfo xmlInfoItem in xmlInfoList)
        {
            templateNodeIndex = 1;
            xmlInfo = xmlInfoItem;
            //Debug.Log(xmlInfoList.Count);
            SaveExtXML();
            //AppendPreviousItems();
            //StoreExtXMLFile();
        }

        StoreExtXMLFile();
        xmlInfoList.Clear();
        nodeNames.Clear();
    }

    string GetXMLString(string[] XMLs)
    {
        string XMLString = "";
        for (int i = 0; i < XMLs.Length; i++)
        {
            XMLString += XMLs[i];
        }
        return XMLString;
    }

    //Load the XML Template file
    void LoadTemplateXML()
    {
        //Debug.Log(Application.persistentDataPath);
        templateXMLs = System.IO.File.ReadAllLines(Application.dataPath + "/XML/" + templateFileName);
        xmlTemplate.LoadXml(GetXMLString(templateXMLs));

        //Loaded data will be able to find from templateXMLs[i]
        //Debug.Log("Display XML Template");
        //for (int i = 0; i < templateXMLs.Length; i++)
        //{
        //    Debug.Log(templateXMLs[i]);
        //}
    }

    void LoadTemplate(XmlNode root)
    {
        nodeNames.Add("Child of " + root.Name);
        foreach (XmlNode node in root.ChildNodes)
        {
            //Debug.Log(node.Name);
            nodeNames.Add(node.Name);
            if (node.ChildNodes.Count > 0)
                LoadTemplate(node);
        }
        nodeNames.Add("End of " + root.Name);
    }

    void SaveExtXML()
    {
        //Debug.Log(templateNodeIndex + ":" + nodeNames.Count);
        //while (templateNodeIndex < nodeNames.Count-1)
        while (templateNodeIndex < 14)
        {
            if (nodeNames[templateNodeIndex].Contains("Child of"))
            {
                //Debug.Log(templateNodeIndex + ": " + nodeNames[templateNodeIndex]);
                templateNodeIndex++;
                templateNodeIndex = XMLChildGen(templateNodeIndex, extRoot, nodeNames);
            }
            else if (nodeNames[templateNodeIndex].Contains("End of"))
            {
                //Debug.Log(templateNodeIndex + ": " + nodeNames[templateNodeIndex]);
                templateNodeIndex++;
            }
            else
            {
                //Debug.Log(templateNodeIndex + ": " + nodeNames[templateNodeIndex]);
                templateNodeIndex++;
            }
        }

        xmlSave.AppendChild(extRoot);
        //xmlSave.AppendChild(xmlTemp);
    }

    int XMLChildGen(int index, XmlNode childRoot, List<string> childNodeNames)
    {
        XmlNode childNode = xmlSave.CreateNode(XmlNodeType.Element, childNodeNames[index], "");
        //SaveXMLData(childRoot);
        childRoot.AppendChild(childNode);
        //Debug.Log(index + ": " + prifix + nodeNames[index]);
        index++;

        prifix += tabSpace;
        while (index < childNodeNames.Count-1)
        {
            //Debug.Log(childNodeNames[index]);
            if (childNodeNames[index].Contains("Child of"))
            {
                //Debug.Log(index + ": " + prifix + nodeNames[index]);
                index++;
                index = XMLChildGen(index, childNode, childNodeNames);
            }
            else if (childNodeNames[index].Contains("End of"))
            {
                //Debug.Log(index + ": " + prifix + nodeNames[index]);
                SaveXMLData(childRoot);
                index++;
                break;
            }
            else
            {
                childNode = xmlSave.CreateNode(XmlNodeType.Element, childNodeNames[index], "");
                childRoot.AppendChild(childNode);
                //Debug.Log(index + ": " + prifix + nodeNames[index]);
                index++;
            }
        }
        return index;
    }

    void SaveXMLData(XmlNode extRoot)
    {
        foreach (XmlNode node in extRoot.ChildNodes)
        {
            //Debug.Log(node.Name);
            switch (node.Name)
            {
                case "username":
                    node.InnerText = xmlInfo.username;
                    break;
                case "userid":
                    node.InnerText = xmlInfo.userid;
                    break;
                case "gender":
                    node.InnerText = xmlInfo.gender.ToString();
                    break;
                case "moduleid":
                    node.InnerText = xmlInfo.moduleid.ToString();
                    break;
                case "sessionid":
                    node.InnerText = xmlInfo.sessionid;
                    break;
                case "starttime":
                    node.InnerText = xmlInfo.starttime;
                    break;
                case "messagetypeid":
                    node.InnerText = xmlInfo.messagetypeid;
                    //Debug.Log(xmlInfo.messagetypeid);
                    break;
                case "message":
                    node.InnerText = xmlInfo.message;
                    //Debug.Log(xmlInfo.message);
                    break;
                case "procedure":
                    node.InnerText = xmlInfo.procedureStatus.ToString();
                    //Debug.Log(xmlInfo.procedureStatus);
                    break;
                case "gamefactors":
                    node.InnerText = xmlInfo.gameFactor;
                    break;
            }
        }
    }

    public void StoreExtXMLFile()
    {
        DateTime now = DateTime.Now;
        
        individualRecordFolderName = CreateMD5(xmlInfo.username);
        individualRecordFileName = individualRecordFolderName + "_" + String.Format("{0:MMddyyyy}", now) + ".xml";

        bool exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);

        while (!exists)
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/XML/" + individualRecordFolderName);
            exists = System.IO.Directory.Exists(Application.dataPath + "/XML/" + individualRecordFolderName);
        }

        if (File.Exists(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName))
        {
            Debug.Log("The file " + individualRecordFileName + " exists.");
            GetComponent<XML_Loader>().LoadExtIndividualXML(individualRecordFolderName + "/" + individualRecordFileName);
            foreach (XmlNode node in GetComponent<XML_Loader>().xmlExt.DocumentElement.ChildNodes)
            {
                XmlNode imported = xmlSave.ImportNode(node, true);
                xmlSave.DocumentElement.AppendChild(imported);
            }
            xmlSave.Save(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName);
        }
        else
        {
            Debug.Log("The file " + individualRecordFileName + " not exists.");
            xmlSave.Save(Application.dataPath + "/XML/" + individualRecordFolderName + "/" + individualRecordFileName);
        }
        
    }

    void Update()
    {
        
    }

}

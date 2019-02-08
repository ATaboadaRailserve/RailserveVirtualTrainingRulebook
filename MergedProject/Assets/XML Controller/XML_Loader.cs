using UnityEngine;
using System.Collections;
using System.Xml;

public class XML_Loader : MonoBehaviour {
    public XmlDocument xmlExt = new XmlDocument();
    private string[] extXMLs;

    public bool xmlLoaded = false;

    // Use this for initialization
    void Start () {
        //LoadExtXML();
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

    public void LoadExtIndividualXML(string individualFileName)
    {
        extXMLs = System.IO.File.ReadAllLines(Application.dataPath + "/XML/" + individualFileName);
        xmlExt.LoadXml(GetXMLString(extXMLs));

        xmlLoaded = true;

        //Loaded data will be able to find from extXMLs[i]

        //for (int i = 0; i < extXMLs.Length; i++)
        //{
        //    Debug.Log(extXMLs[i]);
        //}
    }

    // Update is called once per frame
    void Update () {
	
	}
}

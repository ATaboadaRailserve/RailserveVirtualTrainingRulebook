using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CheckDBConnection : MonoBehaviour {
    string checkUserURL = "http://rsconnect.biz/UserData.php";
    bool networkConnected = false;

    public IEnumerator ConnectToDB()
    {
        Debug.Log("Check Database Connection...");
        Debug.Log("Opening WWW...");
        WWW checkConnection = new WWW(checkUserURL);
        yield return checkConnection;
        if (checkConnection.error != null)
        {
            Debug.Log(checkConnection.error);
            networkConnected = false;
        }
        else
        {
            Debug.Log("Connection is Good!");
            networkConnected = true;
        }
    }

    public bool CheckDatabaseConnection()
    {
        StartCoroutine(ConnectToDB());
        return networkConnected;
    }
}

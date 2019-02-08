using UnityEngine;
using System.Collections;

[System.Serializable]
public class XMLInfoStruct : MonoBehaviour {

    public struct XMLInfo
    {
        public string username;
        public string userid;
        public int gender;
        public int moduleid;
        public string sessionid;
        public string starttime;
        public string messagetypeid;
        public string message;
        public int procedureStatus;
        public string gameFactor;

        public XMLInfo(string node1, string node2, int node3, int node4, string node5, string node6, string node7, string node8, int node9, string node10)
        {
            username = node1;
            userid = node2;
            gender = node3;
            moduleid = node4;
            sessionid = node5;
            starttime = node6;
            messagetypeid = node7;
            message = node8;
            procedureStatus = node9;
            gameFactor = node10;
        }
    }

}

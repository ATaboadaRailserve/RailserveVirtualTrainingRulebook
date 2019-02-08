using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneLoad : MonoBehaviour {

    SceneLoader sceneLoader;
    string input;

    void Start()
    {
        sceneLoader = GameObject.FindObjectOfType<SceneLoader>();
        if (sceneLoader == null)
            this.enabled = false;
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.BackQuote))
    //    {
    //        if (input != "")
    //        {
    //            Debug.Log("jumping to: " + input);
    //            string sceneName = input;
    //            input = "";
    //            sceneLoader.LoadScene(sceneName);
    //        }
    //        else
    //        {
    //            Debug.Log("jumping nowhere.....");
    //        }
    //    }
    //    else if (Input.anyKeyDown)
    //    {
    //        input += Input.inputString;
    //    }
    //}

    public void LoadLevel (string sceneName)
    {
        sceneLoader.LoadScene(sceneName);
    }
}

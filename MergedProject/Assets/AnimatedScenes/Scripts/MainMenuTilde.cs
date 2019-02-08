using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTilde : MonoBehaviour {

	// Change this scene value for immediate jump to named scene
	static string fastJumpScene = "";
	
    SceneLoader sceneLoader;
    string input;

    void Start()
    {
        sceneLoader = GameObject.FindObjectOfType<SceneLoader>();
        if (sceneLoader == null)
            this.enabled = false;
    }

	void Update () {
		if(Input.GetKeyDown(KeyCode.BackQuote))
        {
			if(fastJumpScene != "")
				input = fastJumpScene;
            if(input != "" && Application.CanStreamedLevelBeLoaded(input))
            {
                Debug.Log("jumping to: " + input);
                string sceneName = input;
                input = "";
                sceneLoader.LoadScene(sceneName);
            }
            else
            {
                Debug.Log("Failed to jump to \'"+input+"\'");
				input = "";
            }
        }
        else if(Input.anyKeyDown)
        {
            input += Input.inputString;
        }
	}
}

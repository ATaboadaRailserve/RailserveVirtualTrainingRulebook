using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManualOperation : MonoBehaviour {

    public InteractionHandler iHandler;
    public Camera cameraUI;
    public Camera cameraPlayer;

    //void Update()
    //{

    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;

    //}

    public void ManualView(bool SetToCutScene)
    {
        Cursor.lockState = CursorLockMode.None;
        if (SetToCutScene)
        {
            cameraUI.gameObject.SetActive(true);
            iHandler.interactionCamera = cameraUI;
            cameraPlayer.gameObject.SetActive(false);
            Cursor.visible = true;
        }
        else
        {
            cameraPlayer.gameObject.SetActive(true);
            iHandler.interactionCamera = cameraPlayer;
            cameraUI.gameObject.SetActive(false);
            Cursor.visible = false;
        }
    }

}

using UnityEngine;
using System.Collections;

public class TouchCameraController : MonoBehaviour {
    public GameObject mainCamera;
    public GameObject touchCamera;
    public GameObject mainCharacter;
    public GameObject interactSpot;
    private float mainCameraFOV;
    private bool touchCameraOn = false;
    private bool touchCameraOff = true;
    private string cameraTranslation = "off";

    // Use this for initialization
    void Start () {

    }
	
    void CameraTranlation(string type)
    {
        if (type.Equals("in"))
        {   
            if (mainCameraFOV - touchCamera.GetComponent<Camera>().fieldOfView < 20)
            {
                touchCamera.GetComponent<Camera>().fieldOfView -= 0.5f;
            }
            else
            {
                touchCameraOff = false;
                cameraTranslation = "off";
            }
        }
        else if (type.Equals("out"))
        {
            
            if (mainCameraFOV - touchCamera.GetComponent<Camera>().fieldOfView > 0)
            {
                touchCamera.GetComponent<Camera>().fieldOfView += 0.5f;
            }
            else
            {
                touchCameraOff = true;
                touchCameraOn = false;
                touchCamera.SetActive(false);
                mainCharacter.SetActive(true);
                mainCamera.SetActive(true);
                mainCharacter.transform.rotation = touchCamera.transform.rotation;
                cameraTranslation = "off";
                GetComponent<DistanceDisplay>().enableTouchCamera = false;
            }
        }
    }
	// Update is called once per frame
	void Update () {

        if (GetComponent<DistanceDisplay>().enableTouchCamera && !touchCameraOn)
        {
            mainCharacter.SetActive(false);
            touchCameraOn = true;
            mainCameraFOV = mainCamera.GetComponent<Camera>().fieldOfView;
            touchCamera.transform.position = interactSpot.transform.position;
            //touchCamera.transform.position = Vector3.MoveTowards(touchCamera.transform.position, interactSpot.transform.position, 0.02f * Time.deltaTime);
            mainCamera.SetActive(false);
            touchCamera.SetActive(true);
            touchCamera.GetComponent<Camera>().transform.LookAt(GetComponent<DistanceDisplay>().targetObject.transform);
            touchCamera.GetComponent<Camera>().fieldOfView = mainCameraFOV;
            cameraTranslation = "in"; 
        }

        if((touchCameraOn && !touchCameraOff) && Input.GetKeyDown(KeyCode.Escape))
        {
            cameraTranslation = "out";
        }

        if (!cameraTranslation.Equals("off"))
        {
            CameraTranlation(cameraTranslation);
        }
    }
}

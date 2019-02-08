using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour {
	
	/*
	Notes:
	For the future, make this generate a length of camera view buttons based on a template button and the end caps.
	*/
	
	[System.Serializable]
	public struct CameraView {
		public GameObject button;
		public GameObject camObj;
		
		[HideInInspector]
		public Vector3 buttonOrigin;
	}
	
	public CameraView[] cameraViews;
	public GameObject selectedGraphic;
	public float amountToStickOut = 16;
	public int defaultCamera = 0;
	
	void Start () {
		for (int i = 0; i < cameraViews.Length; i++) {
			cameraViews[i].buttonOrigin = cameraViews[i].button.transform.position;
		}
		ChangeCamera(defaultCamera);
	}
	
	public void ChangeCamera (int index) {
		for (int i = 0; i < cameraViews.Length; i++) {
			cameraViews[i].button.transform.position = cameraViews[i].buttonOrigin;
			cameraViews[i].camObj.SetActive(false);
		}
		
		selectedGraphic.transform.parent = cameraViews[index].button.transform;
		selectedGraphic.transform.localPosition = Vector3.zero;
		
		cameraViews[index].button.transform.position = cameraViews[index].buttonOrigin - new Vector3(amountToStickOut,0,0);
		cameraViews[index].camObj.SetActive(true);
	}
}
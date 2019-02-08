
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WarpSwitchGame : MonoBehaviour {
	public List<GameObject> warplist = new List<GameObject>();
	public List<Image> imagelist = new List<Image>();
	public GameObject numbers, warpIntructions;
	private GameObject player;
	private UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;
	private Camera cam;

	private bool shiftDown = false;
	private bool doingStuff = false;
	private Vector3 centerMousePos;
	public Color selectedColor, originalColor;
	private int changed = -1;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		fpsController = player.transform.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();
		cam = Camera.main;
		foreach (Image i in imagelist) {
			i.enabled = false;
		}
		numbers.SetActive (false);
	}
	public void MoveRotate(GameObject g)
	{
		player.transform.position = g.transform.position;
	}
	public void SelectMenu(float a)
	{
		if (a <= 180 && a > 120 && changed != 0) {ChangeColor (0);}
		else if (a <= 120 && a > 60 && changed != 1) {ChangeColor (1);}
		else if (a <= 60 && a > 0 && changed != 2) {ChangeColor (2);}
		else if (a > -180 && a <= -120 && changed != 5) {ChangeColor (5);}
		else if (a > -120 && a <= -60 && changed != 4) {ChangeColor (4);}
		else if (a > -60 && a <= 0 && changed != 3) {ChangeColor (3);}
	}
	public void ChangeColor(int i)
	{
		changed = i;
		foreach (Image a in imagelist) {
			a.color = originalColor;
		}
		imagelist [i].color = selectedColor;
	}
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown (KeyCode.Alpha1)) {
			MoveRotate (w1);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			MoveRotate (w2);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			MoveRotate (w3);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			MoveRotate (w4);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			MoveRotate (w5);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			MoveRotate (w6);
		}*/
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			shiftDown = true;
			if (!doingStuff) {
				warpIntructions.SetActive (false);
				fpsController.enabled = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				numbers.SetActive (true);
				foreach (Image i in imagelist) {
					i.enabled = true;
				}
				centerMousePos = Input.mousePosition;
			}
			float angle = Mathf.Atan2 ((Input.mousePosition.y - centerMousePos.y), (Input.mousePosition.x - centerMousePos.x)) * 180 / Mathf.PI;
			SelectMenu (angle);
//			print (angle);
			doingStuff = true;
		} else {
			shiftDown = false;
			if (doingStuff) {
				fpsController.enabled = true;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = false;
				numbers.SetActive (false);
				foreach (Image i in imagelist) {
					i.enabled = false;
				}
			}
			doingStuff = false;
		}

		if (shiftDown && Input.GetMouseButtonDown (0)) {
			MoveRotate (warplist [changed]);
		}
	}
}

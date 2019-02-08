using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnuckleController : MonoBehaviour {

	private Quaternion openRotation = new Quaternion(0.4f, 0.6f, 0.6f, -0.4f);
	private Quaternion closeRotation = new Quaternion(0.0f, 0.7f, 0.7f, 0.0f);
	private Transform parent;
	private float timer;
	private float openTime = .5f;
	private float closeTime = .5f;
	private bool open = false;

	// Use this for initialization
	void Start () {
		parent = transform.parent.parent;
		Open ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "Back_Hitbox") {
			Close ();
		}
	}

	public void Open()
	{
		if (parent.localRotation != openRotation && !open) {
			open = true;
			StartCoroutine (Open_E ());
		}
	}

	public void Close()
	{
		if (parent.localRotation != closeRotation && open) {
			open = false;
			StartCoroutine (Close_E ());
		}
	}

	IEnumerator Open_E()
	{
		//print ("Open");
		timer = 0;
		while (timer < 1) {
			timer += Time.deltaTime / openTime;
			parent.localRotation = Quaternion.Lerp(closeRotation, openRotation, timer);
			yield return null;
		}
	}
	IEnumerator Close_E()
	{
		//print ("Close");
		timer = 0;
		while (timer < 1) {
			timer += Time.deltaTime / closeTime;
			parent.localRotation = Quaternion.Lerp(openRotation, closeRotation, timer);
			yield return null;
		}

	}
}

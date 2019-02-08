using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour 
{

	public bool isParent;
	public GameObject[] sections;
	public Vector3 targetPos; //Move to new pos
	public float moveTime = 0.5f;

	private Vector3 origin;
	private bool clicked;
	private GameObject parent;
	private float timer = 0;
	private bool moving;

	void Start () {
		if(isParent)
			Initialize(gameObject);
	}

	public void Initialize(GameObject inParent){
		origin = transform.position;
		parent = inParent;
		foreach(GameObject s in sections){
			if (s.GetComponent<Board>())
				s.GetComponent<Board>().Initialize(gameObject);
			else
				s.SetActive(false);
		}
		if(!isParent && !parent.GetComponent<Board>().isParent)
			gameObject.SetActive(false);
	}

	public void OnClick () {
		if (moving)
			return;
		moving = true;
		print("Clicked: " + gameObject.name);
		clicked = !clicked;
		Toggle(gameObject);

		StartCoroutine(Move());
	}

	public void Toggle (GameObject caller) {
		if (parent != gameObject)
			parent.GetComponent<Board>().Toggle(caller);
		else {
			foreach (GameObject s in sections) {
				if (s.GetComponent<Board>())
					s.GetComponent<Board>().DoToggle(caller);
			}
		}
	}

	public void DoToggle (GameObject caller) {
		foreach (GameObject s in sections) {
			if (s.GetComponent<Board>())
				s.GetComponent<Board>().DoToggle(caller);
			else
				s.SetActive(false);
		}
		if (caller != gameObject)
			gameObject.SetActive(false);
	}

	IEnumerator Move () {
		timer = 0;
		while (timer < 1 - Time.deltaTime/moveTime) {
			timer += Time.deltaTime/moveTime;
			transform.position = Vector3.Lerp(origin + targetPos*(clicked ? 0 : 1), origin + targetPos*(clicked ? 1 : 0), timer);
			yield return null;
		}
		transform.position = origin + targetPos*(clicked ? 1 : 0);

		if (clicked) {
			foreach (GameObject s in sections) {
				s.SetActive(true);
			}
		} else {
			parent.SetActive(true);
			foreach (GameObject s in parent.GetComponent<Board>().sections) {
				s.SetActive(true);
			}
		}
		moving = false;
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + targetPos);
	}
}
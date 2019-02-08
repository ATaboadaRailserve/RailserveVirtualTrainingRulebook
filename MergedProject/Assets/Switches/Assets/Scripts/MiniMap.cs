using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {
	public GameObject panel;
	public GameObject player;
	public Image im;
	public float speed;

	private Vector3 position;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		print (im.rectTransform.anchoredPosition);
		//position.x  +=1;
		position.x = (player.transform.position.z*(177f/290f))- 558f;
		position.y = (player.transform.position.x *(-282f/192f))+ 34f;
		position.z = 0;

		im.rectTransform.localPosition = new Vector3 (im.rectTransform.anchoredPosition.x, im.rectTransform.anchoredPosition.y);
		//image.transform.position = position;
	}
}

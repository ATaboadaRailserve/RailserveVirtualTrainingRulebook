using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Measurer : MonoBehaviour {
	
	public AnimationScrubber scrubber;
	public Vector2 timeFrame;
	public RectTransform graphic;
	public Text text;
	
	private RectTransform rtrans;
	private float deltaTime;
	private float timer;
	private float height;
	
	void Start () {
		rtrans = GetComponent<RectTransform>();
		height = rtrans.rect.height*10f - rtrans.rect.height;
		graphic.sizeDelta = new Vector2(-rtrans.rect.width,height);
		deltaTime = timeFrame.y - timeFrame.x;
	}
	
	void Update () {
		timer = (scrubber.GetTime() - timeFrame.x)/deltaTime;
		timer = Mathf.Clamp(timer, 0, 1);
		graphic.sizeDelta = Vector2.Lerp(new Vector2(-rtrans.rect.width,height), new Vector2(rtrans.rect.width*10f - rtrans.rect.width,height), timer);
		if (timer > 0.25f)
			text.text = ((int)(rtrans.rect.width*timer*3.28084f)).ToString() + "ft";
		else
			text.text = "";
	}
}

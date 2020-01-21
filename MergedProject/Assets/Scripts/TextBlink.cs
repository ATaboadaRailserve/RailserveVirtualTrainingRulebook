using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour {
	
	public Text text;
	public Color[] colors;
	public float totalFadeTime;
	public bool fadeBetween;
	
	float timer;
	float index;
	
	void Update () {
		index = (timer*colors.Length)%colors.Length;
		if (fadeBetween) {
			text.color = Color.Lerp(colors[(int)index], colors[(int)index == colors.Length-1 ? 0 : (int)index+1], index%1);
		} else {
			text.color = colors[(int)index];
		}
		timer += Time.deltaTime/totalFadeTime;
		timer %= 1;
	}
}

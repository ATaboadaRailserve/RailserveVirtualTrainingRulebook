using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Colorizor : MonoBehaviour {
	
	[System.Serializable]
	public struct Colorize {
		public Vector2 timeFrame;
		public Color startColor;
		public Color endColor;
		public AnimationCurve transitionCurve;
	}
	
	public AnimationScrubber scrubber;
	public Colorize[] colorizes;
	public Renderer renderer;
	public Image image;
	
	private List<Colorize> sortedColors;
	private bool found;
	private float timer;
	private float deltaTime;
	
	void Start () {
		if (colorizes.Length <= 0)
			return;
		// If the colorizes are out of order, ORDER THEM
		// If there are gaps between the times, UN GAP THEM!
		
		// Convert to a List because it'll be easier to work with.
		sortedColors = new List<Colorize>();
		for (int i = 0; i < colorizes.Length; i++) {
			sortedColors.Add(colorizes[i]);
		}
		
		// Sort by time.
		for (int i = 1; i < sortedColors.Count; i++) {
			for (int j = i; j > 0; j--) {
				if (sortedColors[j-1].timeFrame.x > sortedColors[j].timeFrame.x) {
					Colorize tempPoint1 = sortedColors[j];
					Colorize tempPoint2 = sortedColors[j-1];
					sortedColors[j-1] = tempPoint1;
					sortedColors[j] = tempPoint2;
				}
			}
		}
		
		// If the first sorted waypoint doesn't start at Zero or less, make one that does.
		if (sortedColors[0].timeFrame.x > 0) {
			Colorize tempPoint = new Colorize();
			tempPoint.startColor = sortedColors[0].startColor;
			tempPoint.endColor = sortedColors[0].startColor;
			tempPoint.timeFrame.x = 0;
			tempPoint.timeFrame.y = sortedColors[0].timeFrame.x;
			tempPoint.transitionCurve = new AnimationCurve();
			sortedColors.Insert(0, tempPoint);
		}
		
		// Fill in the gaps.  For each waypoint, if there's a gap between it and the last one, make a waypoint between them.
		for (int i = 1; i < sortedColors.Count; i++) {
			if (sortedColors[i-1].timeFrame.y < sortedColors[i].timeFrame.x) {
				Colorize tempPoint = new Colorize();
				tempPoint.startColor = sortedColors[i-1].endColor;
				tempPoint.endColor = sortedColors[i-1].endColor;
				tempPoint.timeFrame.x = sortedColors[i-1].timeFrame.y;
				tempPoint.timeFrame.y = sortedColors[i].timeFrame.x;
				tempPoint.transitionCurve = new AnimationCurve();
				sortedColors.Insert(i, tempPoint);
			}
		}
	}
	
	void Update () {
		if (colorizes.Length <= 0)
			return;
		
		found = false;
		timer = scrubber.GetTime();
		for (int i = 0; i < sortedColors.Count; i++) {
			if (timer >= sortedColors[i].timeFrame.x && timer <= sortedColors[i].timeFrame.y) {
				deltaTime = sortedColors[i].timeFrame.y - sortedColors[i].timeFrame.x;
				
				if (renderer)
					renderer.material.color = Color.Lerp(sortedColors[i].startColor, sortedColors[i].endColor, sortedColors[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedColors[i].timeFrame.x)/deltaTime, 0, 1)));
				
				if (image)
					image.color = Color.Lerp(sortedColors[i].startColor, sortedColors[i].endColor, sortedColors[i].transitionCurve.Evaluate(Mathf.Clamp((timer-sortedColors[i].timeFrame.x)/deltaTime, 0, 1)));
				
				found = true;
				break;
			}
		}
		if (!found) {
			int i = sortedColors.Count-1;
		}
	}
}

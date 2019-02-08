using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TimeLinePauser : MonoBehaviour, IPointerDownHandler {
	
	public AnimationScrubber scrubber;
	
	public void OnPointerDown (PointerEventData eventData) {
		scrubber.TimeLinePause();
		scrubber.UpdateTime();
	}
}

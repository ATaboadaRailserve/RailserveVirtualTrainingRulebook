using UnityEngine;
using System.Collections;

public class Revealer : MonoBehaviour {
	
	public float secondsToComplete;
	public Vector3 endPoint;
	public bool reveal;
	public bool destroyThisAfterReveal;
	public Material materialToMove;
	public float uvShiftSpeed;
	public GameObject[] revealers;
	public GameObject[] toBeRevealed;
	
	public bool useScrubberTime;
	public AnimationScrubber scrubber;
	
	private Vector3 origin;
	private float timer;
	private Vector2 textureUV;
	private Vector2 timeFrame = new Vector2(0,0.0000001f);
	
	void Start () {
		origin = transform.position;
	}
	
	void Update () {
		
		if (useScrubberTime) {
			timer = scrubber.GetTime();
			if (timer >= timeFrame.x && timer <= timeFrame.y) {
				
				foreach (GameObject g in revealers) {
					g.SetActive(true);
				}
				
				if (destroyThisAfterReveal && timer >= 1) {
					Destroy(gameObject);
				}
				if (reveal)
					transform.position = Vector3.Lerp(origin, endPoint + origin, (timer - timeFrame.x)/(timeFrame.y-timeFrame.x));
				else
					transform.position = Vector3.Lerp(endPoint + origin, origin, (timer - timeFrame.x)/(timeFrame.y-timeFrame.x));
				
				if (materialToMove) {
					textureUV.x = (timer - timeFrame.x)*uvShiftSpeed;
					materialToMove.SetTextureOffset("_MainTex", new Vector2(textureUV.x, textureUV.y));
				}
				
			} else {
				foreach (GameObject g in revealers) {
					g.SetActive(false);
				}
				
				if (timer > timeFrame.y) {
					if (reveal)
						transform.position = endPoint + origin;
					else
						transform.position = origin;
				} else {
					if (!reveal)
						transform.position = endPoint + origin;
					else
						transform.position = origin;
				}
			}
		} else {
			if (reveal) {
				if (timer <= 1-Time.deltaTime/secondsToComplete) {
					foreach (GameObject g in revealers) {
						g.SetActive(true);
					}
					
				
				foreach (GameObject g in toBeRevealed) {
					g.SetActive(true);
				}
					timer += Time.deltaTime/secondsToComplete;
					if (destroyThisAfterReveal && timer >= 1) {
						Destroy(gameObject);
					}
					transform.position = Vector3.Lerp(origin, endPoint + origin, timer);
					if (materialToMove) {
						textureUV.x += uvShiftSpeed;
						materialToMove.SetTextureOffset("_MainTex", new Vector2(textureUV.x, textureUV.y));
					}
				} else {
					foreach (GameObject g in revealers) {
						g.SetActive(false);
					}
					timer = 1;
					foreach (GameObject g in toBeRevealed) {
						g.SetActive(true);
					}
				}
			} else {
				if (timer >= Time.deltaTime/secondsToComplete) {
					foreach (GameObject g in revealers) {
						g.SetActive(true);
					}
					timer -= Time.deltaTime/secondsToComplete;
					transform.position = Vector3.Lerp(origin, endPoint + origin, timer);
					if (materialToMove) {
						textureUV.x += uvShiftSpeed;
						materialToMove.SetTextureOffset("_MainTex", new Vector2(textureUV.x, textureUV.y));
					}
				} else {
					foreach (GameObject g in revealers) {
						g.SetActive(false);
					}
					timer = 0;
					foreach (GameObject g in toBeRevealed) {
						g.SetActive(false);
					}
				}
			}
		}
	}
	
	public void DoReveal (Vector2 inTimeFrame, bool inReveal) {
		timeFrame = inTimeFrame;
		reveal = inReveal;
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, endPoint + transform.position);
	}
}

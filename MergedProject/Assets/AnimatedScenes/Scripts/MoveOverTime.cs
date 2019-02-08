using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveOverTime : MonoBehaviour {
	public AnimationScrubber scrubber;
	public List<MoveCameraPositions> moveList = new List<MoveCameraPositions> ();
	[HideInInspector]
	public float curve = 0;
	public AnimationCurve animationCurve;

	// Use this for initialization
	void Start () {
		foreach (MoveCameraPositions p in moveList) {
			p.deltaT = p.timeFrame.y - p.timeFrame.x;
		}
		OrderList ();
	}
	public void OrderList()
	{
		for (int write = 0; write < moveList.Count; write++) {
			for (int sort = 0; sort < moveList.Count - 1; sort++) {
				if (moveList [sort].order > moveList [sort + 1].order) {
					MoveCameraPositions temp = moveList [sort + 1];
					moveList [sort + 1] = moveList [sort];
					moveList [sort] = temp;
				}
			}
		}
	}

	public float CheckRotation(int i, float s, float e, bool b)
	{
		float value;
		float f = scrubber.GetTime ();

		if (Mathf.Abs (s - e) > 180 && !b) {
			if (s < e) {
				e -= 360;
			} else {
				s -= 360;
			}
		}
		else if (Mathf.Abs (s - e) < 180 && b){
			if (s < e) {
				e += 360;
			} else {
				s += 360;
			}
		}
		float tfloat = ((f - moveList [i].timeFrame.x) / moveList [i].deltaT);
		value = Mathf.Lerp (s, e, animationCurve.Evaluate (tfloat));

		return value;
	}
	// Update is called once per frame
	void Update () {
		float f = scrubber.GetTime ();
		bool doingStuff = false;

		for (int i = 0; i < moveList.Count; i++)
		{
			if (f > moveList [i].timeFrame.x && f < moveList [i].timeFrame.y) {
				Vector3 temp;
				float tfloat = ((f - moveList [i].timeFrame.x) / moveList [i].deltaT);
				//(f-moveList[i].timeFrame.x) / moveList[i].deltaT
				temp = Vector3.Lerp	 (moveList [i].startTransform.position, moveList [i].endTransform.position, animationCurve.Evaluate (tfloat));
				transform.position = temp;

				Vector3 temps = moveList [i].startTransform.eulerAngles;
				Vector3 tempe = moveList [i].endTransform.eulerAngles;

				temp.x = CheckRotation (i, temps.x, tempe.x, moveList[i].LongRotationX);
				temp.y = CheckRotation (i, temps.y, tempe.y, moveList[i].LongRotationY);
				temp.z = CheckRotation (i, temps.z, tempe.z, moveList[i].LongRotationZ);
				transform.eulerAngles = temp;
				doingStuff = true;
			}
		}
		if (!doingStuff) {
			int cl = -1;
			for (int i = 0; i < moveList.Count; i++) {
				if (f > moveList [i].timeFrame.y) {
					cl = i;
				}
			}
			if (cl == -1) {
				transform.eulerAngles = moveList [0].startTransform.eulerAngles;
				transform.position = moveList [0].startTransform.position;
			} else {
				transform.eulerAngles = moveList [cl].endTransform.eulerAngles;
				transform.position = moveList [cl].endTransform.position;
			}
		}
	}
}

[System.Serializable]
public class MoveCameraPositions
{
	public float order = 1;
	public Vector2 timeFrame;
	public Transform startTransform;
	public Transform endTransform;
	public bool LongRotationX,LongRotationY,LongRotationZ;
	[HideInInspector]
	public float deltaT;
}




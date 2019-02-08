using UnityEngine;
using System.Collections;

public class BezierSetup : MonoBehaviour {
	
	public struct BezierPacket {
		public BezierSetup controller;
		public SplineInterpolator spline;
		public bool derailing;
	}
	
	public BezierSetup previousBezier;
	public BezierSetup nextBezier;
	public BezierSetup divergeNextBezier;
	
	public SplineInterpolator currentBezier;
	public SplineInterpolator divergeCurrentBezier;
	public SplineInterpolator derailBezier;
	
	public bool invertPositionOnPreviousBezierChange;
	public bool invertPositionOnNextBezierChange;
	public bool invertPositionOnDivergeBezierChange;
	public bool diverging;
	public bool unlatchedSwitch;
	public bool overridePreviousDiverge;
	public bool overrideNextDiverge;
	public bool overridePreviousStraight;
	public bool overrideNextStraight;
	
	public bool isSwitch;
	
	//[HideInInspector]
	public float currentT;
	
	//[HideInInspector]
	public float visibleDistance;
	//[HideInInspector]
	public float visibleDistanceDiverge;
	//[HideInInspector]
	public float visibleDistanceDerail;
	//[HideInInspector]
	public float currentDistance;
	
	public BezierPacket GetBezier (bool previous) {
		
		// Make a new packet to send back || Packet contains the BezierSetup and SplineInterpolator
		BezierPacket temp = new BezierPacket();
		
		// Figure out which direction we're going
		if (previousBezier && previous)
			temp.controller = previousBezier;
		else if (divergeNextBezier && diverging)
			temp.controller = divergeNextBezier;
		else if (nextBezier)
			temp.controller = nextBezier;
		
		// Figure out which spline to put it on in the new direction
		if (previous) {
			if (overridePreviousDiverge) {
				temp.spline = temp.controller.divergeCurrentBezier;
				currentDistance = visibleDistanceDiverge;
			} else if (overridePreviousStraight) {
				temp.spline = temp.controller.currentBezier;
				currentDistance = visibleDistance;
			} else {
				temp.spline = temp.controller.GetSpline();
			}
		} else {
			if (temp.controller.unlatchedSwitch) {
				temp.spline = temp.controller.derailBezier;
				temp.derailing = true;
				currentDistance = visibleDistanceDerail;
			} else if (overrideNextDiverge) {
				temp.spline = temp.controller.divergeCurrentBezier;
				currentDistance = visibleDistanceDiverge;
			} else if (overrideNextStraight) {
				temp.spline = temp.controller.currentBezier;
				currentDistance = visibleDistance;
			} else {
				temp.spline = temp.controller.GetSpline();
			}
		}
		
		return temp;
	}
	
	public SplineInterpolator GetSpline () {
		if (divergeCurrentBezier && diverging)
			return divergeCurrentBezier;
		else
			return currentBezier;
	}
	
	void Start () {
		if (isSwitch) {
			if (currentBezier)
				visibleDistance = currentBezier.GetLength(currentBezier.mPoints.Count);
			
			if (divergeCurrentBezier)
				visibleDistanceDiverge = divergeCurrentBezier.GetLength(divergeCurrentBezier.mPoints.Count);
			
			if (derailBezier)
				visibleDistanceDerail = derailBezier.GetLength(derailBezier.mPoints.Count);
			
			currentT = 1;
			currentDistance = visibleDistance;
			return;
		}
		
		int maxVisibleRail = -1;
		
		if (currentBezier) {
			maxVisibleRail = -1;
			for (int i = 0; i < currentBezier.mPoints.Count; i++) {
				if (currentBezier.mPoints[i].childCount > 0) {
					maxVisibleRail = i;
				}
			}
			if (maxVisibleRail >= 0) {
				visibleDistance = currentBezier.GetLength(maxVisibleRail);
				//float tempDistance = currentBezier.GetLength(currentBezier.mPoints.Count-1, Mathf.Max(maxVisibleRail, 0));
				//currentT = visibleDistance/(tempDistance+visibleDistance);
				currentT = visibleDistance/currentBezier.GetLength(currentBezier.mPoints.Count);
			}
			currentDistance = visibleDistance;
		}
	}
	
	public void SwitchLocked (bool diverge) {
		diverging = diverge;
		unlatchedSwitch = false;
	}
	
	public void SwitchOpen () {
		unlatchedSwitch = true;
	}
}

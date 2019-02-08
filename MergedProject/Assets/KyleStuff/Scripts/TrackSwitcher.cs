using UnityEngine;
using System.Collections;

// This script is attached to a split piece of track - or one
// that has a switch on it.  The public var (bezierSet) must
// be a Bezier set, not an individual Bezier curve
public class TrackSwitcher : MonoBehaviour {
	public Transform bezierSet;
	private BezierSwitcher bezierSetSwitchScript;
	// Use this for initialization
	void Start () {
		bezierSetSwitchScript = bezierSet.GetComponent<BezierSwitcher>();
	}

	public void SwitchTrack() {
		bezierSetSwitchScript.SwitchBezier();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Bezier switcher is a script used to toggle which Bezier curve is active in the set.
/// It is attached to a BezierSet (empty) that contains one or two Beziers as children.
/// Only one Bezier can be active at a time
/// </summary>
public class BezierSwitcher : MonoBehaviour {

	public Transform bezier1;
	public Transform bezier2;

	public BezierScript GetActiveBezierScript() {
		if (bezier1.GetComponent<BezierScript>().isActive) {
			return bezier1.GetComponent<BezierScript>();
		}
		return bezier2.GetComponent<BezierScript>();
	}

	public void SwitchBezier() {


		if (bezier1.GetComponent<BezierScript>().isActive) {
			bezier1.GetComponent<BezierScript>().Hide();  // This also sets it to active
			bezier2.GetComponent<BezierScript>().Show();
		}
		else {
			bezier1.GetComponent<BezierScript>().Show();
			bezier2.GetComponent<BezierScript>().Hide();
		}
	}

}

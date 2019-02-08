using UnityEngine;
using System.Collections;
using Rewired;

public class LookNMove : MonoBehaviour {
	
	public Tutorial tutorial;
	public Transform xAxisDetector;
	public Transform yAxisDetector;
	
	private Vector2 previousAngles;
	private bool look = true;
	private bool move;
	private bool up;
	private bool down;
	private bool left;
	private bool right;
	
	#region RewiredStuff
	private Player _player;
			
	private Player player {
		get {
			// Get the Rewired Player object for this player. Refresh it as needed so it will get the new reference after a script recompile in the editor.
			if(_player == null) _player = ReInput.players.GetPlayer(0);
			return _player;
		}
	}
	#endregion
	
	void Update () {
		if (look) {
			print ("Looking");
			if (!up && xAxisDetector.localEulerAngles.x > previousAngles.x)
				up = true;
			if (!down && xAxisDetector.localEulerAngles.x < previousAngles.x)
				down = true;
			if (!right && yAxisDetector.localEulerAngles.y > previousAngles.y)
				right = true;
			if (!left && yAxisDetector.localEulerAngles.y < previousAngles.y)
				left = true;
			if (up && down && left && right) {
				tutorial.Next();
				look = false;
				move = true;
				up = false;
				down = false;
				left = false;
				right = false;
			}
			previousAngles = new Vector2(xAxisDetector.localEulerAngles.x, yAxisDetector.localEulerAngles.y);
		} else if (move) {
			if (!up && player.GetAxis("Vertical") > 0)
				up = true;
			if (!down && player.GetAxis("Vertical") < 0)
				down = true;
			if (!right && player.GetAxis("Horizontal") > 0)
				right = true;
			if (!left && player.GetAxis("Horizontal") < 0)
				left = true;
			if (up && down && left && right) {
				tutorial.Next();
				Destroy(gameObject);
			}
		}
	}
}

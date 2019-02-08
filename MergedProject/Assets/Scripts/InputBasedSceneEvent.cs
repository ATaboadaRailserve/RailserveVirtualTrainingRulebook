using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class InputBasedSceneEvent : MonoBehaviour {
	
	[System.Serializable]
	public enum SceneDelimiter {
		Contains,
		DoesNotContain,
		StartsWith,
		StartsWithout
	}
	
	[System.Serializable]
	public struct AND {
		[Header("AND")]
		public OR[] subLimiter;
	}
	
	[System.Serializable]
	public struct OR {
		[Header("OR")]
		public SceneDelimiter limitation;
		public string delimiter;
	}
	
	public string axis;
	public AND[] limitations;
	public InteractionHandler.InvokableState onButtonDown;
	public InteractionHandler.InvokableState onButton;
	public InteractionHandler.InvokableState onButtonUp;
	
	private bool enable;
	
	#region Rewired Stuff
	private Player player;
	void Awake() {
		player = ReInput.players.GetPlayer(0); // get the Rewired Player
	}
	#endregion
	
	void Update () {
		if (player.GetButtonDown(axis) && CheckScene())
			onButtonDown.Invoke();
		if (player.GetButton(axis) && CheckScene())
			onButton.Invoke();
		if (player.GetButtonUp(axis) && CheckScene())
			onButtonUp.Invoke();
	}
	
	bool CheckScene () {
		if (limitations.Length == 0)
			return false;
		
		string scene = SceneManager.GetActiveScene().name;
		enable = false;
		foreach (AND a in limitations) {
			foreach (OR o in a.subLimiter) {
				enable = false;
				switch (o.limitation) {
					case SceneDelimiter.Contains:
						if (scene.Contains(o.delimiter)) {
							enable = true;
						}
						break;
					case SceneDelimiter.DoesNotContain:
						if (!scene.Contains(o.delimiter)) {
							enable = true;
						}
						break;
					case SceneDelimiter.StartsWith:
						if (scene.Substring(0,o.delimiter.Length) == o.delimiter) {
							enable = true;
						}
						break;
					case SceneDelimiter.StartsWithout:
						if (scene.Substring(0,o.delimiter.Length) != o.delimiter) {
							enable = true;
						}
						break;
				}
				if (enable)
					break;
			}
			if (!enable)
				break;
		}
		return enable;
	}
}

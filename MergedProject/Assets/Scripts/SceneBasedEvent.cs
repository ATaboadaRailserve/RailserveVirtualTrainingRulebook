using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBasedEvent : MonoBehaviour {
	
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
	
	public AND[] limitations;
	public InteractionHandler.InvokableState onTrue;
	public InteractionHandler.InvokableState onFalse;
	
	private bool enable;
	
	void Start () {
		SceneManager.sceneLoaded += OnSceneLoaded;
		
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}
	
	void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
		if (limitations.Length == 0) {
			onTrue.Invoke();
			return;
		}
		
		enable = false;
		foreach (AND a in limitations) {
			foreach (OR o in a.subLimiter) {
				enable = false;
				switch (o.limitation) {
					case SceneDelimiter.Contains:
						if (scene.name.Contains(o.delimiter)) {
							enable = true;
						}
						break;
					case SceneDelimiter.DoesNotContain:
						if (!scene.name.Contains(o.delimiter)) {
							enable = true;
						}
						break;
					case SceneDelimiter.StartsWith:
						if (scene.name.Substring(0,o.delimiter.Length) == o.delimiter) {
							enable = true;
						}
						break;
					case SceneDelimiter.StartsWithout:
						if (scene.name.Substring(0,o.delimiter.Length) != o.delimiter) {
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
		if (enable)
			onTrue.Invoke();
		else
			onFalse.Invoke();
	}
}

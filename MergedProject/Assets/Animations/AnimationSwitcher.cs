using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSwitcher : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
	}

	public void Next()
	{
		int aHash = Animator.StringToHash("Switch");
		animator.SetTrigger(aHash);
	}
}

//Andrew Taboada
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSetup : MonoBehaviour {
	[System.Serializable]
	public struct AnimationPointTrigger {
		[Range(0,1)]
		public float percent;
		[Range(0,1)]
		public float range;

		public bool resetOnReplay;

		[HideInInspector]
		public bool hasTriggered;

		[HideInInspector]
		public bool hasTriggeredEnd;

		public InteractionHandler.InvokableState onStateEnter;
		public InteractionHandler.InvokableState onStateExit;
	}

	[System.Serializable]
	public struct AnimationContents {
		public string clipName; //Name of Clip
		public float playLength; //How long to play clip for. If set to -1 the clip will play forever
		public int animationLayer; 
		public AnimationCurve animationSpeed; //animation curve
		public bool curveIsPerCycle; //animation speed will follow full curve per animations cycle
		public float animationOffset; //when to start animation
		public bool loop;
		[Range(0,1)]
		public float weight; //used if animation is masked
		public bool resetWeigthAtEnd; //does animation stayed masked or not
		public AnimationPointTrigger[] animationPointTrigger;

		[Header("Transition after animation")]
		public int animatorTransitionIndex; //index of the next animation you want to play in the list
		public InteractionHandler.InvokableState onTransitionEnd; //do something
		public InteractionHandler.InvokableState onClipLengthEnd; //when playLenght is done
		public InteractionHandler.InvokableState onClipBegin;

		[HideInInspector]
		public float clipLength;

		[HideInInspector]
		public Animator animator;

	}

	public struct CurrentAnimation {
		public string clipname;
		public float timer; //how long has the animation been playing
		public int index;
		public int stagedTransition; //what animation plays next
	}

	public AnimationContents[] animations; //List of animations
	private CurrentAnimation[] currentAnim;  //Current animation for each layer

	private bool initialized = false; 
	private float workerFloat;
	private bool readyToPlay = false;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < animations.Length; i++) {
			animations [i].animator = gameObject.GetComponent<Animator>();
			foreach (AnimationClip ac in animations[i].animator.runtimeAnimatorController.animationClips) {
				if (ac.name == animations [i].clipName) {
					animations [i].clipLength = ac.length; //get cliplength
				}
			}
		}

		currentAnim = new CurrentAnimation[gameObject.GetComponent<Animator>().layerCount]; //set current animations list to be layercount long

		initialized = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.GetComponent<Animator> ().GetInteger("Index") != null)
			gameObject.GetComponent<Animator> ().SetInteger ("Index", 0);
		for (int i = 0; i < currentAnim.Length; i++) 
		{
			if (currentAnim [i].clipname != "" && readyToPlay) 
			{
				if (currentAnim [i].timer < animations [currentAnim [i].index].playLength) 
				{
					currentAnim [i].timer += Time.deltaTime;
					if (animations [currentAnim [i].index].curveIsPerCycle) 
					{
						if (animations [currentAnim [i].index].loop) 
						{
							workerFloat = ((currentAnim [i].timer) / (animations [currentAnim [i].index].clipLength) + animations [currentAnim [i].index].animationOffset / animations [currentAnim [i].index].clipLength);
							workerFloat %= 1f;
						} 
						else 
						{
							workerFloat = animations [currentAnim [i].index].animationSpeed.Evaluate ((currentAnim [i].timer) / (animations [currentAnim [i].index].playLength));
							workerFloat = Mathf.Clamp (workerFloat, 0, 1);
						}
						workerFloat = animations [currentAnim [i].index].animationSpeed.Evaluate (workerFloat);
					} 
					else 
					{
						if (animations [currentAnim [i].index].loop) {
							workerFloat = animations [currentAnim [i].index].animationSpeed.Evaluate ((currentAnim [i].timer) / (animations [currentAnim [i].index].playLength) + animations [currentAnim [i].index].animationOffset / animations [currentAnim [i].index].clipLength) * ((animations [currentAnim [i].index].playLength) / animations [currentAnim [i].index].clipLength);
							workerFloat %= 1f;
						} 
						else 
						{
							workerFloat = ((currentAnim [i].timer) / (animations [currentAnim [i].index].clipLength) + animations [currentAnim [i].index].animationOffset / animations [currentAnim [i].index].clipLength);
							workerFloat = Mathf.Clamp (workerFloat, 0, 1);
							workerFloat = animations [currentAnim [i].index].animationSpeed.Evaluate (workerFloat);
						}
					}
					//print ("Name: "+animations [currentAnim [i].index].clipName+"     Layer: "+ animations [currentAnim [i].index].animationLayer);
					animations [currentAnim [i].index].animator.Play (animations [currentAnim [i].index].clipName, animations [currentAnim [i].index].animationLayer, workerFloat);
					for (int k = 0; k < animations[currentAnim[i].index].animationPointTrigger.Length; k++) {
						if (!animations[currentAnim[i].index].animationPointTrigger[k].hasTriggered && workerFloat > animations[currentAnim[i].index].animationPointTrigger[k].percent - animations[currentAnim[i].index].animationPointTrigger[k].range / 2 && workerFloat < animations[currentAnim[i].index].animationPointTrigger[k].percent + animations[currentAnim[i].index].animationPointTrigger[k].range / 2) {
							animations[currentAnim[i].index].animationPointTrigger[k].hasTriggered = true;
							animations[currentAnim[i].index].animationPointTrigger[k].onStateEnter.Invoke ();
						} else if (!animations[currentAnim[i].index].animationPointTrigger[k].hasTriggeredEnd && workerFloat > animations[currentAnim[i].index].animationPointTrigger[k].percent + animations[currentAnim[i].index].animationPointTrigger[k].range / 2) {
							animations[currentAnim[i].index].animationPointTrigger[k].hasTriggeredEnd = true;
							animations[currentAnim[i].index].animationPointTrigger[k].onStateExit.Invoke ();
						}
					}
				} 
				else if (animations [currentAnim [i].index].playLength == -1) {
					currentAnim [i].timer += Time.deltaTime;
					workerFloat = animations [currentAnim [i].index].animationSpeed.Evaluate (((currentAnim [i].timer)/ animations [currentAnim [i].index].clipLength)%1);
					animations [currentAnim [i].index].animator.Play (animations [currentAnim [i].index].clipName, animations [currentAnim [i].index].animationLayer, workerFloat);
				}
				else 
				{
					//EndClip
					currentAnim [i].clipname = "";
					if (animations [currentAnim [i].index].resetWeigthAtEnd) {
						gameObject.GetComponent<Animator> ().SetLayerWeight (animations [currentAnim [i].index].animationLayer, 0f);
					}
					gameObject.GetComponent<Animator> ().SetInteger ("Index", animations [currentAnim [i].index].animatorTransitionIndex);
					if (gameObject.GetComponent<Animator> ().GetInteger ("Index") != 0) {
						StartCoroutine (TransitionBuffer (currentAnim [i].index, animations [currentAnim [i].index].animationLayer));
					}
					animations [currentAnim [i].index].onClipLengthEnd.Invoke ();
				}
			}
		}
	}

	public void Play(int animIndex)
	{
		if (!initialized) {
			StartCoroutine (PlayBuffer (animIndex));
			return;
		}
		readyToPlay = true;
		animations [animIndex].onClipBegin.Invoke ();
		for (int i = 0; i < animations [animIndex].animationPointTrigger.Length; i++) {
			for (int j = 0; j < animations [animIndex].animationPointTrigger.Length; j++) {
				if (animations [animIndex].animationPointTrigger [j].resetOnReplay) {
					animations [animIndex].animationPointTrigger [j].hasTriggered = false;
					animations [animIndex].animationPointTrigger [j].hasTriggeredEnd = false;
				}
			}
		}
		currentAnim [animations [animIndex].animationLayer].clipname = animations [animIndex].clipName;
		currentAnim [animations [animIndex].animationLayer].timer = 0;
		currentAnim [animations [animIndex].animationLayer].index = animIndex;
		gameObject.GetComponent<Animator> ().SetLayerWeight (animations [animIndex].animationLayer, animations [animIndex].weight);
		animations [animIndex].animator.Play (animations [animIndex].clipName, animations [animIndex].animationLayer, 0);
	}

	private IEnumerator PlayBuffer(int animIndex)
	{
		while (!initialized) {
			yield return null; //wait to be initialized
		}
		Play (animIndex);
	}
	
	IEnumerator TransitionBuffer (int animIndex, int layer) {
		while(gameObject.GetComponent<Animator> ().GetAnimatorTransitionInfo (layer).normalizedTime == 0) {
			yield return null;
		}
		while(gameObject.GetComponent<Animator> ().GetAnimatorTransitionInfo (layer).normalizedTime != 0) {
			yield return null;
		}
		animations [animIndex].onTransitionEnd.Invoke ();
	}
}

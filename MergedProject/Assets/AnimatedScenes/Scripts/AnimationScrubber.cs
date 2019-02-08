using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AnimationScrubber : MonoBehaviour {
	
	public bool AJFix = false;
	
	[System.Serializable]
	public struct Animationize {
		public Animator animator;
		public string clipName;
		public int animationLayer;
		public Vector2 timeframe;
		public AnimationCurve animationSpeed;
		public bool curveIsPerCycle;
		public float animationOffset;
		public bool loop;
		[HideInInspector]
		public float clipLength;
	}
	
	[System.Serializable]
	public struct AnimationizeNonHumanoid {
		public Animation animation;
		public string clipName;
		public float startTime;
	}
	
	[System.Serializable]
	public struct Audiobilities {
		public AudioSource audio;
		public Vector2 timeframe;
	}
	
	[System.Serializable]
	public class VoiceOver {
		public float order;
		public AudioClip audio;
		public VOSubtitle[] subtitles;
		public float customPostAudioGap;
		[HideInInspector]
		public Vector2 timeframe;
	}
	
	[System.Serializable]
	public struct VOSubtitle {
		public string line;
		public float time;
		public GameObject[] customizedLine;
		public bool overrideJustification;
	}
	
	[System.Serializable]
	public struct Audioization {
		public AudioSource audioSource;
		public float startTime;
	}
	
	[System.Serializable]
	public struct Annotation {
		public GameObject[] annote;
		public Vector2 timeframe;
		public Color color;
		public Audioization[] audio;
	}
	
	[System.Serializable]
	public enum ArrayToEdit { VoiceOvers, Annotations, Animations, AnimationsNonHumanoid, Audios };
	
	[Header("Module")]
	public DatabaseMessageHolder.Module moduleName;
	
	[Header ("Voice Overs")]
	public float startGap;
	public float betweenGap;
	public float endGap;
	public VoiceOver[] voiceOvers;
	public AudioSource voiceOverAudioSource;
	public bool centerJustify;
	
	[Header ("Timed Events")]
	public Annotation[] annotations;
	public Animationize[] animations;
	public AnimationizeNonHumanoid[] animationsNonHumanoid;
	public Audiobilities[] audios;
	public float overrideMaxTime = 0;
	public bool playOnAwake;
	
	[Header("UI")]
	public Text subtitleText;
	public Slider timeline;
	public GameObject playButton;
	public GameObject pauseButton;
	public Text endTime;
	public Text currentTime;
	public GameObject annotationMarker;
	public Transform annotationHolder;
	
	public DatabaseMessageHolder messageHolder;
	
	[Header("Array Hospital")]
	public ArrayToEdit arrayToEdit;
	public int indexToChange;
	
	// Does not need to be Serialized
	public struct InternalSubtitleStruct {
		public int voiceOverIndex;
		public int subtitleIndex;
		public Vector2 timeframe;
	}
	
	private List<InternalSubtitleStruct> internalizedSubtitles;
	private int activeVoiceOver;
	private bool isPlaying;
	private float timer = 0;
	private float maxTime = 0;
	private float timePlayed = 0;
	private bool passed;
	private List<string> immuneObjects;
	private bool immuneTester;
	private float workerFloat;
	private int cycleCount;
	private float runPercent;
	
	private bool initialized;
	private Keyframe key1;
	private Keyframe key2;
	
	void Start () {
		gameObject.tag = "AnimationScrubber";
		
        // Sort voice over order
        for (int i = 0; i < voiceOvers.Length; i++)
        {
            List<VOSubtitle> tempVOS = new List<VOSubtitle>(voiceOvers[i].subtitles);
            tempVOS.Sort((s1, s2) => s1.time.CompareTo(s2.time));
            voiceOvers[i].subtitles = tempVOS.ToArray();
        }
		
		// Order VOs based on VoiceOver.order
		List<float> orders = new List<float>();
		foreach (VoiceOver v in voiceOvers) {
			if (!orders.Contains(v.order))
				orders.Add(v.order);
		}
		orders.Sort();
		List<VoiceOver> tempVO = new List<VoiceOver>();
		for (int i = 0; i < orders.Count; i++) {
			for (int j = 0; j < voiceOvers.Length; j++) {
				if (voiceOvers[j].order == orders[i]) {
					tempVO.Add(voiceOvers[j]);
				}
			}
		}
		voiceOvers = tempVO.ToArray();
		
		// Sort out Voice Over timing and pointers
		float timeHelper = 0;
		internalizedSubtitles = new List<InternalSubtitleStruct>();
		InternalSubtitleStruct tempInternalSubtitle;
		for (int i = 0; i < voiceOvers.Length; i++) {
			if (i == 0)
				timeHelper += startGap;
			voiceOvers[i].timeframe.x = timeHelper;
			timeHelper += voiceOvers[i].audio.length;
			voiceOvers[i].timeframe.y = timeHelper;
			if (i < voiceOvers.Length-1)
				timeHelper += betweenGap + voiceOvers[i].customPostAudioGap;
			else
				timeHelper += endGap + voiceOvers[i].customPostAudioGap;
			
			for (int j = 0; j < voiceOvers[i].subtitles.Length; j++) {
				tempInternalSubtitle = new InternalSubtitleStruct();
				tempInternalSubtitle.voiceOverIndex = i;
				tempInternalSubtitle.subtitleIndex = j;
				tempInternalSubtitle.timeframe.x = voiceOvers[i].timeframe.x + voiceOvers[i].subtitles[j].time;
				if (j < voiceOvers[i].subtitles.Length-1) {
					tempInternalSubtitle.timeframe.y = voiceOvers[i].timeframe.x + voiceOvers[i].subtitles[j+1].time;
				} else {
					tempInternalSubtitle.timeframe.y = voiceOvers[i].timeframe.y;
				}
				internalizedSubtitles.Add(tempInternalSubtitle);
			}
		}
		
		// Set up defaults for animations that don't need a special curve
		key1 = new Keyframe(0,0);
		key1.outTangent = 0.785398f;
		key2 = new Keyframe(1,1);
		key2.inTangent = 0.785398f;
		
		// Set all animation speeds to 0 so they can be controlled by the scrubber
		for (int i = 0; i < animations.Length; i++) {
			if (animations[i].animationSpeed.length == 0) {
				animations[i].animationSpeed = new AnimationCurve(key1, key2);
			}
			animations[i].animator.speed = 0;
			animations[i].animator.Play(animations[i].clipName);
			/*
			animations[i].deltaTime = animations[i].timeframe.y - animations[i].timeframe.x;
			runPercent = (animations[i].deltaTime * animations[i].speed) / animations[i].animator.GetCurrentAnimatorStateInfo(animations[i].animationLayer).length;
			animations[i].startPercent = animations[i].startTime / animations[i].animator.GetCurrentAnimatorStateInfo(animations[i].animationLayer).length;
			animations[i].endPercent = animations[i].startPercent + runPercent;
			animations[i].speedPercent = animations[i].speed / animations[i].animator.GetCurrentAnimatorStateInfo(animations[i].animationLayer).length;
			*/
			foreach (AnimationClip ac in animations[i].animator.runtimeAnimatorController.animationClips) {
				if (ac.name == animations[i].clipName) {
					animations[i].clipLength = ac.length;
				}
			}
		}
		
		foreach (AnimationizeNonHumanoid a in animationsNonHumanoid) {
			a.animation[a.clipName].speed = 0;
		}
		
		// Stop all audio because we shouldn't be playing any unless the scrubber is playing
		foreach (Audiobilities a in audios) {
			a.audio.Stop();
		}
		
		// Set up the Max Time if there isn't an override time already.  Attempt to use the Voice Overs then attempt to use the Annotations
		if (overrideMaxTime <= 0) {
			if (timeHelper > 0)
				maxTime = timeHelper;
			else if (annotations.Length > 0) {
				for (int i = 0; i < annotations.Length; i++) {
					if (maxTime < annotations[i].timeframe.y)
						maxTime = annotations[i].timeframe.y;
				}
			} else {
				print("No Voice Overs or Annotations Found");
				maxTime = 60;
			}
		} else {
			maxTime = overrideMaxTime;
		}
		
		// Set up the End Time test
		endTime.text = ((int)(maxTime/60)).ToString() + ":";
		if ((int)(maxTime%60) < 10)
			endTime.text += "0" + ((int)(maxTime%60)).ToString();
		else
			endTime.text += ((int)(maxTime%60)).ToString();
		
		// Set up the Visualizations for the Annotations
		GameObject temp;
		foreach (Annotation a in annotations) {
			foreach (GameObject g in a.annote)
				g.SetActive(false);
			temp = (GameObject)Instantiate(annotationMarker, Vector3.zero, Quaternion.identity);
			//temp.transform.parent = annotationHolder;
			temp.transform.SetParent(annotationHolder);
			temp.GetComponent<RectTransform>().sizeDelta = new Vector2(((a.timeframe.y/maxTime) - (a.timeframe.x/maxTime)) * annotationHolder.GetComponent<RectTransform>().rect.width, temp.GetComponent<RectTransform>().rect.height);
			temp.GetComponent<RectTransform>().localPosition = new Vector3((a.timeframe.x/maxTime)*annotationHolder.GetComponent<RectTransform>().rect.width - annotationHolder.GetComponent<RectTransform>().rect.width/2f + temp.GetComponent<RectTransform>().sizeDelta.x/2f, 0, 0);
			temp.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			temp.GetComponent<Image>().color = a.color;
		}
		
		// If playOnAwake is set, then we need to play... on awake.  It's actually on start not awake so technically this is a lie but TOO BAD.
		if (playOnAwake) {
			Play();
		}
		
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("MessageHolder")) {
			if (go.GetComponent<DataLoader>())
				messageHolder = go.GetComponent<DatabaseMessageHolder>();
		}
		
		if (!messageHolder && GameObject.FindWithTag("MessageHolder"))
			messageHolder = GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>();
		
		if (!messageHolder)
			return;
		
		messageHolder.WriteMessage("Training Started");
		messageHolder.PushingMessages();
		
		StartCoroutine(Initialize());
	}
	
	IEnumerator Initialize () {
		yield return null;
		initialized = true;
	}
	
	// Play the scrubber from the current time
	public void Play () {
		if (!isPlaying) {
			isPlaying = true;
			playButton.SetActive(false);
			pauseButton.SetActive(true);
			StartCoroutine(RunAnimation());
			return;
		}
		//for (int i = 0; i < animations.Length; i++) {
		//	animations[i].animator.speed = 0;
		//}
		isPlaying = false;
		playButton.SetActive(true);
		pauseButton.SetActive(false);
	}
	
	// Pause at the current time
	public void TimeLinePause () {
		isPlaying = false;
		playButton.SetActive(true);
		pauseButton.SetActive(false);
	}
	
	// Update the time of te scrubber which ultimately propogates through various scripts looking at the timer
	public void UpdateTime () {
		if (!isPlaying) {
			timer = timeline.value * maxTime;
			UpdateTimer();
		}
	}
	
	// Returns the current time
	public float GetTime () {
		return timer;
	}
	
	// Returns whether or not the scrubber is playing
	public bool IsPlaying () {
		return isPlaying;
	}
	
	// A coroutine that handles propogation through this script during play
	IEnumerator RunAnimation () {
		yield return null;
		if (timer >= maxTime) {
			timer = 0;
			UpdateTimer();
			timeline.value = 0;
		}
		while (isPlaying && timer < maxTime) {
			timer += Time.deltaTime;
			UpdateTimer();
			timeline.value = timer/maxTime;
			if (!passed && messageHolder) {
				timePlayed += Time.deltaTime;
				if (timePlayed >= maxTime-1f) {
					passed = true;
					messageHolder.WriteMessage("Video complete.");
					messageHolder.CompleteModule();
				}
			}
			yield return null;
		}
		isPlaying = false;
		playButton.SetActive(true);
		pauseButton.SetActive(false);
		if (timer > maxTime) {
			timer = maxTime;
			UpdateTimer();
			timeline.value = 1;
		}
	}
	
	void Update () {
		if (!initialized)
			return;
		if (immuneObjects == null) {
			immuneObjects = new List<string>();
		}
		// Best Feature N.A.
		if (Input.GetKeyDown(KeyCode.Space))
			Play();
		
		// For backwards compatibility, if we don't have a VOAudio Source, assume it's an older video that doesn't use this entire module
		if (voiceOverAudioSource) {
			if (voiceOvers.Length > 0) {
				// Sort out which Voice Over we should be using
				if (voiceOverAudioSource.isPlaying && !isPlaying)
					voiceOverAudioSource.Stop();
				if (activeVoiceOver < 0 && timer >= voiceOvers[0].timeframe.x) {
					activeVoiceOver = 0;
				} else if (activeVoiceOver >= voiceOvers.Length && timer <= voiceOvers[voiceOvers.Length-1].timeframe.y) {
					activeVoiceOver = voiceOvers.Length-1;
				}
				while (activeVoiceOver >= 0 && activeVoiceOver < voiceOvers.Length && timer < voiceOvers[activeVoiceOver].timeframe.x) {
					activeVoiceOver--;
				}
				while (activeVoiceOver >= 0 && activeVoiceOver < voiceOvers.Length && timer > voiceOvers[activeVoiceOver].timeframe.y) {
					activeVoiceOver++;
				}
				
				// Play the clip of the VO we've chosen.
				if (activeVoiceOver >= 0 && activeVoiceOver < voiceOvers.Length) {
					if (voiceOverAudioSource.clip != voiceOvers[activeVoiceOver].audio)
						voiceOverAudioSource.clip = voiceOvers[activeVoiceOver].audio;
					if (voiceOverAudioSource.clip && !voiceOverAudioSource.isPlaying && isPlaying && timer >= voiceOvers[activeVoiceOver].timeframe.x && timer <= voiceOvers[activeVoiceOver].timeframe.y) {
						voiceOverAudioSource.time = timer - voiceOvers[activeVoiceOver].timeframe.x;
						voiceOverAudioSource.Play();
					}
					
					// Grab the appropriate subtitle
					subtitleText.text = "";
					foreach (InternalSubtitleStruct i in internalizedSubtitles) {
						if (i.voiceOverIndex == activeVoiceOver && timer >= i.timeframe.x && timer <= i.timeframe.y) {
                            string displayText = voiceOvers[i.voiceOverIndex].subtitles[i.subtitleIndex].line;
                            if ((centerJustify != voiceOvers[i.voiceOverIndex].subtitles[i.subtitleIndex].overrideJustification) && displayText.Length > 60)
                            {
                                int cut = displayText.Length / 2;
                                int forward = cut + 1;
                                int backward = cut - 1;
                                while (displayText[cut] != ' ' || forward == displayText.Length || backward == -1)
                                {
                                    if (displayText[forward] == ' ') { cut = forward; }
                                    else if (displayText[backward] == ' ') { cut = backward; }
                                    else { forward++; backward--; }
                                }
                                displayText = displayText.Substring(0, cut) + "\n" + displayText.Substring(cut + 1);
                            }
							subtitleText.text = displayText;
							foreach (GameObject g in voiceOvers[i.voiceOverIndex].subtitles[i.subtitleIndex].customizedLine) {
								g.SetActive(true);
							}
						} else {
							foreach (GameObject g in voiceOvers[i.voiceOverIndex].subtitles[i.subtitleIndex].customizedLine) {
								g.SetActive(false);
							}
						}
					}
				}
			}
		} else {
			print("Voice Over Audio Source not found, assuming older video that doesn't use this module.");
		}
		
		// Play animations as they are timed using the start time as the anchor to the timeframe and the speed as the direction and intensity of the playback
		for (int i = 0; i < animations.Length; i++) {
			if (timer >= animations[i].timeframe.x && timer <= animations[i].timeframe.y) {
				if (animations[i].curveIsPerCycle) {
					if (animations[i].loop) {
						workerFloat = ((timer-animations[i].timeframe.x)/(animations[i].clipLength) + animations[i].animationOffset / animations[i].clipLength);
						workerFloat %= 1f;
					} else {
						if(AJFix)
							workerFloat = (timer-animations[i].timeframe.x) / (animations[i].timeframe.y-animations[i].timeframe.x);
						else
							workerFloat = animations[i].animationSpeed.Evaluate((timer - animations[i].timeframe.x)/(animations[i].timeframe.y - animations[i].timeframe.x));
						workerFloat = Mathf.Clamp(workerFloat, 0, 1);
					}
					workerFloat = animations[i].animationSpeed.Evaluate(workerFloat);
				} else {
					if (animations[i].loop) {
						workerFloat = animations[i].animationSpeed.Evaluate((timer-animations[i].timeframe.x)/(animations[i].timeframe.y - animations[i].timeframe.x)  + animations[i].animationOffset/animations[i].clipLength) * ((animations[i].timeframe.y - animations[i].timeframe.x)/animations[i].clipLength);
						workerFloat %= 1f;
					} else {
						workerFloat = ((timer-animations[i].timeframe.x)/(animations[i].clipLength) + animations[i].animationOffset / animations[i].clipLength);
						workerFloat = Mathf.Clamp(workerFloat, 0, 1);
						workerFloat = animations[i].animationSpeed.Evaluate(workerFloat);
					}
				}
				
				//if (isPlaying)
				//	animations[i].animator.speed = 1;
				animations[i].animator.Play(animations[i].clipName, animations[i].animationLayer, workerFloat);
			}
		}
		
		for (int i = 0; i < animationsNonHumanoid.Length; i++) {
			// If the timer is less than the animation length plus the start time then set the time to the timer minus the start time (So when the timer reaches the start time, the animation starts [from frame zero])
			if (timer < animationsNonHumanoid[i].animation.clip.length + animationsNonHumanoid[i].startTime)
				animationsNonHumanoid[i].animation[animationsNonHumanoid[i].clipName].time = timer - animationsNonHumanoid[i].startTime;
			else
				animationsNonHumanoid[i].animation[animationsNonHumanoid[i].clipName].time = animationsNonHumanoid[i].animation.clip.length;
		}
		
		foreach (Audiobilities a in audios) {
			// If playing and after the start time of the audio and either before the end time or if the end time is less than or equal to the start time, play the audio
			if (isPlaying && timer >= a.timeframe.x && (a.timeframe.y <= a.timeframe.x || timer <= a.timeframe.y)) {
				if (!a.audio.isPlaying)
					a.audio.Play();
			} else
				a.audio.Stop();
		}
		
		// Ok so...  there's a lot going on here.
		for (int i = 0; i < annotations.Length; i++) {
			// If the timer is between the time frame of the annotation, turn it on
			if (timer >= annotations[i].timeframe.x && timer <= annotations[i].timeframe.y) {
				if (!isPlaying) {
					// If not playing, stop the audio and set it to the proper time (Adjusting for the annotation start time and audio start time.
					// This keeps the audio start time relative to the annotation.
					for (int j = 0; j < annotations[i].audio.Length; j++) {
						annotations[i].audio[j].audioSource.Stop();
						annotations[i].audio[j].audioSource.time = Mathf.Clamp(timer - annotations[i].timeframe.x - annotations[i].audio[j].startTime, 0, annotations[i].audio[j].audioSource.clip.length - 0.0000001f);
					}
				} else {
					// If playing, and beyond the audio start time within the annotation, play the audio.
					for (int j = 0; j < annotations[i].audio.Length; j++) {
						if (timer >= annotations[i].timeframe.x + annotations[i].audio[j].startTime && timer <= annotations[i].timeframe.x + annotations[i].audio[j].startTime + annotations[i].audio[j].audioSource.clip.length) {
							if (!annotations[i].audio[j].audioSource.isPlaying) { // Only adjust the audio time and start it playing ONCE per play.
								annotations[i].audio[j].audioSource.time = Mathf.Clamp(timer - annotations[i].timeframe.x - annotations[i].audio[j].startTime, 0, annotations[i].audio[j].audioSource.clip.length - 0.0000001f);
								annotations[i].audio[j].audioSource.Play();
							}
						} else {
							// Stop it if we're not within the timeframe of the audio.
							annotations[i].audio[j].audioSource.Stop();
						}
					}
				}
				// Turn on all of the Annotation stuff
				foreach (GameObject g in annotations[i].annote) {
						g.SetActive(true);
						immuneObjects.Add(g.name);
				}
			} else {
				// If we're outside of the timeframe of the annotation, stop the audio and turn the annotation off.
				for (int j = 0; j < annotations[i].audio.Length; j++) {
					annotations[i].audio[j].audioSource.Stop();
				}
				foreach (GameObject g in annotations[i].annote) {
					//immuneTester = false;
					//foreach (string s in immuneObjects) {
					//	if (s == g.name) {
					//		immuneTester = true;
					//		break;
					//	}
					//}
					//g.SetActive(immuneTester);
					g.SetActive(false);
				}
			}
		}
	}
	
	void UpdateTimer () {
		// Update the text for the current timestamp.
		currentTime.text = ((int)(timer/60)).ToString() + ":";
		if ((int)(timer%60) < 10)
			currentTime.text += "0" + ((int)(timer%60)).ToString();
		else
			currentTime.text += ((int)(timer%60)).ToString();
	}
	
	public void EditArray (bool doAdd) {
		switch ((int)arrayToEdit) {
			case 0: // Voice Overs
				voiceOvers = DoArrayEdit<VoiceOver>(voiceOvers, doAdd);
				break;
			case 1: // Annotations
				annotations = DoArrayEdit<Annotation>(annotations, doAdd);
				break;
			case 2: // Animations
				animations = DoArrayEdit<Animationize>(animations, doAdd);
				break;
			case 3: // AnimationsNonHumanoid
				animationsNonHumanoid = DoArrayEdit<AnimationizeNonHumanoid>(animationsNonHumanoid, doAdd);
				break;
			case 4: // Audios
				audios = DoArrayEdit<Audiobilities>(audios, doAdd);
				break;
		}
	}
	
	T[] DoArrayEdit<T> (T[] changedList, bool doAdd) where T: new() {
		if (indexToChange >= changedList.Length)
			return changedList;
		List<T> list = new List<T>(changedList);
		if (doAdd) {
			list.Insert(indexToChange, new T());
		} else {
			list.RemoveAt(indexToChange);
		}
		changedList = list.ToArray();
		return changedList;
	}
}
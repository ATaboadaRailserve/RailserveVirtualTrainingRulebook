using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BasicQuester : MonoBehaviour {
	
	[System.Serializable]
	public struct Step {
		public string instructions;
		public AudioClip instructionAudio;
		public Transform objectivePoint;
		public TeleportTo restartPoint;
		public InteractionHandler.InvokableState onStepBegin;
		public InteractionHandler.InvokableState onStepEnd;
		public InteractionHandler.InvokableState onStepFail;
	}
	
	public Step[] steps;
	public Text currentInstructions;
	public Compass compass;
	public AudioSource instructionAudioSource;

	[Header("Array Hospital")]
	public int indexToChange;
	
	private int index = -1;
	
	void Start () {
		Next();
	}
	
	public void Next () {
		if (index >= 0)
			steps[index].onStepEnd.Invoke();
		if (index < steps.Length-1) {
			index++;
			steps[index].onStepBegin.Invoke();
			if (compass) {
				if (steps[index].objectivePoint != null) {
					compass.gameObject.SetActive(true);
					compass.target = steps[index].objectivePoint;
				} else
					compass.gameObject.SetActive(false);
			}
			currentInstructions.text = steps[index].instructions;
			if (steps[index].instructionAudio) {
				instructionAudioSource.clip = steps[index].instructionAudio;
				instructionAudioSource.Play();
			}
		}
	}
	
	public void JumpToIndex (int toIndex) {
		if (toIndex < 0 || toIndex > steps.Length) {
			Debug.Log("Index out of bounds of steps array");
			return;
		}
		if (index >= 0)
			steps[index].onStepEnd.Invoke();
		index = toIndex;
		steps[index].onStepBegin.Invoke();
		if (compass) {
				if (steps[index].objectivePoint != null) {
					compass.gameObject.SetActive(true);
					compass.target = steps[index].objectivePoint;
				} else
					compass.gameObject.SetActive(false);
			}
		currentInstructions.text = steps[index].instructions;
		if (steps[index].instructionAudio) {
			instructionAudioSource.clip = steps[index].instructionAudio;
			instructionAudioSource.Play();
		}
	}
	
	public void FailObjectiveRestart (int toIndex) {
		if (toIndex < 0 || toIndex > steps.Length) {
			Debug.Log("Index out of bounds of steps array");
			return;
		}
		if (index >= 0)
			steps[index].onStepFail.Invoke(); // Rather than onStepEnd'ing, have it call a special onStepFail
		index = toIndex;
		if (steps[index].restartPoint != null)
			steps[index].restartPoint.Teleport();
		steps[index].onStepBegin.Invoke();
		if (compass) {
			if (steps[index].objectivePoint != null) {
				compass.gameObject.SetActive(true);
				compass.target = steps[index].objectivePoint;
			} else
				compass.gameObject.SetActive(false);
		}
		currentInstructions.text = steps[index].instructions;
		if (steps[index].instructionAudio) {
			instructionAudioSource.clip = steps[index].instructionAudio;
			instructionAudioSource.Play();
		}
	}
	
	public void CompletedAllQuests () {
		GameObject.FindWithTag("MessageHolder").GetComponent<DatabaseMessageHolder>().CompleteModule();
	}

	public void EditArray(bool doAdd)
	{
		List<Step> list = new List<Step>(steps);
		if (doAdd) {
			list.Insert (indexToChange, new Step ());
		} else {
			list.RemoveAt (indexToChange);
		}
		steps = list.ToArray();
	}
}

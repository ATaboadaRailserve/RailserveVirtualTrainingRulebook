using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SwitchTutorial : MonoBehaviour {
	//------------------------------Public-----------------------------
	public List<TutorialStep> tutorialStepList = new List<TutorialStep>();
	public Text screenUIText;

	//------------------------------Public Hidden----------------------
	[HideInInspector]
	public int current = 0;

	//------------------------------Private----------------------------
	private AudioSource audio_s;

	void Start () 
	{
		for (int i = 0; i < tutorialStepList.Count; i++) 
		{
			tutorialStepList [i].step = i;
		}
		audio_s = GetComponent<AudioSource> ();
		audio_s.clip = tutorialStepList [0].audio_c;
		StartCoroutine ( WaitToStart(3));
	}

	public void NextStep(int current_step)
	{
		if (current == current_step) {
			tutorialStepList [current_step].ui_board.SetActive (false);
			if (current_step + 1 < tutorialStepList.Count) {
				tutorialStepList [current_step + 1].ui_board.SetActive (true);
				audio_s.Stop ();
				audio_s.clip = tutorialStepList [current_step + 1].audio_c;
				audio_s.Play ();
				current += 1;
				Text tempt = tutorialStepList [current_step + 1].ui_board.GetComponentInChildren<Text> ();
				screenUIText.text = tempt.text;
			}
		}
	}
	public IEnumerator WaitToStart(float i)
	{
		yield return new WaitForSeconds (i);
		audio_s.Play ();
	}
}
[System.Serializable]
public class TutorialStep
{
	[HideInInspector]
	public int step;
	public GameObject ui_board;
	public AudioClip audio_c;
	public TutorialStep(int step_i, GameObject ui_board_g)
	{
		step = step_i;
		ui_board = ui_board_g;
	}
}

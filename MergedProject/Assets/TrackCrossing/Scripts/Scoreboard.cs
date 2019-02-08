using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {
	
	[Header("Warning Systems")]
	public WarningSystem warningSystem;
	public DatabaseMessageHolder messageHolder;
	
	[Header("UI Stuff")]
	public GameObject scoreBoard;
	public GameObject instructionsObject;
	public Text trackCrossing;
	public Text redZone;
	public Text total;
	public Text instructions;
	public GameObject retryButton;
	public GameObject nextButton;
	
	void Start () {
		scoreBoard.SetActive(false);
	}
	
	void GetScore () {
		
		int[] segmentCredit = new int[1]{0};
		int[] segmentWarn = new int[2]{0,2};
		trackCrossing.text = warningSystem.SumSegment(segmentCredit, segmentWarn).ToString();
		
		segmentCredit = new int[1]{1};
		segmentWarn = new int[1]{1};
		redZone.text = warningSystem.SumSegment(segmentCredit, segmentWarn).ToString();
		
		total.text = warningSystem.SumTotal().ToString();
		
		if (warningSystem.SumTotal() == 100) {
			retryButton.SetActive(false);
			nextButton.SetActive(true);
			instructions.text = "Congratulations, you may proceed to the next segment";
		}
		scoreBoard.SetActive(true);
		instructionsObject.SetActive(true);
		
		if (warningSystem.SumTotal() >= 100)
			messageHolder.moduleFinished = true;
		messageHolder.WriteMessage(warningSystem.SumTotal().ToString(), 4);
		messageHolder.PushingMessages();
	}
	
	void OnTriggerEnter (Collider col) {
		GetScore();
		col.SendMessage("ToggleMove", false);
	}
}

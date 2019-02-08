using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoardTemp : MonoBehaviour {

	[Header("Warning System")]
	public WarningSystem warningSystem;

	[Header("UI Stuff")]
	public GameObject scoreBoard;
	public Text switchCompleteness;
	public Text unsafeSwitchState;
	public Text incorrectOperationalPosition;
	public Text total;
	
	private UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;
	private Camera cam;
	private DatabaseMessageHolder messageHolder;
	

	void Start () {
		scoreBoard.SetActive(false);
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		fpsController = cam.transform.parent.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();
		messageHolder = GameObject.FindWithTag ("MessageHolder").GetComponent<DatabaseMessageHolder> ();
	}
	public void GetScoreSwitchGame()
	{
		int[] segmentCredit = new int[1]{0};
		int[] segmentWarn = new int[1]{0};
		switchCompleteness.text = warningSystem.SumSegment(segmentCredit, segmentWarn).ToString();
		fpsController.enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		scoreBoard.SetActive(true);
		//messageHolder.PushingMessages();
	}
	void GetScore () {

		int[] segmentCredit = new int[1]{0};
		int[] segmentWarn = new int[1]{0};
		incorrectOperationalPosition.text = warningSystem.SumSegment(segmentCredit, segmentWarn).ToString();

		segmentCredit = new int[1]{1};
		segmentWarn = new int[1]{1};
		unsafeSwitchState.text = warningSystem.SumSegment(segmentCredit, segmentWarn).ToString();

		segmentCredit = new int[1]{2};
		segmentWarn = new int[1]{2};
		switchCompleteness.text = warningSystem.SumSegment(segmentCredit, segmentWarn).ToString();

		total.text = warningSystem.SumTotal().ToString();
		fpsController.enabled = false;
		Cursor.visible = true;
		scoreBoard.SetActive(true);

	}

	void OnTriggerEnter (Collider col) {
		GetScore();
		//col.SendMessage("ToggleMove", false);
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrossingGenerator : MonoBehaviour {
	
	public DatabaseMessageHolder messageHolder;
	
	public GameObject floor;
	public GameObject[] rails;
	public AnimationCurve railRarity;
	public int index = 0;
	public int score = 0;
	public int lead = 8;
	public int trail = 2;
	public Transform hitboxes;
	public GameObject start;
	
	public GameObject player;
	public GameObject scoreBoard;
	public GameObject retryButton;
	public GameObject continueButton;
	public GameObject finalInstructionsObject;
	public Text finalInstructions;
	public Text instructions;
	public Text scoreText;
	public Text activeScoreText;
	
	private int random;
	private GameObject[] liveRails;
	private int zLocation = 6;
	private bool gameOver;
	private bool checkedTen;
	private int[] lastTypes;
	
	private bool zFighter;
	private float zAdjust = 0.005f;
	
	private float raiseTime = 1f;
	
	void Start () {
		lastTypes = new int[2];
		lastTypes[0] = -1;
		lastTypes[1] = -1;
		liveRails = new GameObject[lead+trail];
		SpawnRail();
	}
	
	void SpawnRail () {
		if (liveRails[lead+trail-1]) {
			StartCoroutine(LowerTheRail(liveRails[0].transform));
			for (int i = 0; i < liveRails.Length-1; i++) {
				liveRails[i] = liveRails[i+1];
			}
			zFighter = !zFighter;
			GameObject temp = (GameObject)Instantiate(floor, new Vector3((zFighter ? zAdjust : 0),(zFighter ? zAdjust : 0),zLocation), Quaternion.identity);
			random = (int)Mathf.Round(railRarity.Evaluate(Random.value)*rails.Length) - 1;
			while (lastTypes[0] == 0 && lastTypes[1] == 0 && random == 0) {
				random = (int)Mathf.Round(railRarity.Evaluate(Random.value)*rails.Length) - 1;
			}
			lastTypes[0] = lastTypes[1];
			lastTypes[1] = random;
			if (random > -1) {
				liveRails[lead+trail-1] = (GameObject)Instantiate(rails[random], new Vector3((zFighter ? zAdjust : 0),(zFighter ? zAdjust : 0),zLocation), Quaternion.identity);
				liveRails[lead+trail-1].transform.parent = transform;
				temp.transform.parent = liveRails[lead+trail-1].transform;
				temp.GetComponent<MiddleBitsCrossing>().game = false;
			} else {
				liveRails[lead+trail-1] = temp;
				liveRails[lead+trail-1].transform.parent = transform;
			}
			StartCoroutine(RaiseTheRail(liveRails[lead+trail-1].transform, (zFighter ? zAdjust : 0)));
			zLocation += 6;
		} else {
			for (int i = 0; i < liveRails.Length; i++) {
				zFighter = !zFighter;
				GameObject temp = (GameObject)Instantiate(floor, new Vector3((zFighter ? zAdjust : 0),(zFighter ? zAdjust: 0),zLocation), Quaternion.identity);
				random = (int)Mathf.Round(railRarity.Evaluate(Random.value)*rails.Length) - 1;
				if (random > -1) {
					liveRails[i] = (GameObject)Instantiate(rails[random], new Vector3((zFighter ? zAdjust : 0),(zFighter ? zAdjust : 0),zLocation), Quaternion.identity);
					liveRails[i].transform.parent = transform;
					temp.transform.parent = liveRails[i].transform;
					temp.GetComponent<MiddleBitsCrossing>().game = false;
				} else {
					liveRails[i] = temp;
					liveRails[i].transform.parent = transform;
				}
				zLocation += 6;
			}
		}
	}
	
	IEnumerator RaiseTheRail (Transform rail, float target) {
		for (float i = 0; i < raiseTime; i += Time.deltaTime) {
			rail.position = Vector3.Lerp(new Vector3(rail.position.x, -20, rail.position.z), new Vector3(rail.position.x, target, rail.position.z), i);
			yield return null;
		}
		rail.position = new Vector3(rail.position.x, target, rail.position.z);
	}
	
	IEnumerator LowerTheRail (Transform rail) {
		for (float i = 0; i < raiseTime; i += Time.deltaTime) {
			rail.position = Vector3.Lerp(new Vector3(rail.position.x, 0, rail.position.z), new Vector3(rail.position.x, -20, rail.position.z), i);
			yield return null;
		}
		Destroy(rail.gameObject);
	}
	
	public void ScoreUp (bool doScore) {
		if (gameOver)
			return;
		
		if (doScore) {
			score++;
			activeScoreText.text = "Score: " + score.ToString() + " ";
		}
		index++;
		if (start && index >= trail)
			Destroy(start);
		hitboxes.position += new Vector3(0,0,6);
		if (index > trail) {
			SpawnRail();
		}
		
		if (score == 10) {
			GameOver(true);
		}
	}
	
	public void GameOver (bool firstTen) {
		if (gameOver || checkedTen)
			return;
		GameObject[] paths = GameObject.FindGameObjectsWithTag("RailPath");
		foreach (GameObject g in paths) {
			g.SendMessage("NukeSpeed");
		}
		gameOver = true;
		player.SendMessage("ToggleMove", false);
		
		retryButton.SetActive(false);
		continueButton.SetActive(true);
		finalInstructions.text = "Congratulations, you may keep going to try for the high score or return to the menu at any point";
		finalInstructionsObject.SetActive(true);
		scoreBoard.SetActive(true);
		scoreText.text = score.ToString();
		checkedTen = true;
		if (score >= 10)
			messageHolder.moduleFinished = true;
		messageHolder.WriteMessage(score.ToString(), 4);
		messageHolder.PushingMessages();
	}
	
	public void GameOver (string ruleName) {
		print("Called");
		if (gameOver)
			return;
		GameObject[] paths = GameObject.FindGameObjectsWithTag("RailPath");
		foreach (GameObject g in paths) {
			g.SendMessage("NukeSpeed");
		}
		gameOver = true;
		player.SendMessage("ToggleMove", false);
		
		retryButton.SetActive(true);
		continueButton.SetActive(false);
		finalInstructions.text = ruleName;
		StartCoroutine(GameOverTimer());
	}
	
	IEnumerator GameOverTimer () {
		yield return new WaitForSeconds(GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>().warningTime);
		scoreBoard.SetActive(true);
		finalInstructionsObject.SetActive(true);
		scoreText.text = score.ToString();
		if (score >= 10)
			messageHolder.moduleFinished = true;
		messageHolder.WriteMessage(score.ToString(), 4);
		messageHolder.PushingMessages();
	}
	
	public void Continue () {
		GameObject[] paths = GameObject.FindGameObjectsWithTag("RailPath");
		foreach (GameObject g in paths) {
			g.SendMessage("ResumeSpeed");
		}
		gameOver = false;
		instructions.text = "Achieve the highest score.";
		player.SendMessage("ToggleMove", true);
		scoreBoard.SetActive(false);
		finalInstructionsObject.SetActive(false);
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class TempTutEnter : MonoBehaviour {
	
	public void LoadTutorial () {
		SceneManager.LoadScene("FirstStartTutorial");
	}
}

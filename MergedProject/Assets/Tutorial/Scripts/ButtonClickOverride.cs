using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class ButtonClickOverride : MonoBehaviour {
	
	public Camera cameraCam;
	public DataLoader dataLoader;
	public Text welcomeText;
	
	private Player _player;
			
	private Player player {
		get {
			// Get the Rewired Player object for this player. Refresh it as needed so it will get the new reference after a script recompile in the editor.
			if(_player == null) _player = ReInput.players.GetPlayer(0);
			return _player;
		}
	}
	
	void Start()
	{
		dataLoader = GameObject.FindWithTag("DataLoader").GetComponent<DataLoader>();
		
		if(dataLoader.onlineMode)
			welcomeText.text = "Welcome, " + dataLoader.CurrentUser.FirstName + " " + dataLoader.CurrentUser.LastName;
		else
			welcomeText.text = "Welcome, " + dataLoader.CurrentUser.Username;
	}
	
	void Update () {
		if (player.GetButtonUp("Fire1")) {
			Ray ray = cameraCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.transform.parent && hit.collider.transform.parent.GetComponent<Button>())
					hit.collider.transform.parent.GetComponent<Button>().onClick.Invoke();
			}
		}
	}
}

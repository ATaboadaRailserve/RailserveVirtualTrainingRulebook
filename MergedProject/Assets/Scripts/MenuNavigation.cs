using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class MenuNavigation : MonoBehaviour {
	
	public InputField[] inputFields;
	public Button submit;
	public Button cancel;
	
	private int index = 0;
	
	private Player _player;
	
	private int buttonPress = 0;
	private bool nextFieldPressed;
	
	private Player player {
		get {
			// Get the Rewired Player object for this player. Refresh it as needed so it will get the new reference after a script recompile in the editor.
			if(_player == null) _player = ReInput.players.GetPlayer(0);
			return _player;
		}
	}
	
	void Update () {
		for (int i = 0; i < inputFields.Length; i++) {
			if (inputFields[i].isFocused) {
				index = i;
				break;
			}
		}
		
		if (!nextFieldPressed && player.GetButtonDown("NextField")) {
			nextFieldPressed = true;
			buttonPress++;
			index++;
			if (index > inputFields.Length-1)
				index = 0;
			inputFields[index].ActivateInputField();
			inputFields[index].Select();
		} else if (nextFieldPressed && !player.GetButtonDown("NextField")) {
			nextFieldPressed = false;
		}
		
		if (player.GetButtonDown("Submit")) {
			submit.onClick.Invoke();
		}
		if (player.GetButtonDown("Cancel") && cancel != null) {
			cancel.onClick.Invoke();
		}
	}
}

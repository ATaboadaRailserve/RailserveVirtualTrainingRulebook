using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	
	public int scene;
	public Slider sensativitySlider;
	public MouseLook mouseLook;
	public Image cheatButton;
	public CharacterMotor player;
	public float lowSpeed = 1.34f;
	public float highSpeed = 10f;
	
	private bool cheatMode;
	private Color buttonColor;
	
	void Start () {
		buttonColor = cheatButton.color;
	}
	
	public void ChangeLevel () {
		Application.LoadLevel(scene);
	}
	
	public void ResetLevel () {
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void SetSensativity () {
		mouseLook.SetSensativity(sensativitySlider.value);
	}
	
	public void CheatMode () {
		cheatMode = !cheatMode;
		if (cheatMode) {
			player.movement.maxForwardSpeed = highSpeed;
			player.movement.maxSidewaysSpeed = highSpeed;
			player.movement.maxBackwardsSpeed = highSpeed;
			cheatButton.color = Color.red;
		} else {
			player.movement.maxForwardSpeed = lowSpeed;
			player.movement.maxSidewaysSpeed = lowSpeed;
			player.movement.maxBackwardsSpeed = lowSpeed;
			cheatButton.color = buttonColor;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMessageHandler : MonoBehaviour {
	
	private InteractionHandler interactionHandler;
	private InfiniGen generator;
	
	void Awake () {
		interactionHandler = GameObject.FindWithTag("InteractionHandler").GetComponent<InteractionHandler>();
		generator = (InfiniGen)GameObject.FindObjectOfType(typeof(InfiniGen));
	}
	
	public void Win () {
		interactionHandler.Win();
	}
	
	public void Lose () {
		interactionHandler.Lose();
	}
	
	public void ReturnToMenu () {
		interactionHandler.ReturnToMenu();
	}
	
	public void Restart () {
		interactionHandler.Restart();
	}
	
	public void InvokeAction (string actionName) {
		interactionHandler.InvokeAction(actionName);
	}
	
	public void PlayerCanWalk (bool state) {
		interactionHandler.PlayerCanWalk = state;
	}
	
	public void PlayerCanLook (bool state) {
		interactionHandler.PlayerCanLook = state;
	}
	
	public void PlayerCanCursorOver (bool state) {
		interactionHandler.PlayerCanCursorOver = state;
	}
	
	public void PlayerCanInteract (bool state) {
		interactionHandler.PlayerCanInteract = state;
	}
	
	public void LockInteraction (bool state) {
		interactionHandler.LockInteraction = state;
	}
	
	public void PlayerIsKinematic (bool state) {
		interactionHandler.PlayerIsKinematic = state;
	}
	
	public void CursorLock (int state) {
		interactionHandler.CursorLock = state;
	}
	
	public void SetInteractionText (string text = "") {
		interactionHandler.SetInteractionText(text);
	}
	
	public void CustomGameState (string state) {
		interactionHandler.CustomGameState(state);
	}
	
	public void ScorePoint()
	{
		generator.ScorePoint();
	}
}

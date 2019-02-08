using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTo : MonoBehaviour {
	
	public InteractionHandler interactionHandler;
	public bool playerCanMoveAfterTeleport;
	public bool unparentAfterTeleport;
	public Transform target;
	public Transform lookTarget;
	public float fadeTime = 0.5f;
	public Fader fader;
	
	public InteractionHandler.InvokableState onTeleportStart;
	public InteractionHandler.InvokableState onTeleportEnd;
	
	public void Teleport () {
		onTeleportStart.Invoke();
		fader.FadeToBlack(fadeTime);
		StartCoroutine(TP());
	}
	
	IEnumerator TP () {
		yield return new WaitForSeconds(fadeTime);
		if (unparentAfterTeleport) {
			interactionHandler.playerController.transform.parent = null;
		}
		interactionHandler.playerController.transform.position = target.position;
		
		if (lookTarget) {
			interactionHandler.playerController.ForceCamera(lookTarget);
		}
		
		interactionHandler.PlayerIsKinematic = !playerCanMoveAfterTeleport;
		interactionHandler.PlayerCanWalk = playerCanMoveAfterTeleport;
		fader.FadeToClear(fadeTime);
		onTeleportEnd.Invoke();
	}
}
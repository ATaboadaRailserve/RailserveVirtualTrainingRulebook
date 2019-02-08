using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneHandler : MonoBehaviour {
	[System.Serializable]
	public struct CutScene {
		public Transform startLocation;
		public Transform endLocation;

		public float timeToMove; //how long the movement takes
		public AnimationCurve movementCurve;
		public bool returnToPlayerOnEnd; //returns control to the player when done moving

		public InteractionHandler.InvokableState onStartMove;
		public InteractionHandler.InvokableState onEndMove;
	}

	public InteractionHandler interactionHandler;
	public Transform cutsceneCamera;
	public CutScene[] cutScenes;

	public void Play(int index)
	{
		StartCoroutine (PlayCutScene (index));
	}

	public void MatchCamera()
	{
		cutsceneCamera.position = interactionHandler.interactionCamera.transform.position;
		cutsceneCamera.rotation = interactionHandler.interactionCamera.transform.rotation;
	}

	public void SetActiveCamera(bool SetToCutScene)
	{
		if (SetToCutScene) {
			cutsceneCamera.gameObject.SetActive (true);
			interactionHandler.interactionCamera.gameObject.SetActive (false);
		}
		else {
			interactionHandler.interactionCamera.gameObject.SetActive (true);
			cutsceneCamera.gameObject.SetActive (false);

		}
	}

	void FreezePlayer(bool isFrozen)
	{
		interactionHandler.PlayerCanCursorOver = !isFrozen;
		interactionHandler.PlayerCanInteract = !isFrozen;
		interactionHandler.PlayerCanLook = !isFrozen;
		interactionHandler.PlayerCanWalk = !isFrozen;
	}

	IEnumerator PlayCutScene(int index)
	{
		cutScenes[index].onStartMove.Invoke();

		MatchCamera ();
		//SetActiveCamera (true);
		FreezePlayer (true);

		float timer = 0;
		while (timer < 1) {
			timer += Time.deltaTime / cutScenes[index].timeToMove;
			cutsceneCamera.position = Vector3.Lerp(cutScenes[index].startLocation.position, cutScenes[index].endLocation.position, Mathf.Clamp(cutScenes[index].movementCurve.Evaluate(timer), 0, 1));
			cutsceneCamera.rotation = Quaternion.Lerp(cutScenes[index].startLocation.rotation, cutScenes[index].endLocation.rotation, Mathf.Clamp(cutScenes[index].movementCurve.Evaluate(timer), 0, 1));
			yield return null;
		}

		if (cutScenes [index].returnToPlayerOnEnd) {
			//SetActiveCamera (false);
			FreezePlayer (false);
		}

		cutScenes[index].onEndMove.Invoke();

	}
}

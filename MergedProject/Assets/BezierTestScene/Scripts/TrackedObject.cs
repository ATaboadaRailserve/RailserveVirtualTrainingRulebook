using UnityEngine;
using System.Collections;
using Rewired;

public class TrackedObject : MonoBehaviour {
	
	public TrackedObject parent;
	public TrackedObject child;
	
	public bool powered;
	public float acceleration;
	public AnimationCurve frictionMultiplier;
	public float deadZone = 0.01f;
	
	//[HideInInspector]
	public float t;
	//[HideInInspector]
	public BezierSpline bezier;
	
	public TrackLayout track;
	public TrackLayout.Line trackPiece;
	public int trackPieceIndex = 0;
	
	[HideInInspector]
	public float velocity;
	
	private Vector3 currentPos;
	private Vector3 incrimentPos;
	private float speedAdjustment;
	
	#region RewiredStuff
	private int playerId = 0;
	private Player player;  // the Rewired player
	
	void Awake () {
		// Get the Player for a particular playerId
		player = ReInput.players.GetPlayer(playerId);
	}
	#endregion
	
	void Start () {
		trackPiece = track.mainLine;
		bezier = trackPiece.beziers[trackPieceIndex];
	}
	
	void Update () {
		if (powered) {
			if (player.GetAxis("AccTrain") > 0) {
				velocity += acceleration * Time.deltaTime;
			} else if (player.GetAxis("AccTrain") < 0) {
				velocity -= acceleration * Time.deltaTime;
			}
		}
		if (velocity > deadZone || velocity < -deadZone)
			velocity -= velocity * frictionMultiplier.Evaluate(velocity) * Time.deltaTime;
		else
			velocity = 0;
		
		/*
		currentPos = bezier.GetPoint(t);
		if (t + 0.02f >= 1)
			incrimentPos = bezier.GetPoint(t - 0.02f);
		else
			incrimentPos = bezier.GetPoint(t + 0.02f);
		speedAdjustment = 1f - (incrimentPos - currentPos).magnitude;
		print (speedAdjustment);
		
		t += velocity * Time.deltaTime * speedAdjustment;
		*/
		
		t += velocity * Time.deltaTime / bezier.GetVelocity(t).magnitude;
		
		if (t > 1f) {
			int switchIndex = -1;
			for (int i = 0; i < trackPiece.piecesTransform.Count; i++) {
				if (trackPiece.piecesTransform[i].name[1] == 'w') {
					switchIndex++;
				}
				if (i >= trackPieceIndex)
					break;
			}
			print (switchIndex);
			if (trackPiece.piecesTransform[trackPieceIndex].name[1] == 'w' && switchIndex >= 0 && trackPiece.diverge[switchIndex] && trackPiece.piecesTransform.Count - 1 > trackPieceIndex) {		// TO DO - Set Up Diverging Bezier
				t -= 1f;
				t = 0f;
				trackPieceIndex = 0;
				trackPiece = trackPiece.childLines[switchIndex];
				bezier = trackPiece.beziers[trackPieceIndex];
			} else if (trackPiece.piecesTransform.Count - 1 > trackPieceIndex) {
				t -= 1f;
				t = 0f;
				trackPieceIndex++;
				bezier = trackPiece.beziers[trackPieceIndex];
			} else
				t = 1f;
		} else if (t < 0f) {
			if (trackPieceIndex > 0) {																					// TO DO - Add CONVERGING onto parent line
				t += 1f;
				t = 1f;
				trackPieceIndex--;
				bezier = trackPiece.beziers[trackPieceIndex];
			} else
				t = 0f;
		}
		
		transform.localPosition = bezier.GetPoint(t);
		transform.LookAt(transform.localPosition + bezier.GetDirection(t));
	}
}

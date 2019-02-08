using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniGen : MonoBehaviour {
	
	[System.Serializable]
	public struct Piece {
		public GameObject prefab;
		[Tooltip("The distance forwards the piece extends")]
		public float length;
		
		[Header("Invokable states")]
		public InteractionHandler.InvokableState onSpawn;
		public InteractionHandler.InvokableState onDespawn;
	}
	
	public struct SpawnedPiece {
		public GameObject gameObject;
		public int pieceIndex;
		public float zPos;
	}
	
	public InteractionHandler interactionHandler;
	
	public Piece[] pieces;
	public AnimationCurve rarityCurve;
	public float landRaiseDistance = 10;
	public float landRaiseTime = 1;
	[Tooltip("The distance forwards to spawn land")]
	public float lead;
	[Tooltip("The distance backwards before despawning land")]
	public float trail;
	public Transform playerBounds;
	public bool overrideHitboxUpdate;
	public GameObject startArea;
	public float startAreaLength;
	public InteractionHandler.InvokableState onStartDespawn;
	
	private List<SpawnedPiece> spawnedPieces;
	private float latestSpawnPoint; // Point to spawn the next piece
	private SpawnedPiece workerPiece;
	public bool starting;
	private Vector3 playerStart;
	private Vector3 landSpawnPoint;
	
	private int lastSpawnedPieceType;
	
	private int winPoints = 5;
	private int numPoints;
	public Text pointsUI;
	
	void Start () {
		landSpawnPoint.z = startAreaLength;
		starting = true;
		playerStart = interactionHandler.playerController.transform.position;
		spawnedPieces = new List<SpawnedPiece>();
		
		// Set the startArea to the first thing in spawnedPieces
		workerPiece = new SpawnedPiece();
		workerPiece.gameObject = startArea;
		workerPiece.pieceIndex = -1;
		spawnedPieces.Add(workerPiece);
		
		// Start the latestSpawnPoint at the player as a default failsafe
		latestSpawnPoint = interactionHandler.playerController.transform.position.z;
	}
	
	void Update () {
		if (starting && (interactionHandler.playerController.transform.position - playerStart).sqrMagnitude > 1f)
			starting = false;
		
		// If the player is approaching the end of the world, spawn another piece
		if (latestSpawnPoint - interactionHandler.playerController.transform.position.z < lead)
			SpawnPiece();
		
		// If the player is far enough from the start of the world, despawn that piece
		if (spawnedPieces.Count > 0 && interactionHandler.playerController.transform.position.z - spawnedPieces[0].zPos - (spawnedPieces[0].pieceIndex >= 0 ? pieces[spawnedPieces[0].pieceIndex].length : startAreaLength) > trail)
			DespawnPiece();
	}
	
	void SpawnPiece () {
		// Create and setup a piece that's a random selection of the pieces array
		workerPiece = new SpawnedPiece();
		workerPiece.pieceIndex = (int)(rarityCurve.Evaluate(Random.value*0.999999f)*pieces.Length);
		
		// check make sure we don't have repeating stuff
		if(workerPiece.pieceIndex == lastSpawnedPieceType)
		{
			workerPiece.pieceIndex++;
			if(workerPiece.pieceIndex >= pieces.Length)
				workerPiece.pieceIndex = 0;
		}
		
		lastSpawnedPieceType = workerPiece.pieceIndex;
		
		workerPiece.gameObject = (GameObject)Instantiate(pieces[workerPiece.pieceIndex].prefab, landSpawnPoint, Quaternion.identity);
		workerPiece.zPos = landSpawnPoint.z;
		
		// Add the new piece's length to the landSpawnPoint.z to offset the next piece appropriately
		landSpawnPoint.z += pieces[workerPiece.pieceIndex].length;
		latestSpawnPoint = landSpawnPoint.z;
		
		// Add the new piece to the spawnedPieces list
		pieces[workerPiece.pieceIndex].onSpawn.Invoke();
		spawnedPieces.Add(workerPiece);
		
		// Update the player bounds if not overriddenn not to
		if (!overrideHitboxUpdate && !starting) {
			UpdateHitboxLocation(new Vector3(0,0,pieces[spawnedPieces[spawnedPieces.Count-1].pieceIndex].length));
		}
		
		StartCoroutine(DoTheSlide(workerPiece.gameObject.transform, true));
	}
	
	void DespawnPiece () {
		if (spawnedPieces.Count <= 0) // Ensure there's at least one piece to despawn
			return;
		
		// Ensure the pieceIndex is valid and Invoke onDespawn for the piece tied to the despawning land
		if (spawnedPieces[0].pieceIndex >= 0 && spawnedPieces[0].pieceIndex < spawnedPieces.Count) {
			pieces[spawnedPieces[0].pieceIndex].onDespawn.Invoke();
		} else {
			onStartDespawn.Invoke(); // Else, we have the startArea and should do a specific Invoke
		}
		StartCoroutine(DoTheSlide(spawnedPieces[0].gameObject.transform));
		spawnedPieces.RemoveAt(0); // Clean the list of the dirty left-overs
	}
	
	IEnumerator DoTheSlide (Transform land, bool up = false) {
		float timer = 0;
		Vector3 landStart = land.position; // Get the land's default point (Where it spawned)
		
		// Lerp between the landStart and landStart - landRaiseDistance(on the Y axis) over the period of landRaiseTime seconds
		if (up) {
			while (timer < 1 - Time.deltaTime / landRaiseTime) {
				timer += Time.deltaTime / landRaiseTime;
				land.position = Vector3.Lerp(landStart - new Vector3(0, landRaiseDistance, 0), landStart, timer);
				yield return null;
			}
			land.position = landStart;
		} else {
			while (timer < 1 - Time.deltaTime / landRaiseTime) {
				timer += Time.deltaTime / landRaiseTime;
				land.position = Vector3.Lerp(landStart, landStart - new Vector3(0, landRaiseDistance, 0), timer);
				yield return null;
			}
			Destroy(land.gameObject); // If going down, DESTROY THE THING one done
		}
	}
	
	// Move the playerBounds hitbox by a given offset
	public void UpdateHitboxLocation (Vector3 offset) {
		if (!playerBounds)
			return;
		playerBounds.position += offset;
	}
	
	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(landSpawnPoint, new Vector3(1,1,1)); // Small cube to help you find where the next piece spawns
		
		if (startArea) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(startArea.transform.position + new Vector3(0, 0, startAreaLength), new Vector3(1,1,1)); // Small cube to help you find where the first piece spawns
		}
	}
	
	public void ScorePoint()
	{
		numPoints++;
		pointsUI.text = numPoints.ToString();
		if(numPoints >= winPoints)
			interactionHandler.Win();
	}
}
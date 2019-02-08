using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Cliffer : MonoBehaviour {
	
	public int width = 3;
	public int depth = 3;
	public float squareSize = 10;
	
	public GameObject[] centerPieces;
	public GameObject[] sidePieces;
	public GameObject[] cornerPieces;
	
	private Transform[] colliders;
	
	// Randomly generate an island based on the defined pieces, size, and dimensions
	public void Generate () {
		
		// Script giving a piece of its mind to give you peace of mind.  (Let's you know it's working.)
		print("Generating Island");
		
		#region Kill The Old Island
		List<Transform> children = new List<Transform>();
		// Get all children pieces
		foreach (Transform t in transform) {
			children.Add(t);
		}
		for (int i = children.Count-1; i >= 0; i--) {
			// Murder them immediately
			DestroyImmediate(children[i].gameObject);
			children.RemoveAt(i);
		}
		#endregion
		
		// Village GameObject
		GameObject temp;
		
		#region Make The New Island
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < depth; y++) {
				#region Corners
				// Decide which corner, grab a random Corner piece from the array of possibilities, rotate it, then child it under the Island Parent Transform (Assuming this script is attached to it)
				if (x == 0 && y == 0) {
					temp = (GameObject)Instantiate(cornerPieces[(int)Random.Range(0,cornerPieces.Length-0.00001f)], new Vector3(x, 0, y)*squareSize, Quaternion.identity);
					temp.transform.localEulerAngles = new Vector3(0,-90,0);
					temp.transform.parent = transform;
				} else if (x == 0 && y == depth - 1) {
					temp = (GameObject)Instantiate(cornerPieces[(int)Random.Range(0,cornerPieces.Length-0.00001f)], new Vector3(x, 0, y)*squareSize, Quaternion.identity);
					temp.transform.localEulerAngles = new Vector3(0,0,0);
					temp.transform.parent = transform;
				} else if (x == width - 1 && y == 0) {
					temp = (GameObject)Instantiate(cornerPieces[(int)Random.Range(0,cornerPieces.Length-0.00001f)], new Vector3(x, 0, y)*squareSize, Quaternion.identity);
					temp.transform.localEulerAngles = new Vector3(0,180,0);
					temp.transform.parent = transform;
				} else if (x == width - 1 && y == depth - 1) {
					temp = (GameObject)Instantiate(cornerPieces[(int)Random.Range(0,cornerPieces.Length-0.00001f)], new Vector3(x, 0, y)*squareSize, Quaternion.identity);
					temp.transform.localEulerAngles = new Vector3(0,90,0);
					temp.transform.parent = transform;
				}
				#endregion
				
				#region Sides
				else if (x == 0 || y == 0 || x == width - 1 || y == depth - 1) {
					// Grab a random Side piece from the array of possibilities
					temp = (GameObject)Instantiate(sidePieces[(int)Random.Range(0,cornerPieces.Length-0.00001f)], new Vector3(x, 0, y)*squareSize, Quaternion.identity);
					
					// Decide which side it's on and rotate appropriately
					if (x == 0)
						temp.transform.localEulerAngles = new Vector3(0,-90,0);
					else if (x == width - 1)
						temp.transform.localEulerAngles = new Vector3(0,90,0);
					else if (y == 0)
						temp.transform.localEulerAngles = new Vector3(0,180,0);
					
					// Tell it to be a child of the Island parent
					temp.transform.parent = transform;
				}
				#endregion
				
				#region Center pieces
				else {
					// Center pieces are easy, grab a random one from the array of possibilities then randomly rotate it and child it.
					temp = (GameObject)Instantiate(centerPieces[(int)Random.Range(0,cornerPieces.Length-0.00001f)], new Vector3(x, 0, y)*squareSize, Quaternion.identity);
					temp.transform.localEulerAngles = new Vector3(0,(int)Random.Range(0,3.999999f)*90,0);
					temp.transform.parent = transform;
				}
				#endregion
			}
		}
		#endregion
		
		#region Add Hitboxes
		// Village BoxCollider
		BoxCollider boxxi;
		
		// Colliders
		for (int i = 0; i < 5; i++) {
			temp = new GameObject();
			temp.transform.parent = transform;
			boxxi = temp.AddComponent<BoxCollider>();
			switch (i) {
				case 0:
					temp.name = "Floor Collider";
					temp.transform.localPosition = new Vector3((width-1)*squareSize/2f, -0.5f, (depth-1)*squareSize/2f);
					boxxi.size = new Vector3(width*squareSize, 1, depth*squareSize);
					break;
				case 1:
					temp.name = "Side 1 Collider";
					temp.transform.localPosition = new Vector3(-squareSize/2f - 0.5f, 10, (depth-1)*squareSize/2f);
					boxxi.size = new Vector3(1, 20, depth*squareSize);
					break;
				case 2:
					temp.name = "Side 2 Collider";
					temp.transform.localPosition = new Vector3((width-1)*squareSize/2f, 10, -squareSize/2f - 0.5f);
					boxxi.size = new Vector3(width*squareSize, 20, 1);
					break;
				case 3:
					temp.name = "Side 3 Collider";
					temp.transform.localPosition = new Vector3((width-1)*squareSize + squareSize/2f + 0.5f, 10, (depth-1)*squareSize/2f);
					boxxi.size = new Vector3(1, 20, depth*squareSize);
					break;
				case 4:
					temp.name = "Side 4 Collider";
					temp.transform.localPosition = new Vector3((width-1)*squareSize/2f, 10, (depth-1)*squareSize + squareSize/2f + 0.5f);
					boxxi.size = new Vector3(width*squareSize, 20, 1);
					break;
			}
		}
		#endregion
		
	}
}
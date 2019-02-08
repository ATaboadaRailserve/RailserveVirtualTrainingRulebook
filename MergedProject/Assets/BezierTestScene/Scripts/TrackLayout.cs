using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TrackLayout : MonoBehaviour {
	
	// Prefab setup for each track segment type (straight_10meter, curve_45_degree, etc.)
	[System.Serializable]
	public struct Segment {
		public string pieceName;
		public GameObject gameObject;
	}
	
	// Container for each branch of the binary tree for the yard.
	[System.Serializable]
	public class Line {
		public string name;
		public Transform lineObject;
		public List<int> pieces;
		public List<int> count;
		public List<int> prevCount;
		public List<Transform> piecesTransform;
		public Line parentLine;
		public List<Line> childLines;
		public List<bool> diverge;
		
		public bool updateSegments = false;
		public bool activated = false;
		public bool softActivate = false;
		public TrackLayout masterTrack;
		
		public List<BezierSpline> beziers;
		public List<BezierSpline> divergeBeziers;
		
		// Automated Initialization
		public void Activate () {
			// If anything,  per chance, is left over from older stuff, kill it.
			CleanUp();
			
			name = "0";
			masterTrack.dirty = true;
			if (!lineObject) {
				lineObject = new GameObject().transform;
				lineObject.name = "TrackLine";
				if (parentLine != null) {
					lineObject.parent = parentLine.lineObject;
					for (int i = 0; i < parentLine.childLines.Count; i++) {
						if (parentLine.childLines[i] == this)
							name = parentLine.name + "-" + i;
					}
				} else {
					lineObject.parent = masterTrack.transform;
				}
				lineObject.localPosition = Vector3.zero;
			}
			if (!softActivate) {
				pieces = new List<int>();
				count = new List<int>();
				prevCount = new List<int>();
				piecesTransform = new List<Transform>();
				beziers = new List<BezierSpline>();
				childLines = new List<Line>();
				diverge = new List<bool>();
				divergeBeziers = new List<BezierSpline>();
			}
			activated = true;
		}
		
		public void UpdateSegments () {
			
			CleanUp();
			
			int switchIndex = 0;
			
			// Make New Pieces
			for(int i = 0; i < pieces.Count; i++) {
				if (i < pieces.Count) {
					GameObject temp = null;
					if (pieces[i] < 0) {
						temp = (GameObject)Instantiate(masterTrack.inverseSegments[(int)Mathf.Abs(pieces[i])].gameObject, Vector3.zero, Quaternion.identity);
					} else {
						temp = (GameObject)Instantiate(masterTrack.segments[(int)Mathf.Abs(pieces[i])].gameObject, Vector3.zero, Quaternion.identity);
					}
					temp.tag = "TrackPiece";
					temp.name = masterTrack.segments[(int)Mathf.Abs(pieces[i])].pieceName;
					foreach (Transform t in temp.transform) {
						if (t.name == "SplitConnection") {
							if (switchIndex > childLines.Count-1) {
								childLines.Add(new Line());
								childLines[childLines.Count-1].Check(masterTrack, this);
								childLines[childLines.Count-1].lineObject.parent = t;
								childLines[childLines.Count-1].lineObject.localPosition = Vector3.zero;
								childLines[childLines.Count-1].lineObject.localEulerAngles = Vector3.zero;
								diverge.Add(false);
								divergeBeziers.Add(t.GetComponent<BezierSpline>());
							} else {
								childLines[switchIndex].activated = false;
								childLines[switchIndex].softActivate = true;
								childLines[switchIndex].Check(masterTrack, this);
								childLines[switchIndex].softActivate = false;
								childLines[switchIndex].lineObject.parent = t;
								childLines[switchIndex].lineObject.localPosition = Vector3.zero;
								childLines[switchIndex].lineObject.localEulerAngles = Vector3.zero;
								childLines[switchIndex].UpdateSegments();
							}
							masterTrack.dirty = true;
							switchIndex++;
							break;
						} else if (t.name == "Bezier") {
							beziers.Add(t.GetComponent<BezierSpline>());
						}
					}
					piecesTransform.Add(temp.transform);
					if (i == 0) {
						temp.transform.parent = lineObject;
					} else {
						Transform tempParent = piecesTransform[i-1];
						foreach (Transform t in piecesTransform[i-1]) {
							if (t.name == "Connection") {
								tempParent = t;
								break;
							}
						}
						temp.transform.parent = tempParent;
					}
					temp.transform.localPosition = Vector3.zero;
					temp.transform.localEulerAngles = Vector3.zero;
					
					if (i < count.Count && pieces[i] == 0) {
						ChangeCount(i, count[i]);
					}
				}
			}
			updateSegments = false;
		}
		
		public void Check (TrackLayout mainScript, Line parentLineIn = null) {
			if (!activated) {
				masterTrack = mainScript;
				if (parentLineIn != null)
					parentLine = parentLineIn;
				Activate();
			}
			if (updateSegments) {
				UpdateSegments();
			}
			for (int i = 0; i < childLines.Count; i++) {
				childLines[i].Check(mainScript, this);
			}
		}
		
		public void CleanUp () {
			// Kill Old Pieces
			if (piecesTransform != null) {
				for (int i = piecesTransform.Count-1; i >= 0; i--) {
					if (piecesTransform[i])
						DestroyImmediate(piecesTransform[i].gameObject);
				}
				piecesTransform.Clear();
				beziers.Clear();
				diverge.Clear();
				divergeBeziers.Clear();
			}
		}
		
		public List<Line> GetChildren (List<Line> lines) {
			lines.Add(this);
			foreach (Line l in childLines) {
				l.GetChildren(lines);
			}
			return lines;
		}
		
		public void ChangeCount (int index, int amount) {
			if (index < piecesTransform.Count && piecesTransform[index] != null) {
				for (int i = piecesTransform[index].childCount - 1; i >= 0; i--) {
					if (piecesTransform[index].GetChild(i).name == "Track") {
						DestroyImmediate(piecesTransform[index].GetChild(i).gameObject);
					} else if (piecesTransform[index].GetChild(i).name == "Connection") {
						piecesTransform[index].GetChild(i).localPosition = new Vector3(-amount*0.5f,0,0);
					} else if (piecesTransform[index].GetChild(i).name == "Bezier") {
						piecesTransform[index].GetChild(i).gameObject.GetComponent<BezierSpline>().points[piecesTransform[index].GetChild(i).gameObject.GetComponent<BezierSpline>().points.Length-1] = new Vector3(amount*0.5f - 5f,0,0);
						piecesTransform[index].GetChild(i).gameObject.GetComponent<BezierSpline>().points[piecesTransform[index].GetChild(i).gameObject.GetComponent<BezierSpline>().points.Length-2] = new Vector3(amount*0.5f-0.1f - 5f,0,0);
					}
				}
				GameObject temp;
				for (int i = 0; i < amount; i++) {
					temp = (GameObject)Instantiate(masterTrack.halfMeterTrack, Vector3.zero, Quaternion.identity);
					temp.transform.parent = piecesTransform[index];
					temp.transform.rotation = Quaternion.identity;
					temp.transform.localPosition = new Vector3(-i*0.5f,0,0);
					temp.name = "Track";
				}
			}
		}
	}
	
	public Segment[] segments;
	public Segment[] inverseSegments;
	public Line mainLine;
	public GameObject halfMeterTrack;
	
	[HideInInspector]
	public List<Line> lines;
	[HideInInspector]
	public bool dirty = true;
	[HideInInspector]
	public int activeLine;
	
	void Update () {
		mainLine.Check(this);
		if (dirty) {
			lines = new List<Line>();
			mainLine.GetChildren(lines);
			dirty = false;
		}
	}
	
	public void Generate () {
		mainLine.UpdateSegments();
	}
	
	public void Next (bool increase) {
		if (increase)
			activeLine++;
		else
			activeLine--;
		if (activeLine > lines.Count-1)
			activeLine = lines.Count-1;
		else if (activeLine < 0)
			activeLine = 0;
	}
	
	public void KillChild (Line line, int killAt) {
		int child = 0;
		for (int i = 0; i < killAt; i--) {
			if (Mathf.Abs(line.pieces[i]) == 8) {
				child++;
			}
		}
		line.childLines[child].CleanUp();
		line.childLines.RemoveAt(child);
	}
	
	public void Reset () {
		for (int i = transform.childCount - 1; i >= 0; i--) {
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
		mainLine.activated = false;
		activeLine = 0;
		Generate();
	}
}

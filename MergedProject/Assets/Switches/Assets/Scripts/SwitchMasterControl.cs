using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SwitchMasterControl : MonoBehaviour
{
	//------------------------------Public-----------------------------
	public int SwitchNumber;
	public enum BranchSide{BranchLeft, BranchRight};
	public BranchSide branchSide;
	public enum StartSide{StartOnRightSide, StartOnLeftSide};
	public StartSide startSide;
	public List<StuckObject> stuckObjectList = new List<StuckObject> ();
	public int stuckObjectRandomFactor;
	public bool HideObjectArrows;
	public bool ScoreSwitch;
	//------------------------------Public Hidden----------------------
	[HideInInspector]
	public bool onFarSide;
	[HideInInspector]
	public bool locked;
	[HideInInspector]
	public bool lined;
	[HideInInspector]
	public BezierSetup bezier;
	//------------------------------Private----------------------------
	GrabandRotate garScript;
	RockScript nrs,frs;
	bool canbelocked = true;


	// Use this for initialization
	void Start () 
	{
		nrs = this.transform.FindChild ("ITEM_N").GetComponent<RockScript> ();
		frs = this.transform.FindChild ("ITEM_F").GetComponent<RockScript> ();
		if(HideObjectArrows) {nrs.hideArrow = true; frs.hideArrow = true;}
		garScript = this.transform.FindChild("Lever_1").FindChild("Lever_0").GetComponent<GrabandRotate>();
		if (branchSide == BranchSide.BranchRight) {garScript.RightSwitch = true;}
		else {garScript.RightSwitch = false;}
		if (startSide == StartSide.StartOnLeftSide) {garScript.start_Left_Side = true;}
		else {garScript.start_Left_Side = false;}
		garScript.objectlist = stuckObjectList;
		garScript.random_factor = stuckObjectRandomFactor;
		garScript.switchScoreNumber = SwitchNumber;
		garScript.switchScoring = ScoreSwitch;
		bezier = gameObject.GetComponent<BezierSetup> ();
		if (!garScript.lined && bezier != null) {
			bezier.SwitchOpen ();
		}
	}
	void Update()
	{
		onFarSide = garScript.onFarSide;
		if (bezier != null) {
			bezier.diverging = onFarSide;

			if (locked != garScript.locked || lined != garScript.lined) {
				if (!garScript.locked || !garScript.lined) {
					bezier.SwitchOpen ();
				} else {
					bezier.SwitchLocked (garScript.locked);
				}
			}


			/*if (lined != garScript.lined) {
				if (!garScript.lined) {
					bezier.SwitchOpen ();
				} else {
					bezier.SwitchLocked (garScript.locked);
				}
			}*/
		}
		locked = garScript.locked;
		lined = garScript.lined;
	}
}


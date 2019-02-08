using UnityEngine;
using System.Collections;

public class GlobalVars : MonoBehaviour {

	public enum RedZoneType { Wire, Box, Rail, None };
	
	public RedZoneType redZoneType;
	
	public static float Mod(float a,float b) {
		return a - b * Mathf.Floor(a / b);
	}
}

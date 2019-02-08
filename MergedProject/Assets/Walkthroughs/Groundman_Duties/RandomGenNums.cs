using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenNums : MonoBehaviour {


	public GameObject Version1, Version2, Version3;
	public int Num;
		// I'm sorry I know this script is shit
	//I'm not really that sorry ;/
		void Start()
		{
		Version1.gameObject.SetActive (false);
		Version2.gameObject.SetActive (false);
		Version3.gameObject.SetActive (false);

			Num = (Random.Range(0, 3));

		if (Num == 0) {
			Version1.gameObject.SetActive (true);}

		if (Num == 1)
		{Version2.gameObject.SetActive (true);}

		if (Num == 2)
		{Version3.gameObject.SetActive (true);}
		}
	}


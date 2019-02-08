using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {
	
	Transform wheelSetOne;
	Transform wheelSetTwo;
	
	void Update () {
		transform.position = (wheelSetOne.position + wheelSetTwo.position)/2f;
		transform.localEulerAngles = wheelSetTwo.position - wheelSetOne.position;
	}
}

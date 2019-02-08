using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class CameraFlyIn : MonoBehaviour {
    Vector3 startLocal;
    Vector3 targetPosition;
    public float distance;
	// Use this for initialization
	void Start () {
        targetPosition = transform.position;
        startLocal = transform.localPosition;
        transform.localPosition = new Vector3(0, 0, -distance);
        StartCoroutine("FlyIn");
	}
	
    IEnumerator FlyIn()
    {
        Debug.Log("Start");
        transform.parent.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        while(Vector3.Distance(targetPosition, transform.position) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPosition;
        transform.parent.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        transform.localPosition = startLocal;
        transform.GetComponent<HeadBob>().enabled = true;
        Debug.Log("End");
    }
}

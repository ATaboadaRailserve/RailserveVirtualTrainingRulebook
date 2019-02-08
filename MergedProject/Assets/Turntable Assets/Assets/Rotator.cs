using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
    public float displacement;

    [HideInInspector]
    public Vector3 startPos;
    Quaternion startRot;
    public float speed;
    public string description;

    bool exploded;
    [HideInInspector]
    public Vector3 targetPos;

    //LineRenderer line;
    //Vector3 lineStart;
    //Vector3 lineEnd;

    Transform target;
    
	// Use this for initialization
	void Start () {

        target = GameObject.FindGameObjectWithTag("Target").transform;

        transform.tag = "Rotator";

        startPos = transform.position;
        startRot = transform.rotation;
        speed = 5;
        

        exploded = false;
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void Explode()
    {
        target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, displacement);
        targetPos = target.position;
        exploded = true;
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine("ExplodeToTarget");

    }

    public void Return()
    {
        exploded = false;
        StartCoroutine("ReturnToStart");
    }

    IEnumerator ExplodeToTarget()
    {
        StopCoroutine("ReturnToStart");
        while (Vector3.Distance(targetPos, transform.position) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPos;
    }

    IEnumerator ReturnToStart()
    {
        StopCoroutine("ExplodeToTarget");
        while (Vector3.Distance(startPos, transform.position) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRot, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
        while(Quaternion.Angle(startRot, transform.rotation) > 1f || Quaternion.Angle(startRot, transform.rotation) < -1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, startRot, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        GameObject.FindGameObjectWithTag("RotatorCamera").GetComponent<RotatorCamera>().FinishReturning();
        GetComponent<BoxCollider>().enabled = true;
        transform.rotation = startRot;
    }
}

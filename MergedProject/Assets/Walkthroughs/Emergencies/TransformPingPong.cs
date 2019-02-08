using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPingPong : MonoBehaviour {

    public Transform start;
    public Transform end;
    public float loopTime = 1.5f;
    float halfTime;
    float t;

    void Start()
    {
        halfTime = loopTime / 2;
    }
	
	void Update () {
        t += Time.deltaTime;
        t = t % (loopTime);
        if(t < halfTime)
        {
            transform.position = Vector3.Lerp(start.position, end.position, t / halfTime);
            transform.rotation = Quaternion.Lerp(start.rotation, end.rotation, t / halfTime);
            transform.localScale = Vector3.Lerp(start.localScale, end.localScale, t / halfTime);
        }
        else
        {
            transform.position = Vector3.Lerp(end.position, start.position, (t - halfTime) / halfTime);
            transform.rotation = Quaternion.Lerp(end.rotation, start.rotation, (t - halfTime) / halfTime);
            transform.localScale = Vector3.Lerp(end.localScale, start.localScale, (t - halfTime) / halfTime);
        }
        
	}
}

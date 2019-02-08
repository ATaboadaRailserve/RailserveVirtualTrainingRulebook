using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBob : MonoBehaviour {

    public Vector3 start = Vector3.one;
    public Vector3 end = Vector3.one;
    public float loopTime = 1.5f;
    float halfTime;
    float t;

    void Start()
    {
        halfTime = loopTime / 2;
    }

    void Update()
    {
        t += Time.deltaTime;
        t = t % (loopTime);
        if (t < halfTime)
        {
            transform.localScale = Vector3.Lerp(start, end, t / halfTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(end, start, (t - halfTime) / halfTime);
        }

    }
}

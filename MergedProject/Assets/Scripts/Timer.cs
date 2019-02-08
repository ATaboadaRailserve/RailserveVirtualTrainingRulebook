using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    public float seconds;
    public InteractionHandler.InvokableState OnAlarm;

    private float t = 0;

	public void StartTimer()
    {
        t = seconds;
    }

    public void StartTimer(float _seconds)
    {
        t = _seconds;
    }

    public void CancelTimer()
    {
        t = 0;
    }

    void Update()
    {
        if (t > 0)
            t -= Time.deltaTime;
        else if (t < 0)
        {
            t = 0;
            OnAlarm.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCrossings : MonoBehaviour {

    public Transform crossingBeam1;
    public Transform crossingBeam2;

    public Vector3 beamDown = new Vector3(-90, -90, 90);
    public Vector3 beamUp = new Vector3(-12, -90, 90);

    public float actionTime = 4.0f;
    public bool startRaised;

    public InteractionHandler.InvokableState OnRaisedStart;
    public InteractionHandler.InvokableState OnRaisedEnd;
    public InteractionHandler.InvokableState OnLoweredStart;
    public InteractionHandler.InvokableState OnLoweredEnd;

    public State CrossingState { get; private set; }
    private Coroutine co;
    private float t = 0.0f;

    public enum State
    {
        CrossingDown,
        CrossingUp,
        Animating
    }

    void Start()
    {
        if (startRaised)
        {
            SetCrossings(true);
            t = actionTime;
        }
        else
        {
            SetCrossings(false);
        }
    }

    void SetCrossings(bool up)
    {
        if (up)
        {
            SetCrossings(beamUp);
            CrossingState = State.CrossingUp;
        }
        else
        {
            SetCrossings(beamDown);
            CrossingState = State.CrossingDown;
        }
    }

    void SetCrossings(Vector3 rotation)
    {
        crossingBeam1.localRotation = Quaternion.Euler(rotation);
        crossingBeam2.localRotation = Quaternion.Euler(rotation);
    }

    public void RaiseCrossing()
    {
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(BeamLerpUp());
    }

    public void LowerCrossing()
    {
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(BeamLerpDown());
    }

    IEnumerator BeamLerpUp()
    {
        CrossingState = State.Animating;
        for(; t < actionTime; t += Time.deltaTime)
        {
            SetCrossings(Vector3.Lerp(beamDown, beamUp, t / actionTime));
            yield return null;
        }
        SetCrossings(true);
    }

    IEnumerator BeamLerpDown()
    {
        CrossingState = State.Animating;
        for(; t > 0.0f; t -= Time.deltaTime)
        {
            SetCrossings(Vector3.Lerp(beamDown, beamUp, t));
            yield return null;
        }
        SetCrossings(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UberLocomotive : MonoBehaviour {

    // Locomotive only, not mount points for cars
    public UberMountPoint[] mountPoints;

    // Cars that are currently attached, and will move with the locomotive
    public List<UberCar> coupledCars;

    // Events
    public UnityEvent OnIdle;
    public UnityEvent OnReady;
    public UnityEvent OnMove;

    
    // how long does the car stay ready before entering idle
    const float readyMaxTime = 120.0f;
    float idleTimer = 0;

    State locomotiveState = State.idle;

    public enum State
    {
        idle,
        ready,
        moving
    }

    void Update()
    {
        if(locomotiveState == State.ready)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer < 0)
                SetIdle();
        }
    }

    public void SetIdle()
    {
        locomotiveState = State.idle;
        OnIdle.Invoke();
    }

    public void SetReady()
    {
        locomotiveState = State.ready;
        idleTimer = readyMaxTime;
        OnReady.Invoke();
    }

    public void SetMoving()
    {
        locomotiveState = State.moving;
        OnMove.Invoke();
    }

    private void SetMountable(bool isMountable)
    {
        foreach (UberMountPoint p in mountPoints)
            p.gameObject.SetActive(isMountable);
        
        if(isMountable)
        {
            foreach (UberCar c in coupledCars)
                c.EnableMountPoints();
        }
        else
        {
            foreach (UberCar c in coupledCars)
                c.DisableMountPoints();
        }
            
    }
}

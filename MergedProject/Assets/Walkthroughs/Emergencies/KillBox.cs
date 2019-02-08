using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class KillBox : MonoBehaviour {

    public bool ActiveOnStart = true;
    public bool Inverse = true;
    public float WarningTime = 3.0f;
    public InteractionHandler.InvokableState OnSafe;
    public InteractionHandler.InvokableState OnWarn;
    public InteractionHandler.InvokableState OnKill;
    public float CurrentWarningTime { get; private set; }
    public STATE CurrentState { get; private set; }

    public enum STATE
    {
        INACTIVE,
        ACTIVE,
        WARNING
    }

    void Start()
    {
        gameObject.SetActive(ActiveOnStart);
    }

    void Update()
    {
        if(Inverse && CurrentState == STATE.WARNING)
        {
            CurrentWarningTime -= Time.deltaTime;
            if (CurrentWarningTime < 0)
                OnKill.Invoke();
        }
    }

    void OnTriggerEnter()
    {
        if(Inverse)
        {
            CurrentState = STATE.ACTIVE;
            OnSafe.Invoke();
        }
        else
        {
            if(WarningTime > 0)
            {
                CurrentState = STATE.WARNING;
                CurrentWarningTime = WarningTime;
                OnWarn.Invoke();
            }
            else
            {
                OnKill.Invoke();
            }
        }
    }

    void OnTriggerExit()
    {
        if(Inverse)
        {
            if (WarningTime > 0)
            {
                CurrentState = STATE.WARNING;
                CurrentWarningTime = WarningTime;
                OnWarn.Invoke();
            }
            else
            {
                OnKill.Invoke();
            }
        }
        else
        {
            CurrentState = STATE.ACTIVE;
            OnSafe.Invoke();
        }
    }

    void OnTriggerStay()
    {
        if(!Inverse)
        {
            CurrentWarningTime -= Time.deltaTime;
            if (CurrentWarningTime < 0)
                OnKill.Invoke();
        }
    }
}

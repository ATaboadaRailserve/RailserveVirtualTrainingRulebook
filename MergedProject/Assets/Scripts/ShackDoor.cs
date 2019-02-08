using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShackDoor : MonoBehaviour {

    [Header("Basic")]
    public State startState = State.closed;
    public bool openOnUnlock = false;
    public InteractionHandler.InvokableState OnOpenInwards;
    public InteractionHandler.InvokableState OnOpenOutwards;
    public InteractionHandler.InvokableState OnClose;
    public InteractionHandler.InvokableState OnLock;
    public InteractionHandler.InvokableState OnUnlock;

    [Header("Coordinated with Proximity Events")]
    public InteractionHandler.InvokableState OnShowUsable;
    public InteractionHandler.InvokableState OnHideUsable;


    [Header("Use Reset(State newStartState) to reset door")]
    [Header("Index 0 consumed after each \"First Open\"")]
    public List<FirstOpenEventPreset> firstOpenEventPresets;

    private Transform playerTrans;
    private bool openedBefore;

    public enum State
    {
        closed,
        locked,
        opened_inwards,
        opened_outwards
    }

    Vector3 localNormal;
    State currentState;

    [System.Serializable]
    public class FirstOpenEventPreset
    {
        public InteractionHandler.InvokableState OnOpen;
    }

    void Awake()
    {
        localNormal = transform.up;
    }

    void Start()
    {

        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTrans == null)
            Debug.LogError("Shack Door cannot find an instance of the player");
        currentState = startState;
        if (currentState == State.locked)
        {
            currentState = State.opened_inwards;
            Lock();
        }
        else if (currentState == State.opened_inwards)
            Open(true);
        else if (currentState == State.opened_outwards)
            Open(false);
    }

    public void Reset(State newStartState)
    {
        openedBefore = false;
        if (firstOpenEventPresets.Count > 0)
            firstOpenEventPresets.RemoveAt(0);
        currentState = newStartState;
        if (currentState == State.locked)
        {
            currentState = State.opened_inwards;
            Lock();
        }
        else if (currentState == State.opened_inwards)
            Open(true);
        else if (currentState == State.opened_outwards)
            Open(false);
    }

	public void ToggleDoor()
    {
        if (currentState == State.locked)
            return;

        if (currentState == State.opened_inwards || currentState == State.opened_outwards)
            Close();
        else
            Open();
    }

    public void Open(bool inwards)
    {
        if (currentState == State.locked)
            return;

        if (inwards)
        {
            currentState = State.opened_inwards;
            OnOpenInwards.Invoke();
        }
        else
        {
            currentState = State.opened_outwards;
            OnOpenOutwards.Invoke();
        }

        if (!openedBefore && firstOpenEventPresets.Count > 0)
        {
            openedBefore = true;
            firstOpenEventPresets[0].OnOpen.Invoke();
        }
    }

    public void Open()
    {
        if (currentState == State.locked)
            return;

        Vector3 direction = playerTrans.position - transform.position;
        float angle = Vector3.Angle(localNormal, direction);

        //print(angle);
        if (angle < 90)
        {
            currentState = State.opened_outwards;
            OnOpenOutwards.Invoke();
        }
        else
        {
            currentState = State.opened_inwards;
            OnOpenInwards.Invoke();
        }

        if (!openedBefore && firstOpenEventPresets.Count > 0)
        {
            openedBefore = true;
            firstOpenEventPresets[0].OnOpen.Invoke();
        }
    }

    public void Close()
    {
        if (currentState == State.closed || currentState == State.locked)
            return;

        currentState = State.closed;
        OnClose.Invoke();
    }

    public void Lock()
    {
        if (currentState == State.locked)
            return;

        Close();
        currentState = State.locked;
        OnLock.Invoke();
    }

    public void Unlock()
    {
        if (currentState != State.locked)
            return;
        
        currentState = State.closed;
        OnUnlock.Invoke();
        if (openOnUnlock)
            Open();
    }

    public void ShowUsable()
    {
        if (currentState != State.locked)
            OnShowUsable.Invoke();
    }

    public void HideUsable()
    {
        OnHideUsable.Invoke();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityInteractable : Interactable {

    public bool mustBeInitialObject;
    public string activationAxis;

    [Header("Proximity Interactions")]
    public InteractionHandler.InvokableState OnMixedEnter;
    public InteractionHandler.InvokableState OnMixedStay;
    public InteractionHandler.InvokableState OnMixedExit;

    [Header("Axis Interactions")]
    public InteractionHandler.InvokableState onButtonDown;
    public InteractionHandler.InvokableState onButtonEnter;
    public InteractionHandler.InvokableState onButton;
    public InteractionHandler.InvokableState onButtonExit;
    public InteractionHandler.InvokableState onButtonUp;

    protected bool isProximal;
    protected bool isLooking;
    protected bool isMixed;

    public bool IsInteractiable
    {
        get { return isInteractiable && isProximal; }
        set { isInteractiable = value; }
    }

    #region Cursor Overrides

    public override void IHCursorEnterEvents()
    {
        isLooking = true;
        base.IHCursorEnterEvents();
        if (IsInteractiable)
            OnMixedEnter.Invoke();
    }
    public override void IHCursorStayEvents()
    {
        isLooking = true;
        base.IHCursorStayEvents();
        if (IsInteractiable)
            OnMixedStay.Invoke();
    }
    public override void IHCursorExitEvents()
    {
        isLooking = false;
        base.IHCursorExitEvents();
        if (IsInteractiable)
            OnMixedExit.Invoke();
    }

    #endregion
    
    #region Button Overrides

    public override void IHButtonDown(InteractionHandler.InteractionParameters iParam)
    {
        if (IsInteractiable && iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject))
        {
            if (!remainLocked)
                interactionHandler.LockInteraction = false;
            onButtonDown.Invoke();
        }
    }

    public override void IHButtonEnter(InteractionHandler.InteractionParameters iParam)
    {
        if (IsInteractiable && iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject))
        {
            if (!remainLocked)
                interactionHandler.LockInteraction = false;
            onButtonEnter.Invoke();
        }
    }

    public override void IHButton(InteractionHandler.InteractionParameters iParam)
    {
        if (IsInteractiable && iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject))
        {
            if (!remainLocked)
                interactionHandler.LockInteraction = false;
            onButton.Invoke();
        }
    }

    public override void IHButtonExit(InteractionHandler.InteractionParameters iParam)
    {
        if (IsInteractiable && iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject))
        {
            if (!remainLocked)
                interactionHandler.LockInteraction = false;
            onButtonExit.Invoke();
        }
    }

    public override void IHButtonUp(InteractionHandler.InteractionParameters iParam)
    {
        if (IsInteractiable && iParam.axis == activationAxis && (!mustBeInitialObject || iParam.isFirst == gameObject))
        {
            if (!remainLocked)
                interactionHandler.LockInteraction = false;
            onButtonUp.Invoke();
        }
    }

    #endregion

    /* Old Button Overrides
    public override void IHButtonDown(InteractionHandler.InteractionParameters iParam) { if (iParam.axis == activationAxis && IsInteractiable) onButtonDown.Invoke(); }
    public override void IHButtonEnter(InteractionHandler.InteractionParameters iParam) { if (iParam.axis == activationAxis && IsInteractiable) onButtonEnter.Invoke(); }
    public override void IHButton(InteractionHandler.InteractionParameters iParam) { if (iParam.axis == activationAxis && IsInteractiable) onButton.Invoke(); }
    public override void IHButtonExit(InteractionHandler.InteractionParameters iParam) { if (iParam.axis == activationAxis && IsInteractiable) onButtonExit.Invoke(); }
    public override void IHButtonUp(InteractionHandler.InteractionParameters iParam) { if (iParam.axis == activationAxis && IsInteractiable) onButtonUp.Invoke(); }
    */

    #region Trigger Overrides

    public override void IHOnTriggerEnterEvents(Collider col)
    {
        if (!col.CompareTag("Player"))
            return;
        isProximal = true;
        base.IHOnTriggerEnterEvents(col);
        if (isLooking)
            OnMixedEnter.Invoke();
    }
    public override void IHOnTriggerStayEvents(Collider col) {
        if (!col.CompareTag("Player"))
            return;
        isProximal = true;
        base.IHOnTriggerStayEvents(col);
    }
    public override void IHOnTriggerExitEvents(Collider col)
    {
        if (!col.CompareTag("Player"))
            return;
        isProximal = false;
        if (isLooking)
            OnMixedExit.Invoke();
        base.IHOnTriggerExitEvents(col);
    }

    #endregion
}

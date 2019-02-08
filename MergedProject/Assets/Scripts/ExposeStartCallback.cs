using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposeStartCallback : MonoBehaviour {

    public InteractionHandler.InvokableState OnAwake;
    public InteractionHandler.InvokableState OnStart;

    void Awake() {
        OnAwake.Invoke();
    }

	void Start () {
        OnStart.Invoke();
	}
}

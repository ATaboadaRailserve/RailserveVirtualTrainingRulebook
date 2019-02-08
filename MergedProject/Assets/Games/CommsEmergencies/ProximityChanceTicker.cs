using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximityChanceTicker : MonoBehaviour {

    public float chance = 0.20f;
    public float tickrate = 1.0f;
    public InteractionHandler.InvokableState OnChanceOccurs;

    bool hasOccured;
    float t = 0;
    float rand;
	
	void OnTriggerStay (Collider c) {
		if(c.CompareTag("Player") && !hasOccured)
        {
            t += Time.deltaTime;
            if(t > tickrate)
            {
                t -= tickrate;
                rand = Random.Range(0.00f, 1.00f);
                if(rand <= chance)
                {
                    hasOccured = true;
                    OnChanceOccurs.Invoke();
                }
            }
        }
	}
}

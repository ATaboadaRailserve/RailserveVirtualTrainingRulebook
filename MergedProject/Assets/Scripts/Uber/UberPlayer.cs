using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UberPlayer : MonoBehaviour {

    public enum STATUS
    {
        NONE,
        EYES_no_movement,
        EYES_walking_movement,
        EYES_mounted_movement,
        EYES_riding_movement
    }

    private STATUS status;
    
	void Start () {
        status = STATUS.NONE;
	}

    public void SetStatus(STATUS newStatus)
    {
        
    }
}

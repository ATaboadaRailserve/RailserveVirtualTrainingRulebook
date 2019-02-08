using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StraightLineTrainMovement : MonoBehaviour {

    // m/s
    const float maxSpeed = 4.4704f;      // 10 mph
    const float maxAllowedSpeed = 2.2352f;      // 5 mph
    const float maxCrossingSpeed = 0.89408f;    // 2 mph
    const float mpsToMph = 2.23694f; // m/s to mph

    // dials
    const float maxDialSetting = 10.0f;
    const float dialMoveSpeed = 4.0f;

    public Transform direction;
    public Text speedUI;

    public float feelMultiplier = 1.0f;

    public InteractionHandler.InvokableState OnSpeedExceeded;
    public InteractionHandler.InvokableState OnSpeedReturned;
    
    float currentSpeed;
    float expectedSpeed;
    float currentDial;

    SpeedLimit speedLimit = SpeedLimit._5mph;
    SpeedingState speedingState = SpeedingState.not_speeding;

    public enum SpeedLimit
    {
        _5mph,
        _2mph
    }
	
    public enum SpeedingState
    {
        not_speeding,
        speeding,
    }

	void Update () {
        if (Input.GetKey(KeyCode.Comma))
            DialDown();
        else if (Input.GetKey(KeyCode.Period))
            DialUp();
        currentSpeed = Mathf.SmoothStep(currentSpeed, expectedSpeed, Time.deltaTime * feelMultiplier);
        transform.Translate(direction.forward * currentSpeed * Time.deltaTime, Space.World);

        float currentSpeedLimit = maxAllowedSpeed;
        if (speedLimit == SpeedLimit._2mph)
            currentSpeedLimit = maxCrossingSpeed;
        
        if(currentSpeed > currentSpeedLimit && speedingState == SpeedingState.not_speeding)
        {
            speedingState = SpeedingState.speeding;
            OnSpeedExceeded.Invoke();
        }
        else if(currentSpeed <= currentSpeedLimit && speedingState == SpeedingState.speeding)
        {
            speedingState = SpeedingState.not_speeding;
            OnSpeedReturned.Invoke();
        }

        if (speedUI != null)
            speedUI.text = ((int)(currentSpeed * mpsToMph)).ToString() + " mph";
	}

    public void DialUp()
    {
        currentDial += dialMoveSpeed * Time.deltaTime;
        currentDial = Mathf.Min(currentDial, maxDialSetting);
        expectedSpeed = Mathf.Lerp(0, maxSpeed, currentDial / maxDialSetting);
    }

    public void DialDown()
    {
        currentDial -= dialMoveSpeed * Time.deltaTime;
        currentDial = Mathf.Max(currentDial, 0.0f);
        expectedSpeed = Mathf.Lerp(0, maxSpeed, currentDial / maxDialSetting);
    }
}
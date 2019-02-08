using UnityEngine;
using System.Collections;

public class TouchscreenInput : MonoBehaviour
{
    void Update()
    {
        Touch myTouch = Input.GetTouch(0);
        Debug.Log(myTouch.fingerId);

        Touch[] myTouches = Input.touches;
        for (int i = 0; i < Input.touchCount; i++)
        {
            //Do something with the touches
            Debug.Log("Touch #" + i + ": " + myTouches[i].position);
        }
    }
}
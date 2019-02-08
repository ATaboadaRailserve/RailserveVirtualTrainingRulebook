//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WheelPusher : MonoBehaviour {

//	public Rigidbody redsdbody;
//	public float force = 100;
//	[HideInInspector]
//	public bool foward;
//	// Use this for initialization
//	void Start () {

//	}

//	// Update is called once per frame
//	void FixedUpdate () {
//		if (!Physics.Raycast(transform.position,-Vector3.up,0.1f))
//		{
//			return;
//		}
//		if (Input.GetKey (KeyCode.W)) {
//			redsdbody.AddForce (transform.forward * force);
//			foward = true;
//		}
//		if (Input.GetKey (KeyCode.S)) {
//			redsdbody.AddForce (-transform.forward * force);
//			foward = false;
//		}
//	}
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPusher : MonoBehaviour
{


    public float force;
    public float maxSpeed; //in mph
    public float brakeForce;
    public Speedometer speedometer;

    [HideInInspector]
    public Rigidbody redsdbody;
    [HideInInspector]
    public bool foward;

    private float distance; //in feet
    private bool move = false;
    private Vector3 moveDirection;
    private float stopDistance;
    private float maxAccel;
    private float lastMPH;
    // Use this for initialization
    void Start()
    {
        redsdbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (move)
        {
            if (Mathf.Abs(lastMPH - speedometer.currentSpeed) > maxAccel)
            {
                maxAccel = Mathf.Abs(lastMPH - speedometer.currentSpeed);
            }
            if (distance - speedometer.distanceTraveled < 0)
            {
                ThatWillDo();
            }
            else if (distance - speedometer.distanceTraveled < 52f)
            {
                if (speedometer.currentSpeed < 1f)
                {
                    Accelerate();
                }
            }
            else if (distance / 2f - speedometer.distanceTraveled > CalculateStopDistance())
            {
                if (speedometer.currentSpeed < maxSpeed)
                {
                    Accelerate();
                }
            }
            else
            {
                Decelerate();
            }
            lastMPH = speedometer.currentSpeed;
        }
    }
    public float CalculateStopDistance()
    {
        if (maxAccel < .001f)
            return 0;
        return ((speedometer.currentSpeed / maxAccel) / 3600f) * (speedometer.currentSpeed / 2f) * 5280f * 4f;
    }
    public void ThatWillDo()
    {
        move = false;
        Brake();
    }
    public void Brake()
    {
        redsdbody.drag = brakeForce;
    }
    public void UnBrake()
    {
        redsdbody.drag = 0;
    }
    public void Accelerate()
    {
        redsdbody.AddForce(moveDirection * force);
    }
    public void Decelerate()
    {
        if (speedometer.currentSpeed > .2f)
        {
            redsdbody.AddForce(-moveDirection * force);
        }
        else {
            Brake();
        }
    }
    public void MoveBack(float m_distance)
    {
        foward = false;
        move = true;
        distance = m_distance;
        moveDirection = -transform.forward;
        speedometer.distanceTraveled = 0;
        stopDistance = 0;
        UnBrake();
    }

    public void MoveFoward(float m_distance)
    {
        foward = true;
        move = true;
        distance = m_distance;
        moveDirection = transform.forward;
        speedometer.distanceTraveled = 0;
        stopDistance = 0;
        UnBrake();
    }
    public void ContinueCall(float m_distance)
    {
        distance = distance - speedometer.distanceTraveled;
        speedometer.distanceTraveled = 0;
        UnBrake();
    }
    public float GetDistanceRemaining
    {
        get { return distance - speedometer.distanceTraveled; }
    }
}
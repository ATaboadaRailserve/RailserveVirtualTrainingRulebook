using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    Vector3 tempVec3;
	// Use this for initialization
	void Start () {

		this.transform.position = transform.position;
		this.transform.eulerAngles = new Vector3 (2.9f, -4.3f, 0.0f);
        tempVec3 = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPreRender() {
		GL.wireframe = false;
	}
	void OnPostRender() {
		GL.wireframe = false;
	}

    public void LateUpdate()
    {
		if(Input.touchCount > 0)
		{
			if (Input.GetTouch (0).phase == TouchPhase.Moved) 
			{
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				if (touchDeltaPosition.x > 0) 
				{
					transform.Rotate(Vector3.up, (float)Time.deltaTime * 30.0f, Space.World);
				}
				if (touchDeltaPosition.x < 0) 
				{
					transform.Rotate(-Vector3.up, (float)Time.deltaTime * 30.0f, Space.World);
				}

				if (touchDeltaPosition.y > 0) 
				{
                    tempVec3.Set(-1.0f, 0.0f, 0.0f);
                    transform.Rotate(tempVec3, (float)Time.deltaTime * 30.0f, Space.Self);
				}
				if (touchDeltaPosition.y < 0) 
				{
                    tempVec3.Set(1.0f, 0.0f, 0.0f);
                    transform.Rotate(tempVec3, (float)Time.deltaTime * 30.0f, Space.Self);
				}
			}
		}
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Rotate(-Vector3.up, (float)Time.deltaTime * 100.0f, Space.World);
            }
            else
            {
                tempVec3.Set(-0.5f, 0.0f, 0.0f);
                transform.Translate(tempVec3, Space.Self);

            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Rotate(Vector3.up, (float)Time.deltaTime * 100.0f, Space.World);
            }
            else
            {
                tempVec3.Set(0.5f, 0.0f, 0.0f);
                transform.Translate(tempVec3, Space.Self);
            }
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            tempVec3.Set(0.0f, 0.0f, 1.0f);
            Vector3 vDir = transform.TransformDirection(tempVec3);
            vDir.y = 0.0f;
            transform.Translate(vDir*0.5f, Space.World);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            tempVec3.Set(0.0f, 0.0f, -1.0f);
            Vector3 vDir = transform.TransformDirection(tempVec3);
            vDir.y = 0.0f;
            transform.Translate(vDir*0.5f, Space.World);
        }

        if (Input.GetKey(KeyCode.PageUp))
        {
            tempVec3.Set(0.0f, 1.0f, 0.0f);
            transform.Translate(tempVec3, Space.World);
        }

        if (Input.GetKey(KeyCode.PageDown))
        {
            tempVec3.Set(0.0f, -1.0f, 0.0f);
            transform.Translate(tempVec3, Space.World);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            tempVec3.Set(1.0f, 0.0f, 0.0f);
            transform.Rotate(tempVec3, (float)Time.deltaTime * 100.0f, Space.Self);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            tempVec3.Set(-1.0f, 0.0f, 0.0f);
            transform.Rotate(tempVec3, (float)Time.deltaTime * 100.0f, Space.Self);
        }


    }
}

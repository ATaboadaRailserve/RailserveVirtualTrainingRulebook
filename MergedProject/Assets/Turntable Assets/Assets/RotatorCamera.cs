using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotatorCamera : MonoBehaviour {

    GameObject rotator;
    GameObject focus;
    bool following;
    bool returning;
    public bool tabletMode;
    public float sensitivity;
    Vector3 velocity;

    Vector2 curPosition;
    Vector2 lastPosition;

    //prevent rotating object when over UI
    bool startOnUI;

    //Use with existing tutorial
    GameObject tutorial;
    bool exploded;
    bool rotated;
    bool returned;

    MeshRenderer curMR;

    public GameObject rotatorUI;
	
	public bool useCursorLock = true;

    GameObject curClickedObj;

	// Use this for initialization
	void Start () {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;
		
        transform.tag = "RotatorCamera";
        focus = transform.parent.Find("Focus").gameObject;
        following = false;
        returning = false;
        velocity = new Vector3();
        rotatorUI.GetComponent<Canvas>().enabled = false;
        rotated = false;
        returned = false;
        tutorial = GameObject.Find("Tutorial");

        if (tabletMode)
        {
            sensitivity /= 10;
        }
    }
	
	// Update is called once per frame
	void Update () {
        
	    if(!following)
        {
			if (useCursorLock)
				Cursor.visible = true;
            ClickItem();
        }
        else if(!returning)
        {
            RotateItem();
        }
	}
    void RotateItem()
    {
        if(tabletMode)
        {

            #region
            if (startOnUI)
            {
                if (Input.touchCount < 1)
                    startOnUI = false;
                //startOnUI = Input.GetMouseButton(0);
            }
            else
            {
                if (Input.touchCount > 0 && IsPointerOverUIObject())
                {
                    startOnUI = true;
                    return;
                }
                //If there is at least one finger on the screen
                if (Input.touchCount == 1)
                {
                    if (!rotated)
                    {
                        if(tutorial!= null)
                            tutorial.GetComponent<Tutorial>().Next();
                        rotated = true;
                    }
                    //Update the last position and current position depending on if a new touch or already touching
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        curPosition = Input.touches[0].position;
                        lastPosition = curPosition;
                    }
                    else
                    {
                        lastPosition = curPosition;
                        curPosition = Input.touches[0].position;
                    }
                    //rotates along the vertical y axis
                    velocity.x = curPosition.x - lastPosition.x;
                    //rotates along the horizontal x axis
                    velocity.y = curPosition.y - lastPosition.y;
                    //inverse because on a tablet
                    velocity.y *= -1;
                    //multiply by sensitivity
                    velocity *= sensitivity;
                    //rotates around the correct axis
                    curMR = rotator.GetComponent<MeshRenderer>();
                    if (curMR != null)
                    {
                        rotator.transform.RotateAround(curMR.bounds.center, Vector3.up, velocity.x);
                        rotator.transform.RotateAround(curMR.bounds.center, transform.right, velocity.y);
                    }
                    else
                    {
                        rotator.transform.RotateAround(rotator.transform.position, Vector3.up, velocity.x);
                        rotator.transform.RotateAround(rotator.transform.position, transform.right, velocity.y);
                    }
                }
                else
                {
                    //Used for decelerating the object more smoothly after the user lets go of middle mouse button (or releases finger)
                    if (velocity.x > .5f)
                    {
                        velocity.x -= .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, Vector3.up, velocity.x);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, Vector3.up, velocity.x);
                    }
                    else if (velocity.x < -.5f)
                    {
                        velocity.x += .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, Vector3.up, velocity.x);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, Vector3.up, velocity.x);
                    }
                    if (velocity.y > .5f)
                    {
                        velocity.y -= .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, transform.right, velocity.y);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, transform.right, velocity.y);
                    }
                    else if (velocity.y < -.5f)
                    {
                        velocity.y += .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, transform.right, velocity.y);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, transform.right, velocity.y);
                    }
                }
            }
            #endregion
            
        }
        else
        {
            #region
            if (startOnUI)
            {
                startOnUI = Input.GetMouseButton(0);
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
                {
                    startOnUI = true;
                    return;
                }
                if (Input.GetMouseButton(0))
                {
                    if (!rotated)
                    {
                        if(tutorial != null)
                            tutorial.GetComponent<Tutorial>().Next();
                        rotated = true;
                    }
                    //rotates along the vertical y axis
                    velocity.x = Input.GetAxis("Mouse X") * -sensitivity;
                    //rotates along the horizontal x axis
                    velocity.y = Input.GetAxis("Mouse Y") * sensitivity;
                    //rotate around the correct axis
                    curMR = rotator.GetComponent<MeshRenderer>();
                    if (curMR != null)
                    {
                        rotator.transform.RotateAround(curMR.bounds.center, Vector3.up, velocity.x);
                        rotator.transform.RotateAround(curMR.bounds.center, transform.right, velocity.y);
                    }
                    else
                    {
                        rotator.transform.RotateAround(rotator.transform.position, Vector3.up, velocity.x);
                        rotator.transform.RotateAround(rotator.transform.position, transform.right, velocity.y);
                    }
                }
                else
                {
                    //Used for decelerating the object more smoothly after the user lets go of middle mouse button (or releases finger)
                    if (velocity.x > .5f)
                    {
                        velocity.x -= .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, Vector3.up, velocity.x);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, Vector3.up, velocity.x);
                    }
                    else if (velocity.x < -.5f)
                    {
                        velocity.x += .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, Vector3.up, velocity.x);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, Vector3.up, velocity.x);
                    }
                    if (velocity.y > .5f)
                    {
                        velocity.y -= .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, transform.right, velocity.y);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, transform.right, velocity.y);
                    }
                    else if (velocity.y < -.5f)
                    {
                        velocity.y += .1f;
                        curMR = rotator.GetComponent<MeshRenderer>();
                        if (curMR != null)
                            rotator.transform.RotateAround(curMR.bounds.center, transform.right, velocity.y);
                        else
                            rotator.transform.RotateAround(rotator.transform.position, transform.right, velocity.y);
                    }
                }
            }
            #endregion
        }
    }

    bool ClickItem()
    {
        if (tabletMode)
        {
            #region
                Ray ray;
                RaycastHit hit;
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    ray = transform.GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        curClickedObj = hit.collider.gameObject;
                    }
                }
                else if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    ray = transform.GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (following)
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Rotator")
                                {
                                    Return();
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Rotator")
                                {
                                    Explode(curClickedObj);
                                    return true;
                                }
                            }
                        }
                    }
                }
            return false;
            #endregion
        }
        else
        {
            #region
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
                Ray ray;
                RaycastHit hit;
                if (Input.GetMouseButtonDown(0))
                {
                    ray = transform.GetComponent<Camera>().ScreenPointToRay(new Vector3(0.5f, 0.5f, 0));
                    if (Physics.Raycast(transform.position, transform.forward, out hit))
                    {
                        curClickedObj = hit.collider.gameObject;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    ray = transform.GetComponent<Camera>().ScreenPointToRay(new Vector3(0.5f, 0.5f, 1));
                    if (Physics.Raycast(transform.position, transform.forward, out hit))
                    {
                        if (following)
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Rotator")
                                {
                                    Return();
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Rotator")
                                {
                                    Explode(curClickedObj);
                                    return true;
                                }
                            }
                        }
                    }
                }
            //}
            return false;
            #endregion
        }
    }

    //Taken from http://answers.unity3d.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html#answer-1115473 to fix problem
    //with clicking on UI objects
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        if (tabletMode)
        {
            if (Input.touchCount != 1)
            {
                return true;
            }
            else
            {
                eventDataCurrentPosition.position = new Vector2(Input.touches[0].position.x, Input.touches[0].position.y);
            }
        }
        else
        {
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void Explode (GameObject rot)
    {
        transform.parent.GetComponent<CharacterMotor>().canControl = false;
        returning = false;
        //"Enabled" is turned on at a certain point during the tutorial to prevent breaking tutorial
        if(!rotatorUI.transform.FindChild("Enabled").gameObject.activeSelf)
        {
            return;
        }
        //Set description and name in ui and enable
        rotatorUI.transform.GetChild(0).Find("Name_Background").GetChild(0).GetComponent<Text>().text = rot.name;
        rotatorUI.transform.GetChild(0).Find("Description_Background").GetChild(0).GetComponent<Text>().text = rot.GetComponent<Rotator>().description;
        rotatorUI.GetComponent<Canvas>().enabled = true;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
        rotator = rot;
        rotator.GetComponent<Rotator>().Explode();
        curMR = rotator.GetComponent<MeshRenderer>();
        if (curMR != null)
        {
            Vector3 displacement = curMR.bounds.center - rotator.transform.position;
            focus.transform.LookAt(rotator.GetComponent<Rotator>().targetPos + displacement);
        }
        else
            focus.transform.LookAt(rotator.GetComponent<Rotator>().targetPos);
        transform.parent.GetComponent<MouseLook>().enabled = false;
        transform.parent.GetComponent<FPSInputController>().enabled = false;
        transform.GetComponent<MouseLook>().enabled = false;
        following = true;
        if (!exploded)
        {
            if(tutorial!= null)
                tutorial.GetComponent<Tutorial>().Next();
            exploded = true;
        }
        StartCoroutine("Following");
    }

    public void Return ()
    {
        transform.parent.GetComponent<CharacterMotor>().canControl = true;
        returning = true;
        velocity = new Vector3();
        rotator.GetComponent<Rotator>().Return();
        rotatorUI.GetComponent<Canvas>().enabled = false;
        focus.transform.LookAt(rotator.GetComponent<Rotator>().startPos);
        if (!returned)
        {
            if(tutorial!= null)
                tutorial.GetComponent<Tutorial>().Next();
            returned = true;
        }
    }

    public void FinishReturning()
    {
        StopCoroutine("Following");
        
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;
        following = false;
        transform.parent.GetComponent<MouseLook>().enabled = true;
        transform.parent.GetComponent<FPSInputController>().enabled = true;
        transform.GetComponent<MouseLook>().enabled = true;
    }

    IEnumerator Following()
    {
        while (following)
        {
            Debug.Log("Following...");
            yield return new WaitForEndOfFrame();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, focus.transform.rotation, 2);
        }
    }
}

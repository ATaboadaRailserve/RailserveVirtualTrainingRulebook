using UnityEngine;
using System.Collections;
using Rewired;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;

public class TurnTable : MonoBehaviour {
    public GameObject mainCamera;
    Vector3 startPosition;
    Quaternion startRotation;

    bool rotating;
    Vector2 velocity;

    //Used for touchscreen
    Vector2 lastPosition;
    Vector2 curPosition;

    Touch t1;
    Touch t2;
    float lastSeperation;
    float curSeperation;

    //For database messages
    DatabaseMessageHolder messageHolder;
    

    //For moving camera in/out to zoom in/out

    float startDisplacement;
    float displacement;
    public float minDisplacement;
    public float maxDisplacement;

    Exploder curExploder;
    bool explodedView;

    //Used to prevent inputs if you start on UI
    bool startOnUI;

    //Explodable last selected (can be accessed to rotate between different models)
    Explodable curExplodable;


    //Object to change the models of parts of the explodable
    public GameObject changeModel;

    //For moving the camera when you click another object
    Vector3 targetPos;
    Quaternion targetRot;
    bool moving;

    //For zooming into an object when you click
    bool zooming;
    float zoomTarget;

    //Drawing a line to the text
    GameObject selectedObj;

    //Object last clicked on
    GameObject curClickedObj;

    bool rotator;
    GameObject rotatorObj;
    Quaternion rotatorObjInitialRot;

    //Sounds
    public AudioClip explodeSound;
    public AudioClip returnSound;
    public AudioClip selectSound;


    bool expanding;
    bool returning;

    //UI instances to show info about the object
    public GameObject objectinfo;
    [HideInInspector]
    public GameObject ui_name;
    [HideInInspector]
    public GameObject ui_description;
    GameObject name_outline;
    GameObject description_outline;

    //how fast the camera rotates
    public float sensitivity;
    public float zoomSensitivity;
    //The boundaries the camerapivot can be rotated up and down
    [Range(0f, 90f)]
    public float rotBoundaries;
    public LayerMask currentLayer;

    public bool tabletMode;

	private float zoomAmount = 5;
	private float disp = 1;

    // Use this for initialization
    void Start ()
    {
        //tabletMode = Input.touchSupported;
        //Lock the cursor back to the playercontroller
        ////Cursor.lockState = CursorLockMode.Locked;
        ////Cursor.visible = false;

        if(tabletMode)
        {
            sensitivity *= .6f;
            zoomSensitivity /= 15;
        }

        expanding = false;
        returning = false;

        targetRot = new Quaternion(0,0,0,1);

        startOnUI = false;
        explodedView = false;
        startPosition = transform.parent.position;
        startRotation = transform.rotation;
        rotating = false;
        velocity = new Vector2();
        moving = false;
        startDisplacement = transform.localPosition.z;
        displacement = startDisplacement;


        ui_name = objectinfo.transform.FindChild("Name_Background").gameObject;
        ui_description = objectinfo.transform.FindChild("Description_Background").gameObject;
        name_outline = objectinfo.transform.FindChild("OUTLINE_Name").gameObject;
        description_outline = objectinfo.transform.FindChild("OUTLINE_Description").gameObject;

        ui_name.SetActive(false);
        ui_description.SetActive(false);
        name_outline.SetActive(false);
        description_outline.SetActive(false);
        objectinfo.SetActive(false);

        messageHolder = GameObject.Find("MessageHolder").GetComponent<DatabaseMessageHolder>();
    }

    // Update is called once per frame
    void Update() {
        if (returning)
        {
            Returning();
        }
        else if (expanding)
        {
            Expanding();
        }
        else
        {
            if (moving)
                Move();
            else if (!ClickItem())
                RotateObject();
            
            //transform.rotation = CameraRotation(transform.gameObject);
            ZoomObject();
        }
        transform.LookAt(transform.parent.position);
    }

    void Returning()
    {
        if(Vector3.Distance(startPosition, mainCamera.transform.position) < .05f) //&& Quaternion.Angle()
        {
            mainCamera.transform.position = startPosition;
            mainCamera.transform.rotation = startRotation;
            curExploder.Return();
            NewCamera.Return();
            ////mainCamera.transform.parent.GetComponent<RigidbodyFirstPersonController>().enabled = true;
            ////mainCamera.GetComponent<HeadBob>().enabled = true;
            returning = false;
            
        }
        else
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, startPosition, Time.deltaTime *2);
            //mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, startRotation, Time.deltaTime );
            mainCamera.transform.LookAt(curExploder.transform.position);
            if (moving)
                Move();
        }
    }

    void Expanding()
    {
        Debug.Log("Expanding...");
        if (Vector3.Distance(mainCamera.transform.position, transform.position) < .05f)
        {
            //transform.parent.rotation = Quaternion.Euler(mainCamera.transform.rotation.x, 0, 0);
            mainCamera.transform.position = transform.position;
            mainCamera.transform.rotation = transform.rotation;
            curExploder.Explode();
            /*if (curExploder.explodables.Length < 2)
                SelectItem(curExploder.explodables[0].gameObject);*/
            Debug.Log("TEST");
            expanding = false;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
        }
        else
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position, Time.deltaTime *2);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, transform.rotation, Time.deltaTime *2);
            //mainCamera.transform.LookAt(curExploder.transform.position);\
            if (moving)
                Move();
        }
    }

    void RotateObject()
    {
        if (tabletMode)
        {
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
                    velocity *= sensitivity * Time.deltaTime;
                    //rotates around the correct axis
                    if (transform.parent.rotation.eulerAngles.x + velocity.y >= rotBoundaries && transform.parent.rotation.eulerAngles.x <= 180 && velocity.y >= 0)
                    {
                        transform.parent.eulerAngles = new Vector3(rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.Rotate(0, velocity.x, 0);
                    }
                    else if (transform.parent.rotation.eulerAngles.x + velocity.y <= 360 - rotBoundaries && transform.parent.rotation.eulerAngles.x >= 180 && velocity.y <= 0)
                    {
                        transform.parent.eulerAngles = new Vector3(360 - rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.Rotate(0, velocity.x, 0);
                    }
                    else
                        transform.parent.Rotate(velocity.y, velocity.x, 0);
                }
                else
                {
                    //Used for decelerating the object more smoothly after the user lets go of middle mouse button (or releases finger)
                    if (velocity.x > .1f + (5f * Time.deltaTime))
                    {
                        velocity.x -= 5f * Time.deltaTime;
                    }
                    else if (velocity.x < -.1f - (5f * Time.deltaTime))
                    {
                        velocity.x += 5f * Time.deltaTime;
                    }
                    else
                    {
                        velocity.x = 0;
                    }
                    if (velocity.y > .1f + (5f * Time.deltaTime))
                    {
                        velocity.y -= 5f * Time.deltaTime;
                    }
                    else if (velocity.y < -.1f - (5f * Time.deltaTime))
                    {
                        velocity.y += 5f * Time.deltaTime;
                    }
                    else
                    {
                        velocity.y = 0;
                    }


                    if (transform.parent.rotation.eulerAngles.x + velocity.y >= rotBoundaries && transform.parent.rotation.eulerAngles.x <= 180 && velocity.y >= 0)
                    {
                        transform.parent.eulerAngles = new Vector3(rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.Rotate(0, velocity.x, 0);
                    }
                    else if (transform.parent.rotation.eulerAngles.x + velocity.y <= 360 - rotBoundaries && transform.parent.rotation.eulerAngles.x >= 180 && velocity.y <= 0)
                    {
                        transform.parent.eulerAngles = new Vector3(360 - rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.Rotate(0, velocity.x, 0);
                    }
                    else
                        transform.parent.Rotate(velocity.y, velocity.x, 0);
                }
            }
        }
        else
        {
            if (startOnUI)
            {
                startOnUI = Input.GetMouseButton(0);
            }
            else
            {
                if(Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
                {
                    startOnUI = true;
                    return;
                }
                if (Input.GetMouseButton(0))
                {
                    //rotates along the vertical y axis
                    velocity.x = Input.GetAxis("Mouse X") * sensitivity;
                    //rotates along the horizontal x axis
                    velocity.y = Input.GetAxis("Mouse Y") * -sensitivity;
                    //rotate around the correct axis
                    if (transform.parent.rotation.eulerAngles.x + velocity.y > rotBoundaries && transform.parent.rotation.eulerAngles.x < 180 && velocity.y > 0)
                    {
                        transform.eulerAngles = new Vector3(rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.RotateAround(transform.parent.position, Vector3.up, velocity.x);
                    }
                    else if (transform.parent.rotation.eulerAngles.x + velocity.y < 360 - rotBoundaries && transform.parent.rotation.eulerAngles.x > 180 && velocity.y < 0)
                    {
                        transform.eulerAngles = new Vector3(360 - rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.RotateAround(transform.parent.position, Vector3.up, velocity.x);
                    }
                    else
                    {
                        transform.parent.RotateAround(transform.parent.position, Vector3.up, velocity.x);
                        transform.parent.Rotate(velocity.y, 0, 0);
                        //transform.parent.Rotate(velocity.y, velocity.x, 0);
                    }
                }
                else
                {
                    //Used for decelerating the object more smoothly after the user lets go of middle mouse button (or releases finger)
                    if (velocity.x > .1f + (5f * Time.deltaTime))
                    {
                        velocity.x -= 5f * Time.deltaTime;
                        //transform.parent.RotateAround(transform.parent.position, Vector3.up, velocity.x);
                    }
                    else if (velocity.x < -.1f - (5f * Time.deltaTime))
                    {
                        velocity.x += 5f * Time.deltaTime;
                        //transform.parent.RotateAround(transform.parent.position, Vector3.up, velocity.x);
                    }
                    else
                    {
                        velocity.x = 0;
                    }
                    if (velocity.y > .1f + (5f * Time.deltaTime))
                    {
                        velocity.y -= 5f * Time.deltaTime;
                        //transform.parent.Rotate(velocity.y, 0, 0);
                    }
                    else if (velocity.y < -.1f - (5f * Time.deltaTime))
                    {
                        velocity.y += 5f * Time.deltaTime;
                        //transform.parent.Rotate(velocity.y, 0, 0);
                    }
                    else
                    {
                        velocity.y = 0;
                    }

                    if (transform.parent.rotation.eulerAngles.x + velocity.y >= rotBoundaries && transform.parent.rotation.eulerAngles.x <= 180 && velocity.y >= 0)
                    {
                        transform.parent.eulerAngles = new Vector3(rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.Rotate(0, velocity.x, 0);
                    }
                    else if (transform.parent.rotation.eulerAngles.x + velocity.y <= 360 - rotBoundaries && transform.parent.rotation.eulerAngles.x >= 180 && velocity.y <= 0)
                    {
                        transform.parent.eulerAngles = new Vector3(360 - rotBoundaries, transform.eulerAngles.y, transform.eulerAngles.z);
                        transform.parent.Rotate(0, velocity.x, 0);
                    }
                    else
                        transform.parent.Rotate(velocity.y, velocity.x, 0);
                }
				
				zoomAmount-=Input.GetAxis("Mouse ScrollWheel");
				zoomAmount = Mathf.Clamp(zoomAmount, -minDisplacement - disp, -maxDisplacement);
				ZoomIn(zoomAmount);
                
            }
        }
    }

    //Taken from http://answers.unity3d.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html#answer-1115473 to fix problem
    //with clicking on UI objects
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        if (tabletMode)
        {
            if(Input.touchCount != 1)
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

    bool ClickItem()
    {
        if (tabletMode)
        {
            #region
            if (!IsPointerOverUIObject())
            {
                GameObject.Find("OVER_UI").GetComponent<Text>().text = "not over ui";
                Ray ray;
                RaycastHit hit;
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    if (explodedView)
                        ray = GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    else
                        ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        curClickedObj = hit.collider.gameObject;
                    }
                }
                else if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    if (explodedView)
                        ray = GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    else
                        ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (explodedView)
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Exploder")
                                {
                                    explodedView = false;
                                    curExploder = hit.collider.gameObject.GetComponent<ExploderButton>().exploder.GetComponent<Exploder>();
                                    curExploder.Return();
                                    Return();
                                    return true;
                                }
                                else if (hit.collider.tag == "Explodable")
                                {
                                    //hit.collider.gameObject.GetComponent<Explodable>().Select();
                                    SelectItem(hit.collider.gameObject);
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Exploder")
                                {
                                    if (Vector3.Distance(mainCamera.transform.position, hit.collider.gameObject.transform.position) < 5)
                                    {
                                        GameObject instructions = GameObject.Find("Instruction_Canvas");
                                        if (instructions != null)
                                            Destroy(instructions.gameObject);
                                        transform.parent.position = hit.collider.GetComponent<ExploderButton>().exploder.transform.position;
                                        explodedView = true;
                                        curExploder = hit.collider.gameObject.GetComponent<ExploderButton>().exploder.GetComponent<Exploder>();
                                        //curExploder.Explode();
                                        Explode(hit.collider.gameObject.GetComponent<ExploderButton>().exploder.transform);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("OVER UI");
                GameObject.Find("OVER_UI").GetComponent<Text>().text = "OVER UI";
            }
            return false;
            #endregion
        }
        else
        {
            #region
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GameObject.Find("OVER_UI").GetComponent<Text>().text = "not over ui";
                Ray ray;
                RaycastHit hit;
                if (Input.GetMouseButtonDown(0))
                {
                    if (explodedView)
                        ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    else
                        ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        curClickedObj = hit.collider.gameObject;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (explodedView)
                        ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    else
                        ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (explodedView)
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Exploder")
                                {
                                    explodedView = false;
                                    curExploder = hit.collider.gameObject.GetComponent<ExploderButton>().exploder.GetComponent<Exploder>();
                                    curExploder.Return();
                                    Return();
                                    return true;
                                }
                                else if (hit.collider.tag == "Explodable")
                                {
                                    //hit.collider.gameObject.GetComponent<Explodable>().Select();
                                    SelectItem(hit.collider.gameObject);
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider.gameObject == curClickedObj)
                            {
                                if (hit.collider.tag == "Exploder")
                                {
                                    if (Vector3.Distance(mainCamera.transform.position, hit.collider.gameObject.transform.position) < 5)
                                    {
                                        GameObject instructions = GameObject.Find("Instruction_Canvas");
                                        if (instructions != null)
                                            Destroy(instructions.gameObject);
                                        transform.parent.position = hit.collider.GetComponent<ExploderButton>().exploder.transform.position;
                                        explodedView = true;
                                        curExploder = hit.collider.gameObject.GetComponent<ExploderButton>().exploder.GetComponent<Exploder>();
                                        //curExploder.Explode();
                                        Explode(hit.collider.gameObject.GetComponent<ExploderButton>().exploder.transform);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("OVER UI");
                GameObject.Find("OVER_UI").GetComponent<Text>().text = "OVER UI";
            }
            return false;
            #endregion
        }
    }

    public void SelectItem(GameObject nextItem)
    {
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<AudioSource>().PlayOneShot(selectSound);
        //Remove tutorial to selectsubobject
        GameObject instructions = GameObject.Find("Instruction_Swipe");
        if (instructions != null)
            Destroy(instructions.gameObject);
        ui_description.transform.GetChild(1).GetComponent<Scrollbar>().value = 1;
        if (nextItem.tag == "Explodable")
        {
            curExplodable = nextItem.GetComponent<Explodable>();
            if(curExplodable.transform.childCount > 1 && !curExplodable.ignoreChildren)
            {
                changeModel.gameObject.SetActive(true);
            }
            else
            {
                changeModel.gameObject.SetActive(false);
            }
        }
        //zooming = false;
        ZoomIn(nextItem);
        Debug.Log("SELECTED");
        moving = true;
        //targetPos = nextItem.transform.position;
        if (nextItem.GetComponent<MeshRenderer>() != null)
            targetPos = nextItem.GetComponent<Renderer>().bounds.center;
        else
            targetPos = nextItem.GetComponent<Explodable>().targetPos;
        nextItem.transform.GetComponent<Explodable>().Select();
        ui_name.SetActive(true);
        ui_description.SetActive(true);
        name_outline.SetActive(true);
        description_outline.SetActive(true);
    }

    void Move()
    {
        if (Vector3.Distance(transform.parent.position, targetPos) > .1f)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, targetPos, Time.deltaTime * 5);
            //transform.parent.Translate((targetPos - transform.parent.position) * Time.deltaTime);
        }
        else
        {
            transform.parent.position = targetPos;
            moving = false;
        }
    }

    public void ZoomIn(GameObject zoomObject)
    {
        Debug.Log(zoomObject.GetComponent<Renderer>().bounds.extents.magnitude);
        //float disp = (-2.8f * zoomObject.GetComponent<Renderer>().bounds.extents.magnitude) + 2.5f;
        if (zoomObject.GetComponent<Explodable>() != null && zoomObject.GetComponent<Explodable>().overrideZoom)
            disp = zoomObject.GetComponent<Explodable>().overrideZoomAmount;
        else
            disp = (-4f * zoomObject.GetComponent<Renderer>().bounds.extents.magnitude) + 2.5f;
        zooming = true;
        zoomTarget = minDisplacement + disp;
        if (zoomTarget < maxDisplacement)
            zoomTarget = maxDisplacement;
        else if (zoomTarget > minDisplacement)
            zoomTarget = minDisplacement;
		zoomAmount = -zoomTarget;
    }

    public void ZoomIn(float scale)
    {
        zooming = true;
        zoomTarget = -(scale);
        Debug.Log(zoomTarget);
    }
    
    void ZoomObject()
    {
        if (!zooming)
        {
            if (tabletMode)
            {
                if (Input.touchCount >= 2)
                {
                    t1 = Input.touches[0];
                    t2 = Input.touches[1];
                    Vector2 difference = t1.position - t2.position;
                    if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
                    {
                        curSeperation = difference.magnitude;
                        lastSeperation = curSeperation;
                    }
                    else
                    {
                        lastSeperation = curSeperation;
                        curSeperation = difference.magnitude;
                    }

                    displacement += (curSeperation - lastSeperation) * sensitivity * Time.deltaTime;
                    displacement = Mathf.Clamp(displacement, maxDisplacement, minDisplacement);
                    transform.localPosition = new Vector3(0, 0, displacement);

                }
            }
            else
            {
                displacement += Input.GetAxis("Mouse ScrollWheel") * sensitivity * Time.deltaTime;
                displacement = Mathf.Clamp(displacement, maxDisplacement, minDisplacement);
                transform.localPosition = new Vector3(0, 0, displacement);
            }
        }
        else
        {
            if (displacement > zoomTarget + Time.deltaTime * 15)
            {
                displacement -= Time.deltaTime * 15;
            }
            else if (displacement < zoomTarget - Time.deltaTime * 15)
            {
                displacement += Time.deltaTime * 15;
            }
            else
            {
                displacement = zoomTarget;
                zooming = false;
            }
            displacement = Mathf.Clamp(displacement, maxDisplacement, minDisplacement);
            transform.localPosition = new Vector3(0, 0, displacement);
        }
        
    }

    public void Explode(Transform exploderObj)
    {
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<AudioSource>().PlayOneShot(explodeSound);
        //Remove tutorial to press button
        GameObject instructions = GameObject.Find("Instruction_Tap");
        if (instructions != null)
            Destroy(instructions.gameObject);
        objectinfo.SetActive(true);
        if(exploderObj.GetComponent<Exploder>().sideways)
            transform.parent.rotation = Quaternion.Euler(30, 90, 0);
        else
            transform.parent.rotation = Quaternion.Euler(30, 0, 0);
        transform.localPosition = new Vector3(0, 0, -3 * exploderObj.GetComponent<Exploder>().scale);
        zoomTarget = -3 * exploderObj.GetComponent<Exploder>().scale;
        displacement = zoomTarget;
        ////mainCamera.transform.parent.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        ////mainCamera.GetComponent<HeadBob>().enabled = false;
        startPosition = mainCamera.transform.position;
        startRotation = mainCamera.transform.rotation;
        expanding = true;
        //Unlock the cursor so you can click anywhere on screen
        ////Cursor.lockState = CursorLockMode.None;
        ////Cursor.visible = true;
        gameObject.GetComponent<Grayscale>().enabled = true;
        targetPos = exploderObj.transform.position + exploderObj.GetComponent<Exploder>().displacement;
        moving = true;
        //zoomTarget = (maxDisplacement + minDisplacement) / 2;
        //ZoomIn(exploderObj.GetComponent<Exploder>().scale);
        //displacement = maxDisplacement;
        zooming = true;
    }

    //When the user presses the button to return the exploder
    public void ReturnExploder()
    {
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<AudioSource>().PlayOneShot(returnSound);
        explodedView = false;
        curExploder.Return();
        Return();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;

        ui_name.SetActive(false);
        ui_description.SetActive(false);
        name_outline.SetActive(false);
        description_outline.SetActive(false);
        objectinfo.SetActive(false);

    }

    //When the user presses the button to explode the exploder
    public void ExplodeExploder(GameObject e)
    {
        
        curExploder = e.GetComponent<Exploder>();
        GameObject instructions = GameObject.Find("Instruction_Canvas");
        if (instructions != null)
            Destroy(instructions.gameObject);
        transform.parent.position = e.transform.position;
        explodedView = true;
        //curExploder.Explode();
        messageHolder.WriteMessage("User viewed " + e.name + " object in encyclopedia.");
        Explode(e.transform);
    }

    public void Return()
    {
        mainCamera.transform.position = transform.position;
        mainCamera.transform.rotation = transform.rotation;
        velocity = new Vector2();
        returning = true;
        //Lock the cursor back to the playercontroller
        ////Cursor.lockState = CursorLockMode.Locked;
        ////Cursor.visible = false;
        ui_name.SetActive(false);
        ui_description.SetActive(false);
        moving = true;
        targetPos = startPosition;
        changeModel.gameObject.SetActive(false);
    }

    //Used to change between different explodable models
    public void NextModel()
    {
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<AudioSource>().PlayOneShot(selectSound);
        curExplodable.NextModel();
    }

    //Used to change between different explodable models
    public void PrevModel()
    {
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent.GetComponent<AudioSource>().PlayOneShot(selectSound);
        curExplodable.PrevModel();
    }
}

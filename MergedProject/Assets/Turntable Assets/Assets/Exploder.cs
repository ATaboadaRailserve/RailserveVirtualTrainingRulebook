using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Exploder : MonoBehaviour {
    public bool rotator;
    public float cameraDistance;
    public float scale;
    public LayerMask layer;
    public explodable[] explodables;
    public Vector3 displacement;
    public bool sideways;

    public AudioClip explode_item;
    public AudioClip return_item;
    public AudioClip select_item;

    Transform[] explodableChildren;
    Vector3[] points;
    GameObject mainCamera;
    GameObject bw_camera;
    GameObject coloredCamera;
    public GameObject itemSelect;
    [HideInInspector]
    public bool exploded;
    //Used for grading section
    [HideInInspector]
    public bool viewed;

    Vector3 mainCameraPosition;
    Quaternion mainCameraRotation;

    float clickStartTime;
    float clickEndTime;

    GameObject ghost;
    Material ghostMaterial;

    int startCullingMask;
    // Use this for initialization
    void Start ()
    {
        //transform.tag = "Exploder";
        exploded = false;
        //DUPLICATE THE OBJECT FOR THE GHOST COPY
        ghost = (GameObject)Instantiate(gameObject, transform.position, transform.rotation);
        ghostMaterial = (Material)Resources.Load("GhostMaterial");
        ghost.name = name + "_Ghost";
        Destroy(ghost.GetComponent<Exploder>());
        /*for (int i = 0; i < ghost.transform.childCount; ++i)
        {
            if (ghost.transform.GetChild(i).gameObject.layer != gameObject.layer)
                Destroy(ghost.transform.GetChild(i).gameObject);
            else
            {
                if (ghost.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                    ghost.transform.GetChild(i).GetComponent<MeshRenderer>().material = ghostMaterial;
                if (ghost.transform.GetChild(i).childCount > 0)
                {
                    for (int j = 0; j < ghost.transform.GetChild(i).childCount; j++)
                    {
                        if (ghost.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>() != null)
                            ghost.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>().material = ghostMaterial;
                    }
                }
            }   
        }*/
        Destroy(ghost.GetComponent<BoxCollider>());
        //Go through all the children and change them to the ghost material
        ChangeToGhost(ghost);
        ghost.SetActive(false);

        points = PointsOnSphere(explodables.Length, displacement);
        GameObject cameras = GameObject.Find("CameraPivot");
        bw_camera = cameras.transform.GetChild(0).gameObject;
        coloredCamera = cameras.transform.GetChild(0).GetChild(0).gameObject;
        mainCamera = bw_camera.GetComponent<TurnTable>().mainCamera;
        bw_camera.GetComponent<Camera>().enabled = false;
        coloredCamera.GetComponent<Camera>().enabled = false;
        startCullingMask = bw_camera.GetComponent<Camera>().cullingMask;
        itemSelect.SetActive(false);

        //ADD "EXPLODABLE" SCRIPT TO THE EXPLODABLE ITEMS
        for(int i = 0; i < explodables.Length; i++)
        {
            Explodable nextExplodable = explodables[i].gameObject.AddComponent<Explodable>();
            nextExplodable.ignoreChildren = explodables[i].ignoreChildren;
            nextExplodable.childSeperation = explodables[i].childSeperation;
            nextExplodable.overrideZoom = explodables[i].overrideZoom;
            nextExplodable.overrideZoomAmount = explodables[i].overrideZoomAmount;
            LineRenderer nextLine = explodables[i].gameObject.AddComponent<LineRenderer>();
            nextExplodable.name = explodables[i].gameObject.name;
            nextExplodable.description = explodables[i].description;
            nextExplodable.exploder = gameObject;
            nextExplodable.tag = "Explodable";
        }
        /*for (int i = 0; i < explodables.Length; i++)
        {
            explodables[i].gameObject.GetComponent<Explodable>().Return();
        }*/
    }
	
    void ChangeToGhost(GameObject parent)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
            parent.GetComponent<MeshRenderer>().material = ghostMaterial;
        if (parent.transform.childCount > 0)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                ChangeToGhost(parent.transform.GetChild(i).gameObject);
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (exploded)
        {
            //mainCamera.GetComponent<Camera>().transform.position = coloredCamera.transform.position;
            //mainCamera.GetComponent<Camera>().transform.rotation = coloredCamera.transform.rotation;
        }
	}

    public void SelectItem(GameObject item)
    {
        Debug.Log((1f / item.GetComponent<Renderer>().bounds.extents.magnitude) * 5);
        bw_camera.GetComponent<TurnTable>().SelectItem(item);
        mainCamera.transform.parent.GetComponent<AudioSource>().PlayOneShot(select_item);
        //bw_camera.GetComponent<TurnTable>().ZoomIn(item);
    }

    public void Explode()
    {
        viewed = true;
        itemSelect.transform.FindChild("Scrollbar").GetComponent<Scrollbar>().value = 1;
        exploded = true;
        //mainCameraPosition = mainCamera.transform.position;
        //mainCameraRotation = mainCamera.transform.rotation;
        mainCamera.transform.parent.GetComponent<AudioSource>().PlayOneShot(explode_item);
        //mainCamera.GetComponent<Camera>().enabled = false;
        //mainCamera.GetComponent<Camera>().depth = 
        bw_camera.GetComponent<Camera>().enabled = true;
        coloredCamera.GetComponent<Camera>().enabled = true;
        //bw_camera.transform.position = mainCamera.GetComponent<Camera>().transform.position;
        //coloredCamera.transform.position = mainCamera.GetComponent<Camera>().transform.position;
        ghost.SetActive(true);

        for (int i = 0; i < explodables.Length; i++)
        {
            explodables[i].gameObject.GetComponent<Explodable>().Explode(points[i]);
        }
        //Make only the stuff specific to this object colored and everything else black and white
        coloredCamera.GetComponent<Camera>().cullingMask = layer;
        bw_camera.GetComponent<Camera>().cullingMask = startCullingMask - layer;

        bw_camera.GetComponent<TurnTable>().currentLayer = layer;
        //Disable all buttons
        Transform buttons = itemSelect.transform.FindChild("Buttons").GetChild(0);
        for (int i = 0; i < buttons.childCount; i++)
        {
            buttons.GetChild(i).gameObject.SetActive(false);
        }


        //Make item UI have correct info
        int buttonsused = 0;
        itemSelect.transform.FindChild("Name").GetChild(0).GetComponent<Text>().text = gameObject.name;
        for (int i = 0; i < explodables.Length; i++)
        {
            GameObject currentButton = buttons.GetChild(buttonsused).gameObject;
            currentButton.SetActive(true);
            currentButton.transform.GetChild(0).GetComponent<Text>().text = explodables[i].gameObject.name;
            currentButton.transform.GetComponent<CustomButton>().SetItem(explodables[i].gameObject);
            explodables[i].gameObject.GetComponent<Explodable>().my_button = currentButton;
            buttonsused++;
        }

        itemSelect.SetActive(true);
    }

    public void Return()
    {
        itemSelect.transform.FindChild("Scrollbar").GetComponent<Scrollbar>().value = 1;
        exploded = false;
        //mainCamera.transform.position = mainCameraPosition;
        //mainCamera.transform.rotation = mainCameraRotation;
        mainCamera.transform.parent.GetComponent<AudioSource>().PlayOneShot(return_item);
        //mainCamera.GetComponent<Camera>().enabled = true;
        ghost.SetActive(false);
        for (int i = 0; i < explodables.Length; i++)
        {
            explodables[i].gameObject.GetComponent<Explodable>().Return();
        }
        //Make everything colored again
        coloredCamera.GetComponent<Camera>().cullingMask = 0;
        bw_camera.GetComponent<Camera>().cullingMask = startCullingMask;
        itemSelect.SetActive(false);

        bw_camera.GetComponent<Camera>().enabled = false;
        coloredCamera.GetComponent<Camera>().enabled = false;
    }

    //Taken from http://forum.unity3d.com/threads/evenly-distributed-points-on-a-surface-of-a-sphere.26138/
    //Returns n points on a sphere that are evenly distributed across the surface
    Vector3[] PointsOnSphere(int n, Vector3 displacement)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            upts.Add(new Vector3((x * scale) + transform.position.x + displacement.x, (y * scale) + transform.position.y + displacement.y, (z * scale) + transform.position.z + displacement.z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
    }

    public void SelectObject(GameObject target)
    {
        for (int i = 0; i < explodableChildren.Length; i++)
        {
            explodableChildren[i].GetComponent<Explodable>().Explode(points[i]);
        }
        target.GetComponent<Explodable>().Return();
    }

    public void GhostOn()
    {
        ghost.SetActive(true);
    }

    public void GhostOff()
    {
        ghost.SetActive(false);
    }

    public Ray Raycast(Vector3 screenPosition)
    {
        Debug.Log("Raycasted");
        return coloredCamera.GetComponent<Camera>().ScreenPointToRay(screenPosition);
        
    }

    public void UpdateModelZoom(GameObject nextObject)
    {
        bw_camera.GetComponent<TurnTable>().ZoomIn(nextObject);
    }
}




[System.Serializable]

public struct explodable
{
    public GameObject gameObject;
    public string description;
    public bool overrideZoom;
    public float overrideZoomAmount;
    public bool ignoreChildren;
    public float childSeperation;
    
}
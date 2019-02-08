using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Explodable : MonoBehaviour {
    [HideInInspector]
    public string name;
    [HideInInspector]
    public string description;
    //[HideInInspector]
    public GameObject exploder;
    [HideInInspector]
    public GameObject my_button;
    [HideInInspector]
    public float childSeperation;
    [HideInInspector]
    public bool overrideZoom;
    [HideInInspector]
    public float overrideZoomAmount;
    public bool ignoreChildren;

    Vector3 startPos;
    public Vector3 targetPos;
    float speed;
    GameObject circle;
    GameObject ui_name;
    GameObject ui_description;

    Material[] ghostMaterial;

    bool exploded;
    LineRenderer line;
    Vector3 lineStart;
    Vector3 lineEnd;

    bool clickedOn;
    
    //Use this for initialization
	void Start ()
    {
        ghostMaterial = new Material[1] { (Material)Resources.Load("GhostMaterial") };
        if (transform.childCount>0 && !ignoreChildren)
        {
            SaveMaterials(transform);
            StartCoroutine("DisableExtraObjects");
        }
        else if(transform.childCount > 0)
        {
            SaveMaterials(transform);
        }
        
        startPos = transform.position;
        speed = 5;
        ui_name = GameObject.Find("TurnTableCamera").GetComponent<TurnTable>().ui_name;
        ui_description = GameObject.Find("TurnTableCamera").GetComponent<TurnTable>().ui_description;
        gameObject.tag = "Explodable";

        exploded = false;
        line = GetComponent<LineRenderer>();

        //line.SetWidth(0f, GetComponent<Renderer>().bounds.extents.magnitude);
        line.SetWidth(.05f, .05f);

        line.SetVertexCount(2);
        //line.SetColors(new Color(255, 221, 2, .5f), new Color(255, 221, 2, 0));
        line.SetColors(new Color(255, 221, 2, 1), new Color(255, 221, 2, 1));
        if (ignoreChildren)
		{
            if(GetComponent<Renderer>() != null)
				{
					lineStart = new Vector3(GetComponent<Renderer>().bounds.center.x, GetComponent<Renderer>().bounds.min.y - 0.1f, GetComponent<Renderer>().bounds.center.z);
				}
				else
				{
					lineStart = transform.position;
				}
		}
        else
            lineStart = GetComponent<Renderer>().bounds.center;
        line.material = (Material)Resources.Load("LineMaterial");

        
    }

    void SaveMaterials(Transform parent)
    {
        if (parent.childCount == 0)
            return;
        for (int i = 0; i < parent.childCount; i++)
        {
            MyMaterial nextMat = parent.GetChild(i).gameObject.AddComponent<MyMaterial>();
            nextMat.UpdateMaterial();
            if (parent.GetChild(i).childCount > 0)
            {
                SaveMaterials(parent.GetChild(i));
            }
        }
    }
    
    //Used because if you disable them immediately it messes up the materials due to how its instantiated at Start()
    IEnumerator DisableExtraObjects ()
    {
        yield return new WaitForSeconds(.5f);
        for(int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        UpdateModelPositions();
        if (transform.childCount > 1 && !ignoreChildren)
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (exploded)
        {
            if (ignoreChildren)
			{
                
				if(GetComponent<Renderer>() != null)
				{
					lineEnd = new Vector3(GetComponent<Renderer>().bounds.center.x, GetComponent<Renderer>().bounds.min.y - 0.1f, GetComponent<Renderer>().bounds.center.z);
				}
				else
				{
					lineEnd = transform.position;
				}
			}
            else
                lineEnd = new Vector3(GetComponent<Renderer>().bounds.center.x, GetComponent<Renderer>().bounds.min.y - 0.1f, GetComponent<Renderer>().bounds.center.z);
            line.SetPosition(0, lineStart);
            line.SetPosition(1, lineEnd);
        }
	}

    public void Explode(Vector3 target)
    {
        
        if (transform.childCount > 1 && !ignoreChildren)
        {
            for(int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        line.enabled = true;
        exploded = true;
        targetPos = target;
        StartCoroutine("ExplodeToTarget");
        
    }

    public void Return()
    {
        if (transform.childCount > 1 && !ignoreChildren)
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        exploded = false;
        StartCoroutine("ReturnToStart");
        line.enabled = false;
    }

    IEnumerator ExplodeToTarget()
    {
        StopCoroutine("ReturnToStart");
        while (Vector3.Distance(targetPos, transform.position) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        GetComponent<BoxCollider>().enabled = true;
        transform.position = targetPos;
    }

    IEnumerator ReturnToStart()
    {
        StopCoroutine("ExplodeToTarget");
        GetComponent<BoxCollider>().enabled = false;
        while (Vector3.Distance(startPos, transform.position) > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
        if (exploder != null)
        {
            
            exploder.GetComponent<Exploder>().GhostOff();
            //exploder = null;
        }
    }

    public void Select()
    {
        //my_button.GetComponent<CustomButton>().SelectItem();
        ui_name.transform.FindChild("Name").GetComponent<Text>().text = name;
        ui_description.transform.FindChild("Description").GetComponent<Text>().text = description;
        UpdateModelPositions();
    }

    public void NextModel()
    {
        transform.GetChild(0).SetAsLastSibling();
        UpdateModelPositions();
    }

    public void PrevModel()
    {
        transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
        UpdateModelPositions();
    }

    void GhostChildren(Transform parent)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
            parent.GetComponent<MeshRenderer>().materials = ghostMaterial;
        for (int i = 0; i < parent.childCount; i++)
        {
            GhostChildren(parent.GetChild(i));
        }
    }

    void UnGhostChildren(Transform parent)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
            parent.GetComponent<MeshRenderer>().materials = parent.GetComponent<MyMaterial>().materials;
        for (int i = 0; i < parent.childCount; i++)
        {
            UnGhostChildren(parent.GetChild(i));
        }
    }

    void UpdateModelPositions()
    {
        if (transform.childCount > 0 && !ignoreChildren)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform nextChild = transform.GetChild(i);
                switch (i)
                {
                    case 0:
                        nextChild.gameObject.SetActive(true);
                        nextChild.localPosition = new Vector3(0, 0, 0);
                        nextChild.localScale = new Vector3(1f, 1f, 1f);
                        nextChild.GetComponent<MeshRenderer>().materials = transform.GetChild(i).GetComponent<MyMaterial>().materials;
                        UnGhostChildren(nextChild);
                        break;
                    case 1:
                        nextChild.gameObject.SetActive(true);
                        //nextChild.localPosition = new Vector3(0, -2, 0);
                        nextChild.position = transform.GetChild(0).transform.position + new Vector3(0, -childSeperation, 0);
                        nextChild.localScale = new Vector3(.5f, .5f, .5f);
                        nextChild.GetComponent<MeshRenderer>().materials = ghostMaterial;
                        GhostChildren(nextChild);
                        break;
                    default:
                        if (i == transform.childCount - 1)
                        {
                            nextChild.gameObject.SetActive(true);
                            //nextChild.localPosition = new Vector3(0, 2, 0);
                            nextChild.position = transform.GetChild(0).transform.position + new Vector3(0, childSeperation, 0);
                            nextChild.localScale = new Vector3(.5f, .5f, .5f);
                            nextChild.GetComponent<MeshRenderer>().materials = ghostMaterial;
                            GhostChildren(nextChild);
                        }
                        else
                        {
                            nextChild.gameObject.SetActive(false);
                        }
                        break;
                }
            }
            exploder.GetComponent<Exploder>().UpdateModelZoom(transform.GetChild(0).gameObject);
        }
    }
}

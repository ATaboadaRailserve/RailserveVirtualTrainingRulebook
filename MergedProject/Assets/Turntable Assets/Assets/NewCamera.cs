using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewCamera : MonoBehaviour {
    public GameObject[] exploders;
    public GameObject turntable;
    public GameObject canvas;
    GameObject item_name;
    GameObject left_arrow;
    GameObject right_arrow;
    GameObject view_button;

    //Keep track of current object
    public int curExploder;
    //Keep track of "score"
    int explodersViewed;
    int totalExploders;
    static bool exploded;
    bool moving;

	void Start () {
        item_name = canvas.transform.Find("ItemName").Find("Name").gameObject;
        left_arrow = canvas.transform.Find("ItemName").Find("Left_Arrow").gameObject;
        right_arrow = canvas.transform.Find("ItemName").Find("Right_Arrow").gameObject;
        view_button = canvas.transform.Find("ViewButton").gameObject;
        explodersViewed = 0;
        totalExploders = exploders.Length;
        StartCoroutine("MoveTo", exploders[curExploder]);
    }

	void Update () {
        if (!exploded)
        {
            Show();
            transform.LookAt(transform.parent);
        }
	}

    public void NextExploder()
    {
        StopCoroutine("MoveTo");
        if (curExploder < totalExploders - 1)
            curExploder++;
        StartCoroutine("MoveTo", exploders[curExploder]);
    }

    public void PrevExploder()
    {
        StopCoroutine("MoveTo");
        if (curExploder > 0)
            curExploder--;
        StartCoroutine("MoveTo", exploders[curExploder]);
    }

    IEnumerator MoveTo(GameObject nextExploder)
    {
        moving = true;
        view_button.SetActive(false);
        Debug.Log("START MOVING");
        CorrectArrows();
        item_name.GetComponent<Text>().text = nextExploder.name;
        Vector3 newDisplacement = new Vector3(0,transform.localPosition.y,-nextExploder.GetComponent<Exploder>().cameraDistance);
        Vector3 targetPosition = nextExploder.transform.position;
        while(Vector3.Distance(transform.parent.position, targetPosition) > .01f || Vector3.Distance(transform.localPosition, newDisplacement) > .01f)
        {
            yield return new WaitForEndOfFrame();
            transform.parent.position = Vector3.Lerp(transform.parent.position, targetPosition, 3*Time.deltaTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, newDisplacement, 3*Time.deltaTime);
        }
        Debug.Log("DONE MOVING");
        DoneMoving();
        transform.parent.position = targetPosition;
        transform.localPosition = newDisplacement;
    }

    void DoneMoving()
    {
        moving = false;
        view_button.SetActive(true);
    }

    public void Hide()
    {
        canvas.GetComponent<Canvas>().enabled = false;
    }

    public void Show()
    {
        canvas.GetComponent<Canvas>().enabled = true;
    }

    //Enable or disable arrows as needed
    void CorrectArrows()
    {
        if (curExploder <= 0)
        {
            left_arrow.SetActive(false);
        }
        else
        {
            left_arrow.SetActive(true);
        }
        if(curExploder >= totalExploders-1)
        {
            right_arrow.SetActive(false);
        }
        else
        {
            right_arrow.SetActive(true);
        }
    }

    public void ViewExploder()
    {
        StopCoroutine("MoveTo");
        Hide();
        exploded = true;

        //Determine percentage viewed
        explodersViewed = 0;
        for(int i = 0; i < totalExploders; i++)
        {
            if (exploders[i].GetComponent<Exploder>().viewed)
                explodersViewed++;
        }

        //Explode object
        turntable.GetComponent<TurnTable>().ExplodeExploder(exploders[curExploder]);
    }

    public static void Return()
    {
        exploded = false;

    }
}

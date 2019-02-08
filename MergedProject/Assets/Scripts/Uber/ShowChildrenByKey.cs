using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowChildrenByKey : MonoBehaviour
{

    public KeyCode key = KeyCode.K;
    private bool isShown;
    List<GameObject> children;

    void Start()
    {
        children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            children.Add(child.gameObject);
        }
    }

    void Update()
    {
        if(isShown != Input.GetKey(key))
        {
            isShown = !isShown;
            foreach (GameObject child in children)
            {
                child.SetActive(isShown);
            }
        }
    }
}
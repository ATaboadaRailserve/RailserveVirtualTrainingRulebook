using UnityEngine;
using System.Collections;

public class MyMaterial : MonoBehaviour {
    public Material[] materials;
    MeshRenderer mr;

    // Use this for initialization
    void Start () {
        mr = GetComponent<MeshRenderer>();
        if(mr!= null)
            materials = GetComponent<MeshRenderer>().materials;
	}

    public void UpdateMaterial()
    {
        mr = GetComponent<MeshRenderer>();
        if (mr != null)
            materials = GetComponent<MeshRenderer>().materials;
    }
}

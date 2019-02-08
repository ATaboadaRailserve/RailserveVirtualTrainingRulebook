using UnityEngine;
using System.Collections;

public class AnimationTrans : MonoBehaviour
{
    public GameObject ModuleTransObject;

    public void ModuleTransition()
    {
        ModuleTransObject.SetActive(true);
      //  Debug.Log("PrintEvent");
    }
}
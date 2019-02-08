using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UberSubmoduleExample_1 : UberSubmodule {

    public GameObject[] randomEnableOne;

    public override void StartSubmodule()
    {
        base.StartSubmodule();
        foreach (GameObject g in randomEnableOne)
            g.SetActive(false);
        randomEnableOne[Random.Range(0, randomEnableOne.Length)].SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UberSubmoduleExample_2 : UberSubmodule {

    public RectTransform timerBar;
    public float maxTime = 12;
    private float t;

    public override void StartSubmodule()
    {
        base.StartSubmodule();
        t = maxTime;
    }

    void Update()
    {
        if(IsSubmoduleActive)
        {
            t -= Time.deltaTime;
            if(t < 0)
            {
                FailSubmodule();
            }
            else
            {
                Vector3 scale = Vector3.one;
                scale.x = t / maxTime;
                timerBar.localScale = scale;
            }
        }
    }
}

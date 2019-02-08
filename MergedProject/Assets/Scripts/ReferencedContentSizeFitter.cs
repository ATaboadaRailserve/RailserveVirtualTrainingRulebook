using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReferencedContentSizeFitter : MonoBehaviour {

    public RectTransform reference;
    public Vector2 padding = new Vector2(20, 12);
	
	void Update () {
		if(reference != null)
        {
            RectTransform self = GetComponent<RectTransform>();
            if(self != null)
            {
                self.sizeDelta = new Vector2((reference.sizeDelta.x * reference.localScale.x) + padding.x, (reference.sizeDelta.y * reference.localScale.y) + padding.y);
            }
        }
	}
}

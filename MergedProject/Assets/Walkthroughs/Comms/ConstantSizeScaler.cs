using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConstantSizeScaler : MonoBehaviour {

    public Transform headCamera;
    public Vector3 scaleAtTenMeters = Vector3.one;
    public Vector3 maxScale = Vector3.one * 2;
    public Vector3 minScale = Vector3.one / 2;
    public float minimizeTime = 0.5f;
    public bool startMinimized;
    public bool isLocked { get; set; }
    Vector3 desiredScale;
    Coroutine maximizing;
    Coroutine minimizing;
    bool isMinimized;
    bool isMaximized;

    void Start()
    {
        if (headCamera == null)
            this.enabled = false;
        if (startMinimized)
            transform.localScale = Vector3.zero;
        isMinimized = startMinimized;
        isMaximized = !isMinimized;
    }

	void Update () {
        float distance = Vector3.Distance(headCamera.position, transform.position);
        desiredScale = scaleAtTenMeters * (distance / 10.0f);
        if (desiredScale.magnitude > maxScale.magnitude)
            desiredScale = maxScale;
        else if (desiredScale.magnitude < minScale.magnitude)
            desiredScale = minScale;
        if(!isMinimized && maximizing == null && minimizing == null)
        {
            transform.localScale = desiredScale;
        }
	}

    public void Minimize()
    {
        if (isLocked || isMinimized)
            return;
        if (maximizing != null)
        {
            StopCoroutine(maximizing);
            maximizing = null;
        }
        if(minimizing == null)
            minimizing = StartCoroutine(DoMinimize());
    }

    public void Maximize()
    {
        if (isLocked || isMaximized)
            return;
        if (minimizing != null)
        {
            StopCoroutine(minimizing);
            minimizing = null;
        }
        if(maximizing == null)
            maximizing = StartCoroutine(DoMaximize());
    }

    IEnumerator DoMinimize()
    {
        isMaximized = false;
        Vector3 currentScale = transform.localScale;
        for(float t = 0.0f; t < minimizeTime; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Slerp(currentScale, Vector3.zero, t / minimizeTime);
            yield return null;
        }
        transform.localScale = Vector3.zero;
        isMinimized = true;
        minimizing = null;
    }

    IEnumerator DoMaximize()
    {
        isMinimized = false;
        Vector3 currentScale = transform.localScale;
        for (float t = 0.0f; t < minimizeTime; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Slerp(currentScale, desiredScale, t / minimizeTime);
            yield return null;
        }
        isMaximized = true;
        maximizing = null;
    }
}

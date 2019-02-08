using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtilityInterface : MonoBehaviour {

	public void MoveTo(Transform input)
    {
        transform.position = input.position;
        transform.rotation = input.rotation;
    }

    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public void SetLocalPosition(Vector3 localPosition)
    {
        transform.localPosition = localPosition;
    }

    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void SetRotation(Vector3 euler)
    {
        transform.rotation = Quaternion.Euler(euler);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UberCar : MonoBehaviour {

    public CarType carType;
    public UberMountPoint[] mountPoints;

	public enum CarType
    {
        tank,
        box,
        hopper
    }

    public void DisableMountPoints()
    {
        foreach (UberMountPoint p in mountPoints)
            p.gameObject.SetActive(false);
    }

    public void EnableMountPoints()
    {
        foreach (UberMountPoint p in mountPoints)
            p.gameObject.SetActive(true);
    }
}

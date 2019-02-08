using UnityEngine;
using System.Collections;

public class HitLoR : MonoBehaviour {
	public enum ColliderLocation{LeftSide, RightSide, Middle};
	public ColliderLocation collider_Location;
	private GameObject player;
	private GrabandRotate grab;
	private ObjectShaderMove osm;

	void Start()
	{
		grab = transform.parent.FindChild("Lever_1").FindChild("Lever_0").gameObject.GetComponent<GrabandRotate>(); //Finds the Lever GameObject
		osm = gameObject.GetComponent<ObjectShaderMove>();
	}
	void OnTriggerStay(Collider other)
	{
			if (collider_Location == ColliderLocation.LeftSide) {
				grab.playerLocation = 1;
			osm.ChangeColor (1);
			} else if (collider_Location == ColliderLocation.RightSide) {
				grab.playerLocation = 2;
			osm.ChangeColor (1);
			} else if (collider_Location == ColliderLocation.Middle) {
				grab.playerLocation = 3;
			}
	}
	void OnTriggerExit(Collider other)
	{
		grab.playerLocation = 0;
		if (collider_Location != ColliderLocation.Middle) {
			osm.ChangeColor (0);
		}
	}
}

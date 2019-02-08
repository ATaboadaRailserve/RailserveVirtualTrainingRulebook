using UnityEngine;
using System.Collections;

public class WorldSpaceTexture : MonoBehaviour {
	
	public int materialIndex = 0;
	
	void Start () {
		GetComponent<Renderer>().materials[materialIndex].SetTextureOffset("_MainTex", new Vector2(transform.position.x, transform.position.z));
	}
}

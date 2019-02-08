using UnityEngine;
using System.Collections;

public class ObjectShaderMove : MonoBehaviour {

	Renderer mytexture;
	public float speed;
	public Color[] m_StripeColors;
	private float visible = 0;
	private GrabandRotate grab;
	private bool justchangedcolor = false;
	public Vector3 upPos,downPos;
	// Use this for initialization
	void Start () {
		mytexture = gameObject.GetComponent<Renderer> ();
		MeshRenderer rd = gameObject.GetComponent<MeshRenderer> ();
		rd.material.SetColor ("_StripeColor", m_StripeColors [0]);
		grab = this.transform.parent.FindChild("Lever_1").FindChild("Lever_0").GetComponent<GrabandRotate>();
	}

	// Update is called once per frame
	void Update () {
		if (!grab.inSwitchRange) {
			transform.localPosition = downPos;
			//print ("down");
			justchangedcolor = false;
		} else if (grab.inSwitchRange && !justchangedcolor) {
			transform.localPosition = upPos;
			//print ("up");
			justchangedcolor = true;
		}
	}

	public void ChangeColor(int i)
	{
		MeshRenderer rd = gameObject.GetComponent<MeshRenderer> ();
		rd.material.SetColor ("_StripeColor", m_StripeColors [i]);
	}
}
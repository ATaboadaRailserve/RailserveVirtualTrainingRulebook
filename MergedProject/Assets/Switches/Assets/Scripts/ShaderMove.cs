using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShaderMove : MonoBehaviour {

	Renderer mytexture;
	public float speed;
	public List<ChangeNumberOverTime> ChangeNumberList = new List<ChangeNumberOverTime> ();
	public AnimationScrubber scrubber;
	public Color[] m_RimColors;
	public float m_ClipVarianceMin = 100.0f;
	public float m_ClipVarianceMax = 200.0f;
	private float visible = 0;
	private bool working = false;
	public float timeflicker;
	// Use this for initialization
	void Start () {
		mytexture = gameObject.GetComponent<Renderer> ();

		foreach (ChangeNumberOverTime p in ChangeNumberList) {
			p.deltaT = p.timeFrame.y - p.timeFrame.x;
		}
			
			SkinnedMeshRenderer rd = gameObject.GetComponent<SkinnedMeshRenderer> ();
			rd.material.SetColor ("_RimColor", m_RimColors [0]);

	}
	
	// Update is called once per frame
	void Update () {
		float scrollspeed = speed * Time.time;
		mytexture.material.mainTextureOffset = new Vector2 (scrollspeed, -scrollspeed);

			float f = scrubber.GetTime ();
			bool doingStuff = false;

			for (int i = 0; i < ChangeNumberList.Count; i++) {
				if (f > ChangeNumberList [i].timeFrame.x && f < ChangeNumberList [i].timeFrame.y) {
					float tfloat = ((f - ChangeNumberList [i].timeFrame.x) / ChangeNumberList [i].deltaT);
					visible = Mathf.Lerp (ChangeNumberList [i].startNumber, ChangeNumberList [i].endNumber, tfloat);
					mytexture.material.SetFloat ("_HologramClip", visible);
					doingStuff = true;
				}
			}
			if (!doingStuff) {
				int cl = -1;
				for (int i = 0; i < ChangeNumberList.Count; i++) {
					if (f > ChangeNumberList [i].timeFrame.y) {
						cl = i;
					}
				}
				if (cl == -1) {
					mytexture.material.SetFloat ("_HologramClip", ChangeNumberList [0].startNumber);
					visible = ChangeNumberList [0].startNumber;
				} else {
					mytexture.material.SetFloat ("_HologramClip", ChangeNumberList [cl].endNumber);
					visible = ChangeNumberList [cl].endNumber;
				}
			}
			if (!working && visible > 0) {
				StartCoroutine (Tick ());
				working = true;
				//print ("Start");
			}

	}

	IEnumerator Tick ()
	{
		while (visible > 0)
		{
			UpdateCustom ();
			yield return new WaitForSeconds (timeflicker);
		}
		//print ("end");
		working = false;
	}
	void UpdateCustom ()
	{
		// hologram flicker
		float newclip = Random.Range (m_ClipVarianceMin, m_ClipVarianceMax);	
		int newRim = Random.Range (0, m_RimColors.Length - 1);
		for (int i = 0; i < m_RimColors.Length; i++)
		{
			//MeshRenderer rd = m_Objects[i].GetComponent<MeshRenderer>();
				SkinnedMeshRenderer rd = gameObject.GetComponent<SkinnedMeshRenderer> ();
				rd.material.SetFloat ("_HologramClip", newclip);
				rd.material.SetColor ("_RimColor", m_RimColors [newRim]);
		}
	}
}

[System.Serializable]
public class ChangeNumberOverTime
{
	public Vector2 timeFrame;
	public float startNumber, endNumber;
	[HideInInspector]
	public float deltaT;
}

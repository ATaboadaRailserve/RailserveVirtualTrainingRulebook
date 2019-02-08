using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoHologram : MonoBehaviour
{
	public GameObject[] m_Objects;
	public Color[] m_RimColors;
	public float m_ClipVarianceMin = 100.0f;
	public float m_ClipVarianceMax = 200.0f;
	private int m_Type = 1;
	
    void Start ()
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			//MeshRenderer rd = m_Objects[i].GetComponent<MeshRenderer>();
			SkinnedMeshRenderer rd = m_Objects[i].GetComponent<SkinnedMeshRenderer>();
			rd.material.SetColor ("_RimColor", m_RimColors[i]);
		}
		StartCoroutine (Tick ());
	}
	IEnumerator Tick ()
	{
		while (true)
		{
			UpdateCustom ();
			yield return new WaitForSeconds (0.08f);
		}
	}
	void UpdateCustom ()
    {
		// hologram flicker
		float newclip = Random.Range (m_ClipVarianceMin, m_ClipVarianceMax);		
		for (int i = 0; i < m_Objects.Length; i++)
		{
			//MeshRenderer rd = m_Objects[i].GetComponent<MeshRenderer>();
			SkinnedMeshRenderer rd = m_Objects[i].GetComponent<SkinnedMeshRenderer>();
			rd.material.SetFloat ("_HologramClip", newclip);
		}
    }
	void OnGUI ()
	{
		GUI.Box (new Rect (10, 10, 180, 25), "Amazing Stripe Effect Demo");
		string[] names = { "Hologram X", "Hologram Y" };
		m_Type = GUI.SelectionGrid (new Rect (10, 40, 180, 25), m_Type, names, 2);
		Apply ();
	}
	void Apply ()
	{
		if (0 == m_Type)
		{
			ApplyShaderMacro ("STRIPE_HOLOGRAM_X", "");
			m_ClipVarianceMin = 420;
			m_ClipVarianceMax = 425;
		}
		if (1 == m_Type)
		{
			ApplyShaderMacro ("", "STRIPE_HOLOGRAM_X");
			m_ClipVarianceMin = 180;
			m_ClipVarianceMax = 185;
		}
	}
	void ApplyShaderMacro (string on, string off)
	{
		for (int i = 0; i < m_Objects.Length; i++)
		{
			//MeshRenderer rd = m_Objects[i].GetComponent<MeshRenderer>();
			SkinnedMeshRenderer rd = m_Objects[i].GetComponent<SkinnedMeshRenderer>();
			rd.material.EnableKeyword (on);
			rd.material.DisableKeyword (off);
		}
	}
}

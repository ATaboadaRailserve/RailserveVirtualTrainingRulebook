using UnityEngine;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
	public GameObject[] m_StripeObjects;
	public GameObject m_StripeCage;
	private int m_StripeType = 0;
	private float m_StripeWidth = 0.1f;
	private float m_StripeDensity = 16f;
	private float m_StripeMoveSpeedX = 0f;
	private float m_StripeMoveSpeedY = 0f;
	private float m_StripeMoveSpeedZ = 0f;
	private Rect[] m_GUIRects = new Rect[5];
	
	public bool MouseOnGUI ()
	{
		for (int i = 0; i < m_GUIRects.Length; i++)
		{
			if (m_GUIRects[i].Contains (new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
				return true;
		}
		return false;
	}
    void Start ()
	{
		m_GUIRects[0] = new Rect (100, 140, 100, 25);
		m_GUIRects[1] = new Rect (100, 170, 100, 25);
		m_GUIRects[2] = new Rect (10, 220, 60, 25);
		m_GUIRects[3] = new Rect (70, 220, 60, 25);
		m_GUIRects[4] = new Rect (130, 220, 60, 25);
		
		Shader.DisableKeyword ("STRIPE_X");
		MeshRenderer rd = m_StripeCage.GetComponent<MeshRenderer>();
		rd.material.EnableKeyword ("STRIPE_XYZ");
		rd.material.DisableKeyword ("STRIPE_X");
	}
	void Update ()
    {
		for (int i = 0; i < m_StripeObjects.Length; i++)
		{
			MeshRenderer rd = m_StripeObjects[i].GetComponent<MeshRenderer>();
			rd.material.SetFloat ("_StripeWidth", m_StripeWidth);
			rd.material.SetFloat ("_StripeDensity", m_StripeDensity);
			rd.material.SetVector ("_StripeMove", new Vector4 (m_StripeMoveSpeedX, m_StripeMoveSpeedY, m_StripeMoveSpeedZ, 0f));
		}
    }
	void OnGUI()
	{
		GUI.Box (new Rect (10, 10, 180, 26), "Amazing Stripe Effect Demo");
		string[] names = { "X", "Y", "Z", "XY", "XZ", "YZ", "XYZ" };
		m_StripeType = GUI.SelectionGrid (new Rect (10, 40, 180, 80), m_StripeType, names, 3);
		
		GUI.Box (new Rect (10, 130, 80, 25), "Width");
		m_StripeWidth = GUI.HorizontalSlider (m_GUIRects[0], m_StripeWidth, 0.01f, 0.8f);
		GUI.Box (new Rect (10, 160, 80, 25), "Density");
		m_StripeDensity = GUI.HorizontalSlider (m_GUIRects[1], m_StripeDensity, 0.5f, 20f);
		GUI.Box (new Rect (10, 190, 180, 25), "Move Speed");
		m_StripeMoveSpeedX = GUI.HorizontalSlider (m_GUIRects[2], m_StripeMoveSpeedX, -5f, 5f);
		m_StripeMoveSpeedY = GUI.HorizontalSlider (m_GUIRects[3], m_StripeMoveSpeedY, -5f, 5f);
		m_StripeMoveSpeedZ = GUI.HorizontalSlider (m_GUIRects[4], m_StripeMoveSpeedZ, -5f, 5f);
		if (GUI.Button (new Rect (10, 250, 80, 25), "Reset"))
			m_StripeMoveSpeedX = m_StripeMoveSpeedY = m_StripeMoveSpeedZ = 0f;
		
		Apply ();
	}
	void Apply ()
	{
		if (0 == m_StripeType)
		{
			string[] offs = { "STRIPE_Y", "STRIPE_Z", "STRIPE_XY", "STRIPE_XZ", "STRIPE_YZ", "STRIPE_XYZ" };
			ApplyShaderMacro ("STRIPE_X", offs);
		}
		if (1 == m_StripeType)
		{
			string[] offs = { "STRIPE_X", "STRIPE_Z", "STRIPE_XY", "STRIPE_XZ", "STRIPE_YZ", "STRIPE_XYZ" };
			ApplyShaderMacro ("STRIPE_Y", offs);
		}
		if (2 == m_StripeType)
		{
			string[] offs = { "STRIPE_X", "STRIPE_Y", "STRIPE_XY", "STRIPE_XZ", "STRIPE_YZ", "STRIPE_XYZ" };
			ApplyShaderMacro ("STRIPE_Z", offs);
		}
		if (3 == m_StripeType)
		{
			string[] offs = { "STRIPE_X", "STRIPE_Y", "STRIPE_Z", "STRIPE_XZ", "STRIPE_YZ", "STRIPE_XYZ" };
			ApplyShaderMacro ("STRIPE_XY", offs);
		}
		if (4 == m_StripeType)
		{
			string[] offs = { "STRIPE_X", "STRIPE_Y", "STRIPE_Z", "STRIPE_XY", "STRIPE_YZ", "STRIPE_XYZ" };
			ApplyShaderMacro ("STRIPE_XZ", offs);
		}
		if (5 == m_StripeType)
		{
			string[] offs = { "STRIPE_X", "STRIPE_Y", "STRIPE_Z", "STRIPE_XY", "STRIPE_XZ", "STRIPE_XYZ" };
			ApplyShaderMacro ("STRIPE_YZ", offs);
		}
		if (6 == m_StripeType)
		{
			string[] offs = { "STRIPE_X", "STRIPE_Y", "STRIPE_Z", "STRIPE_XY", "STRIPE_XZ", "STRIPE_YZ" };
			ApplyShaderMacro ("STRIPE_XYZ", offs);
		}
	}
	void ApplyShaderMacro (string on, string[] off)
	{
		for (int i = 0; i < m_StripeObjects.Length; i++)
		{
			MeshRenderer rd = m_StripeObjects[i].GetComponent<MeshRenderer>();
			rd.material.EnableKeyword (on);
			for (int j = 0; j < off.Length; j++)
				rd.material.DisableKeyword (off[j]);
		}
	}
}

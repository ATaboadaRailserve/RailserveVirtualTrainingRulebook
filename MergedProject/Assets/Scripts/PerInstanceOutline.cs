using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerInstanceOutline : MonoBehaviour {
	
	public static Color defaultOutlineColor = new Color(206f/255f,201f/255f,23f/255f, 1.0f);
	static Shader outlineShader;
	
	public bool isOutlined = false;
	public bool scaleToWorldSpace = true;
	[Range(0.00f, 0.10f)]
	public float outlineWidth = 0.011f;
	public bool useDefaultOutlineColor = true;
	public Color outlineColor = defaultOutlineColor;
	
	private bool lastIsOutlined;
	private float lastOutlineWidth;
	private bool lastuseDefaultOutlineColor;
	private Color lastOutlineColor;
	
	private Renderer rend;
	private Material outline;
	
	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		if(!rend)
		{
			this.enabled = false;
			Debug.LogError("Missing Renderer Component");
			return;
		}
		
		if(outlineShader == null)
		{
			outlineShader = Shader.Find("Custom/Outline_Only");
		}
		
		outline = new Material(outlineShader);
		
		Material[] newMats = new Material[rend.materials.Length + 1];
		for (int i = 0; i < rend.materials.Length; i++)
		{
			Debug.Log(rend.materials[i].name);
			newMats[i] = rend.materials[i];
		}
		newMats[rend.materials.Length] = outline;
		rend.materials = newMats;
		
		lastIsOutlined = isOutlined;
		lastOutlineWidth = outlineWidth;
		lastuseDefaultOutlineColor = useDefaultOutlineColor;
		lastOutlineColor = outlineColor;
		
		UpdateOutline();
	}
	
	// Update is called once per frame
	void Update () {
		if(lastIsOutlined != isOutlined ||
		lastOutlineWidth != outlineWidth ||
		lastuseDefaultOutlineColor != useDefaultOutlineColor ||
		lastOutlineColor != outlineColor)
		{
			UpdateOutline();
			
			lastIsOutlined = isOutlined;
			lastOutlineWidth = outlineWidth;
			lastuseDefaultOutlineColor = useDefaultOutlineColor;
			lastOutlineColor = outlineColor;
		}
	}
	
	void UpdateOutline()
	{
		Color currentColor = outlineColor;
		float currentWidth = outlineWidth;
		if(!isOutlined || outlineWidth <= 0.01f)
			currentWidth = 0.0f;
		else if(useDefaultOutlineColor)
			currentColor = defaultOutlineColor;
		
		if(scaleToWorldSpace)
		{
			float maxScale = transform.lossyScale.x;
			maxScale = Mathf.Max(maxScale, transform.lossyScale.y);
			maxScale = Mathf.Max(maxScale, transform.lossyScale.z);
			currentWidth = currentWidth / maxScale;
			//print(currentWidth.ToString("0.0000"));
		}
		
		outline.SetColor("_OutlineColor", currentColor);
		outline.SetFloat("_OutlineWidth", currentWidth);
	}
	
	public void SetOutline(bool isOn)
	{
		isOutlined = isOn;
	}
}

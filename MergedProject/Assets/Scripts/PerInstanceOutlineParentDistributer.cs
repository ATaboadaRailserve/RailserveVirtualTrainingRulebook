using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerInstanceOutlineParentDistributer : MonoBehaviour {

	public bool usePreassignedOnly = false;
	public bool isOutlined = false;
	public bool scaleToWorldSpace = true;
	[Range(0.00f, 0.10f)]
	public float outlineWidth = 0.011f;
	public bool useDefaultOutlineColor = true;
	public Color outlineColor = PerInstanceOutline.defaultOutlineColor;
	public Renderer[] preassignedRenderers;
	
	private PerInstanceOutline[] outlines;
	
	void Start()
	{
		List<PerInstanceOutline> outlineList = new List<PerInstanceOutline>();
		
		if(GetComponent<Renderer>())
			outlineList.Add(gameObject.AddComponent(typeof(PerInstanceOutline)) as PerInstanceOutline);
		
		foreach(Renderer r in preassignedRenderers)
			outlineList.Add(r.gameObject.AddComponent(typeof(PerInstanceOutline)) as PerInstanceOutline);
			
		if(!usePreassignedOnly)
		{
			Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
			foreach(Renderer r in childRenderers)
				outlineList.Add(r.gameObject.AddComponent(typeof(PerInstanceOutline)) as PerInstanceOutline);
		}
		
		outlines = outlineList.ToArray();
	}
	
	void Update()
	{
		foreach(PerInstanceOutline o in outlines)
			CopyOutlineData(o);
	}
	
	void CopyOutlineData(PerInstanceOutline outline)
	{
		outline.isOutlined = isOutlined;
		outline.outlineWidth = outlineWidth;
		outline.useDefaultOutlineColor = useDefaultOutlineColor;
		outline.outlineColor = outlineColor;
	}
	
	public void SetOutline(bool isOn)
	{
		isOutlined = isOn;
	}
}

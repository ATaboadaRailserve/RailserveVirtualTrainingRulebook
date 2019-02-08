using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Global
{
	public static List<GameObject> VolumetricObjList = new List<GameObject>();
}

public class VolumetricFogX : MonoBehaviour 
{
	public Vector3 Position;
	public Vector3 Rotation;
	public float   Radius = 0.5f;
	public float   Height = 30.0f;
	public float   NoiseScale = 0.24f;
	public float   FogDensity = 0.105f;
	public Color   FogColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public float   WindSpeed = 0.079f;
	public Vector2 WindDirection = new Vector2(0.707f, 0.707f);
	public GameObject[] PointLights;
	public GameObject[] SpotLights;

	public float fadeSpeed;//Added by Andrew

	private Material fogFilterMat;
	private Camera BackFaceCamera;
	private Camera SceneDepthCamera;
	private Camera FrontFaceCamera;
	private RenderTexture frontfaceDepthRT;
	private RenderTexture fogFilterRT;
	private RenderTexture sceneDepthRT;
	private Shader sceneDepthShader;
	private GameObject volumeFogObj;
	private Material frontfaceMaterial;
	private Material backfaceMaterial;
	private GameObject fogGeometry;
	private Texture2D fogGeometryTex;
	private Texture2D fogNoiseTex;
	private GameObject temp;
	private Vector3 vScale;
	private Vector3 vPosition;
	private GameObject[] VolumetricObjsF; 
	private GameObject[] VolumetricObjsB;
	private int    iVoidFogIndex;
	private Vector4 vParameters;
	private Vector3 vWindParameters;
	private int VolumetricObjsFLayer;
	private int VolumetricObjsBLayer;
	private int MainCameraLayer;
	private Vector4 _FogColor;
	private Vector4[] vLightParameter1;
	private Vector4[] vLightParameter2;
	private Vector4[] vLightParameter3;
	private Vector4[] vLightParameter4;
	private Vector4[] vLightParameter5;
	private Texture3D tex3D;
	private Color[] color;
	private int depth;
	private Camera volumetricFogCamera;
	// Use this for initialization
	void Start () 
	{
		volumetricFogCamera = this.GetComponent<Camera> ();
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("VolumetricFogArea");
		VolumetricObjsF = new GameObject[objs.Length + 1];
		VolumetricObjsB = new GameObject[objs.Length + 1];
		for (int i = 0; i < objs.Length; i++) 
		{
			GameObject objF = objs [i];
			GameObject objB = GameObject.Instantiate(objs [i]);
			objF.layer = LayerMask.NameToLayer("VolumetricFogF");
			objF.hideFlags = HideFlags.HideInHierarchy;
			//objF.name = "VolumetricFog Area";
			VolumetricObjsF[i] = objF;

			objB.layer = LayerMask.NameToLayer("VolumetricFogB");
			//objB.name = "VolumetricFog Area";
			VolumetricObjsB[i] = objB;
		}

		iVoidFogIndex = objs.Length;
		GameObject fogObj = Resources.Load ("Models/VolumeArea") as GameObject;

		GameObject VolumeFogAreaF = GameObject.Instantiate (fogObj);
		//VolumeFogAreaF.transform.localScale = new Vector3 (500, 50, 500);
		VolumeFogAreaF.layer = LayerMask.NameToLayer("VolumetricFogF");
		VolumeFogAreaF.hideFlags = HideFlags.HideInHierarchy;
		VolumeFogAreaF.name = "Void Fog";
		VolumetricObjsF[iVoidFogIndex] = VolumeFogAreaF;

		GameObject VolumeFogAreaB = GameObject.Instantiate (fogObj);
		//VolumeFogAreaB.transform.localScale = new Vector3 (500, 50, 500);
		VolumeFogAreaB.layer = LayerMask.NameToLayer("VolumetricFogB");
		VolumeFogAreaB.hideFlags = HideFlags.HideInHierarchy;
		VolumeFogAreaB.name = "Void Fog";
		VolumetricObjsB[iVoidFogIndex] = VolumeFogAreaB;


		volumetricFogCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("VolumetricFogF"));
		volumetricFogCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("VolumetricFogB"));
	
		GameObject BackFaceCameraObj = new GameObject ("BackFaceCamera");
		BackFaceCameraObj.AddComponent<Camera> ();
		BackFaceCameraObj.transform.parent = this.transform;
		BackFaceCameraObj.hideFlags = HideFlags.HideInHierarchy;
		BackFaceCamera = BackFaceCameraObj.GetComponent<Camera> ();

		BackFaceCamera.CopyFrom (volumetricFogCamera);
		BackFaceCamera.clearFlags = CameraClearFlags.SolidColor;
		BackFaceCamera.backgroundColor = new Color (0.0f, 0.0f, 0.0f, 0.0f);
		BackFaceCamera.cullingMask = 0;
		BackFaceCamera.cullingMask = (1 << LayerMask.NameToLayer("VolumetricFogB"));
		//BackFaceCamera.depth = -2;
		BackFaceCamera.depth = volumetricFogCamera.depth;
		BackFaceCamera.depthTextureMode = DepthTextureMode.None;
		fogFilterRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGBFloat/*ARGB32*/);
		BackFaceCamera.targetTexture = fogFilterRT;


		GameObject depthObj = new GameObject ("SceneDepthCamera");
		depthObj.AddComponent<Camera> ();
		depthObj.transform.parent = this.transform;
		depthObj.hideFlags = HideFlags.HideInHierarchy;
		SceneDepthCamera = depthObj.GetComponent<Camera> ();

		SceneDepthCamera.CopyFrom (volumetricFogCamera);
		SceneDepthCamera.clearFlags = CameraClearFlags.SolidColor;
		SceneDepthCamera.backgroundColor = new Color (5000.0f, 0.0f, 0.0f, 1.0f);
		SceneDepthCamera.depthTextureMode = DepthTextureMode.None;
		SceneDepthCamera.cullingMask = 0;
		sceneDepthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.RGHalf);
		SceneDepthCamera.targetTexture = sceneDepthRT;
		sceneDepthShader = Shader.Find("Custom/SceneDepth");
		SceneDepthCamera.SetReplacementShader(sceneDepthShader, "");


		GameObject frontfaceDepthObj = new GameObject ("FrontFaceCamera");
		frontfaceDepthObj.AddComponent<Camera> ();
		frontfaceDepthObj.transform.parent = this.transform;
		frontfaceDepthObj.hideFlags = HideFlags.HideInHierarchy;
		FrontFaceCamera = frontfaceDepthObj.GetComponent<Camera> ();

		FrontFaceCamera.CopyFrom (volumetricFogCamera);
		FrontFaceCamera.clearFlags = CameraClearFlags.SolidColor;
		FrontFaceCamera.backgroundColor = new Color (0.0f, 0.0f, 0.0f, 0.0f);
		FrontFaceCamera.cullingMask = (1 << LayerMask.NameToLayer("VolumetricFogF"));
		FrontFaceCamera.cullingMask = 0;
		//FrontFaceCamera.depth = -3;
		FrontFaceCamera.depth = volumetricFogCamera.depth;
		FrontFaceCamera.depthTextureMode = DepthTextureMode.None;
		frontfaceDepthRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.RGHalf);
		FrontFaceCamera.targetTexture = frontfaceDepthRT;


		fogGeometryTex = Resources.Load("Textures/Fog/fog7") as Texture2D;

		depth = 64;
		tex3D = new Texture3D(fogGeometryTex.height, fogGeometryTex.width, depth, TextureFormat.ARGB32, false);
		color = new Color[fogGeometryTex.height * fogGeometryTex.width * depth];
	
		// reseting the texture
		for(int i = 0; i < fogGeometryTex.height; i++)
		{
			for(int j = 0; j < fogGeometryTex.width; j++)
			{
				for(int k = 0; k < depth; k++)
				{
					color[k * fogGeometryTex.width * fogGeometryTex.width + i * fogGeometryTex.width + j] = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				}
			}
		}
		Vector3 color1 = new Vector3(1.0f, 1.0f, 1.0f);
		for(int i = 0; i < fogGeometryTex.height; i++)
		{
			for(int j = 0; j < fogGeometryTex.width; j++)
			{
				float alpha = fogGeometryTex.GetPixel(i, j).a;
				float y = fogGeometryTex.GetPixel(i, j).grayscale;
				int y1 = (int)(y * depth / 2.0f);
				int depth1 = depth/2 - y1;
				int depth2 = depth/2 + y1;

				for(int k = depth1; k < depth2; k++)
				{
					float colorFactor = Mathf.Abs((float)(k - depth/2.0f))/ y1;
					color1 = color1 * colorFactor;
					color[i * fogGeometryTex.width * fogGeometryTex.width + k * fogGeometryTex.width + j] = new Color(1.0f, 1.0f, 1.0f, alpha);
				}

			}
		}


		tex3D.SetPixels(color);
		tex3D.Apply();

		backfaceMaterial = new Material (Shader.Find ("Custom/VolumeBox"));
		frontfaceMaterial = new Material (Shader.Find ("Custom/FrontfaceDepth"));

		fogNoiseTex = Resources.Load("Textures/Fog/Noise3") as Texture2D;

		backfaceMaterial.SetTexture ("_VolumeTex", tex3D);
		backfaceMaterial.SetTexture ("_NoiseTex", fogNoiseTex);

		foreach (GameObject obj in VolumetricObjsF) 
		{
			obj.GetComponent<MeshRenderer> ().material = frontfaceMaterial;
		}
		foreach (GameObject obj in VolumetricObjsB) 
		{
			obj.GetComponent<MeshRenderer> ().material = backfaceMaterial;
		}

		vScale = new Vector3(0.0f, 0.0f, 0.0f);
		vPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		vParameters = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
		vWindParameters = new Vector3(0.0f, 0.0f, 0.0f);

		VolumetricObjsFLayer = (1 << LayerMask.NameToLayer("VolumetricFogF"));
		VolumetricObjsBLayer = (1 << LayerMask.NameToLayer("VolumetricFogB"));
		MainCameraLayer = volumetricFogCamera.cullingMask;
		_FogColor = new Vector4 ();

		vLightParameter1 = new Vector4[6];
		vLightParameter2 = new Vector4[6];
		for (int i = 0; i < 6; i++) 
		{
			vLightParameter1 [i] = new Vector4 ();
			vLightParameter2 [i] = new Vector4 ();	
		}
		vLightParameter3 = new Vector4[6];
		vLightParameter4 = new Vector4[6];
		vLightParameter5 = new Vector4[6];
		for (int i = 0; i < 6; i++) 
		{
			vLightParameter3 [i] = new Vector4 ();
			vLightParameter4 [i] = new Vector4 ();	
			vLightParameter5 [i] = new Vector4 ();
		}

	}
	//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv-Added by Andrew-vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
	public void setDensity(float density) //Added by Andrew
	{
		StopAllCoroutines ();
		//print ("Begin " + density);
		StartCoroutine (ChangeDensity (density));
	}

	private IEnumerator ChangeDensity(float density) //Added by Andrew
	{
		//print ("Starting ChangeDensity to " + density + " from " + FogDensity);
		if (FogDensity > density) {
			float tempD = FogDensity;
			while (FogDensity > density) {
				FogDensity -= Time.deltaTime/fadeSpeed * tempD;
				//print (FogDensity);
				yield return null;
			}
		} else if (FogDensity < density) {
			while (FogDensity < density) {
				FogDensity += Time.deltaTime/fadeSpeed * density;
				//print (FogDensity);
				yield return null;
			}
		}
		FogDensity = density;
	}
	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^-Added by Andrew-^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
	void Awake ()
	{
		fogFilterMat = new Material (Shader.Find ("Custom/VolumeFog"));
		temp = new GameObject ("Temp");
		temp.hideFlags = HideFlags.HideInHierarchy;
	}

	// Update is called once per frame
	void UpdateOnRenderImage () 
	{
		Matrix4x4 view = volumetricFogCamera.worldToCameraMatrix;
		int pointLightCount = 0;
		for (int i = 0; (i < PointLights.Length && i < 6); i++) 
		{
			vLightParameter1 [i].Set (0.0f, 0.0f, 0.0f, 0.0f);
			vLightParameter2 [i].Set (0.0f, 0.0f, 0.0f, 0.0f);

			if (PointLights [i] != null) 
			{
				Light light = PointLights [i].GetComponent<Light>();
				vLightParameter1 [pointLightCount].x = light.transform.position.x;
				vLightParameter1 [pointLightCount].y = light.transform.position.y;
				vLightParameter1 [pointLightCount].z = light.transform.position.z;
				vLightParameter1 [pointLightCount].w = light.range / 3;

				vLightParameter2 [pointLightCount].x = light.color.r;
				vLightParameter2 [pointLightCount].y = light.color.g;
				vLightParameter2 [pointLightCount].z = light.color.b;
				vLightParameter2 [pointLightCount].w = light.intensity;
				pointLightCount++;
			}
		}

		GameObject g = VolumetricObjsB [iVoidFogIndex];

		//Vector3 camPos = volumetricFogCamera.transform.position;
		//camPos.y -= 5;//Position.y;
		g.transform.position = Position;
		float r = volumetricFogCamera.farClipPlane * Radius;
		g.transform.localScale = new Vector3 (r, Height, r);

		vParameters.x = NoiseScale * r ;
		vParameters.y = Time.time;
		vParameters.z = FogDensity;
		WindDirection.Normalize ();
		vWindParameters.Set (WindDirection.x, WindDirection.y, WindSpeed);

		_FogColor.Set (FogColor.r, FogColor.g, FogColor.b, 0.0f);

		GameObject objB = VolumetricObjsB[0];
		GameObject objF = VolumetricObjsF[0];

		objF.transform.position = objB.transform.position;
		objF.transform.rotation = objB.transform.rotation;
		objF.transform.localScale = objB.transform.localScale;

		Matrix4x4 proj = volumetricFogCamera.projectionMatrix;
		objF.GetComponent<Renderer> ().material.SetMatrix("_InvProj", proj.inverse);

		objB.GetComponent<Renderer> ().material.SetMatrix("_InvProj", proj.inverse);
		objB.GetComponent<Renderer> ().material.SetMatrix("_InvView", view.inverse);
		temp.transform.rotation = objB.transform.rotation;
		objB.GetComponent<Renderer> ().material.SetMatrix("_InvRot", temp.transform.worldToLocalMatrix);


		vScale = objB.transform.localScale;
		vPosition = objB.transform.position;

		objB.GetComponent<Renderer> ().material.SetVector("vWindParameters", vWindParameters);
		objB.GetComponent<Renderer> ().material.SetVector("vParameters", vParameters);
		objB.GetComponent<Renderer> ().material.SetVector("vScale", vScale);
		objB.GetComponent<Renderer> ().material.SetVector("vPosition", vPosition);
		objB.GetComponent<Renderer> ().material.SetInt ("_NoOfPointLights", pointLightCount);
		objB.GetComponent<Renderer> ().material.SetVectorArray ("vLightParams1", vLightParameter1);
		objB.GetComponent<Renderer> ().material.SetVectorArray ("vLightParams2", vLightParameter2);

		objB.GetComponent<Renderer> ().material.SetVector ("_FogColor", _FogColor);

	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		UpdateOnRenderImage ();

		FrontFaceCamera.cullingMask = VolumetricObjsFLayer;
		FrontFaceCamera.Render ();
		FrontFaceCamera.cullingMask = 0;

		SceneDepthCamera.cullingMask = MainCameraLayer;
		SceneDepthCamera.Render();
		SceneDepthCamera.cullingMask = 0;

		foreach (GameObject objB in VolumetricObjsB) 
		{
			objB.GetComponent<Renderer> ().material.SetTexture ("_ScenePos", sceneDepthRT);
			objB.GetComponent<Renderer> ().material.SetTexture ("_FrontfaceBoxDepth", frontfaceDepthRT);
		}

		BackFaceCamera.cullingMask = VolumetricObjsBLayer;
		BackFaceCamera.Render ();
		BackFaceCamera.cullingMask = 0;

		fogFilterMat.SetTexture("_FilterTex", fogFilterRT);
		Graphics.Blit (source, destination, fogFilterMat);
		//Graphics.Blit (fogFilterRT, destination);
	}
		
}

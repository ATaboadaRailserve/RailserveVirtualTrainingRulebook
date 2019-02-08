using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// #-----------------------------------------------------------------------------
// #  Copyright (C) 2015  GaGaGames
// #-----------------------------------------------------------------------------

public class SplineElement : MonoBehaviour 
{

    public class MeshTemp
    {
        public float        zsize;
        // Where the pivot is. This is where the Z base rotation should occur
        public Vector3      pivot;
        public Vector3[]    meshverts;

        public Vector3[]    verts;
        public Vector2[]    uvs;
        public int[]        tris;

		// Local transform of the mesh - need to copy into any copy of the mesh
		//public Vector3 		localPos;
		//public Quaternion 	localRot;
		//public Vector3 		localScl;

        public Material     material;

        // Vertex count for the prefab block this represents
        public int          blockVertCount; 
        // Offset that the center pivot has to the front of the block
        public Vector3      blockOffset;
	};

    public enum SplineElementType
    {
		// Extrude the object using its size as a basis for repeating 
        Object_Extrude_Length,
		// Extrude the object to fit "count" number of times per node segment.
        Object_Extrude_Count,
		// Repeat objects from a pool of objects - randomly selected.
		Object_Repeat_RandomGroup,
		// Repeat a single object along a spline path
        Object_Repeat
     };
	
    public Vector3 segment_offset   = Vector3.zero;        // offset center of road (can be negative)
    public Vector3 segment_rotate   = Vector3.zero;        // offset rotation of the object
    public float rotation_random    = 0.0f;
    public float segment_size       = 5.0f;	       // Width of element (footpath, lane etc)
    public int count                = 5;          // Count of objects along the node to node segment - this can be 1->n

    // Spline this node is a part of (parent - road)
    public SplineInterpolator  		spline;
    // Index to the node in this spline
	[HideInInspector]
    public int                  	nodeIndex = -1;
    public float                	tolerance = 0.1f;

    public GameObject       		obj;
	public bool						doNotGenerate = false;
    
	[HideInInspector]
	public List<GameObject>        	allobjs;               // Object that contains segment mesh, material.. etc - simple mesh ideally.
	public SplineElementType  		etype = SplineElementType.Object_Extrude_Length;	    // Element Type - ref a mesh portion

    //private bool            		initDone = false;
	// So multiple elemetns can be applied on a single node
    private int             		id = -1;

	[HideInInspector]
	public GameObject[] 			meshObjects;
	[HideInInspector]
	public int 						ownerId = 0;

	public int GetOwner()
	{
		return ownerId;
	}

	public void SetOwner(int id)
	{
		ownerId = id;
	}

    public void Init(SplineInterpolator s, GameObject o)
    {
        spline        = s;
        obj         = o;

        if(allobjs == null)
            allobjs = new List<GameObject>();

		id = this.GetInstanceID ();
    }

	// Use this for initialization
	void Start () 
    {
    }

    // Button to execute a regen mesh
    public void GenerateElementButton()
	{
        Init(spline, obj);
        GenerateElement();
    }

    // Test a single vert along a z axis
    public bool CheckVert(Vector3 v, float Zvalue)
    {
        if (v.z > Zvalue - tolerance && v.z < Zvalue + tolerance)
            return true;
        return false;
    }

    // CollectVerts along a Z axis
    public int [] CollectVerts(Mesh m, float Zvalue)
    {
        ArrayList results = new ArrayList();
        for (int i = 0; i < m.vertexCount; i++)
        {
            if (CheckVert(m.vertices[i], Zvalue))
                results.Add(i);
        }
        return (int [])results.ToArray(typeof(int));
    }

	private void ReapplyPivotToVertices(ref Vector3 [] verts, Vector3 pivot)
	{
		for (int i = 0; i < verts.Length; i++) {

			verts[i] = verts[i]-pivot;
		}
	}

    // Generates vertices that have been correctly modified with 
    // pivot and segment rotation offsets
    //  segment offset is applied after when the mesh is generated
    public Bounds ModifiedVertices(Vector3 [] oldverts, ref Vector3 [] newverts, 
	                               Transform tf)
    {
        Bounds mbounds = new Bounds();
        for (int i = 0; i < oldverts.Length; i++)
        {
			newverts[i] = Quaternion.Euler(segment_rotate) * oldverts[i];
			newverts[i] = tf.TransformPoint(newverts[i]);
			mbounds.Encapsulate(newverts[i]);
        }
        return mbounds;
    }

    /// <summary>
    /// Make a new pivot - so that it matches the center with the longest Z.
    ///    Front should have '0' Zvalues and Back should have maximim Z values
    ///    Pivot should be center front.
    /// </summary>
    /// <param name="oldverts"></param>
    /// <returns></returns>
    public Vector3 MakePivot(ref Vector3[] oldverts, float zmin)
    {
		Vector3 pivotOffset = new Vector3(0.0f, 0.0f, zmin);
		//Vector3 pivotOffset = new Vector3(mbounds.center.x, mbounds.center.y, mbounds.min.z);
        for (int i = 0; i < oldverts.Length; i++)
            oldverts[i] -= pivotOffset;
        return pivotOffset;
    }
	
    // Name generator for sub objects
    public string MakeName(int i)
    {
		return ( "SplineElementMesh_" + id + "_" + i);
    }

    // Make child game objects and delete old ones with the same name
    public GameObject[] MakeChildren(MeshFilter[] allmeshes)
    {
        // Make a new gameObject - this will hold the meshes for this element.
        meshObjects = new GameObject[allmeshes.Length];
        for (int i = 0; i < allmeshes.Length; i++)
        {
            string childName = MakeName(i);
            for (int c = 0; c < gameObject.transform.childCount; c++)
                if (gameObject.transform.GetChild(c).name == childName)
                    DestroyImmediate(gameObject.transform.GetChild(c).gameObject);

            GameObject meshObject = new GameObject(childName);
            meshObjects[i] = meshObject;
        }
        return meshObjects;
    }

	public void DeleteElements()
	{
		if (meshObjects != null) {
			// meshObjects
			foreach (GameObject m in meshObjects) {
				if(m)
				{
					m.transform.parent = null;
					GameObject.DestroyImmediate (m);
				}
			}

			meshObjects = null;
		}

		if (allobjs != null) {
			// Reset the generated elements
			foreach (GameObject o in allobjs) {
				if(o)
				{
					o.transform.parent = null;
					GameObject.DestroyImmediate (o);
				}
			}
			// Clear the list
			allobjs.Clear ();
		}
	}
	
	// Generate Mesh - from all the profile data
    public void GenerateMesh(bool useCount)
    {
        // Make a list of meshes in the object - get its bounds.
        MeshFilter[] allmeshes = MathUtils.GetAllChilds<MeshFilter>(obj.transform);
        Bounds meshbounds = new Bounds();

        // Allocate vert, uvs, triangles and materials - copy in the shared data
        MeshTemp[] allmeshtemps = new MeshTemp[allmeshes.Length];
		float zscale = 1.0f /(float)count;

		// Use this GO for transforms that dont effect the source prefab!!
		GameObject temp = new GameObject ();

        // Count through meshes - prepare them and collect oriented vertices
        for (int mc = 0; mc < allmeshes.Length; mc++ )
        {
            // New mesh
            allmeshtemps[mc] = new MeshTemp();
            // Test direction - we alway move along spline in +ve Z axis direction.
            allmeshtemps[mc].meshverts = new Vector3[allmeshes[mc].sharedMesh.vertices.Length];

			temp.transform.position = allmeshes[mc].gameObject.transform.position;
			temp.transform.rotation = allmeshes[mc].gameObject.transform.rotation;
			temp.transform.localScale = allmeshes[mc].gameObject.transform.localScale;
			Vector3 oscale = temp.transform.localScale;

			if(useCount == true) {
				temp.transform.localScale = new Vector3(oscale.x, oscale.y, zscale);
			}
            Bounds mbound = ModifiedVertices(allmeshes[mc].sharedMesh.vertices, 
			                                 ref allmeshtemps[mc].meshverts, temp.transform);
            Vector3 pivot = MakePivot(ref allmeshtemps[mc].meshverts, mbound.min.z);
            allmeshtemps[mc].zsize = mbound.size.z;
			allmeshtemps[mc].pivot = new Vector3(0.0f, 0.0f, pivot.z);
			//Debug.Log ("Pivot: " + pivot);
			//ReapplyPivotToVertices(ref allmeshtemps[mc].meshverts, allmeshtemps[mc].pivot); 
//			allmeshtemps[mc].localPos = allmeshes[mc].gameObject.transform.localPosition;
//			allmeshtemps[mc].localScl = allmeshes[mc].gameObject.transform.localScale;
//			allmeshtemps[mc].localRot = allmeshes[mc].gameObject.transform.localRotation;

//            allmeshtemps[mc].frontverts = CollectVerts(allmeshes[mc].sharedMesh, -zsize);
//            allmeshtemps[mc].backverts = CollectVerts(allmeshes[mc].sharedMesh, zsize);
            meshbounds.Encapsulate(mbound);
        }

		// Realign pivots for all meshes
		for (int md = 0; md < allmeshes.Length; md++)
		{
			allmeshtemps[md].pivot = MakePivot(ref allmeshtemps[md].meshverts, meshbounds.min.z - allmeshtemps[md].pivot.z);
			allmeshtemps[md].zsize = meshbounds.size.z;
		}

		// Remove the created obj
		DestroyImmediate (temp);

		float totalzsize = meshbounds.size.z;
        // Mesh bounds are AA aligned.. this could be a pain - need to ensure mesh setup
        //      NOTE: All prefabs must be oriented XZ.. with Y up or height
        //float distance = spline.mPoints[nodeIndex + 1].Time - spline.mPoints[nodeIndex].Time;
        meshObjects = MakeChildren(allmeshes);

        // Iterate until we overshoot the distance of the spline - this is not overly accurate
        int maxObjects = 0;
        // This is a bit of a hack - only checks the first mesh. 
        // TODO: make this check all meshes to get the right step?
        float boundz = meshbounds.size.z;
		float dlen = spline.GetLength(nodeIndex);
		float dlen2 = spline.GetLength(nodeIndex+1); 
		maxObjects = (int)Math.Ceiling((dlen2 - dlen) / (boundz * 1.2f));
		count = maxObjects;
		//Debug.Log ("Max Objects: " + maxObjects);
        
        for (int i = 0; i < allmeshes.Length; i++)
        {
            Mesh sm = allmeshes[i].sharedMesh;

            allmeshtemps[i].material = allmeshes[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            allmeshtemps[i].verts = new Vector3[maxObjects * sm.vertexCount];
            allmeshtemps[i].uvs = new Vector2[maxObjects * sm.vertexCount];
            allmeshtemps[i].tris = new int[maxObjects * (sm.triangles.Length)];

            allmeshtemps[i].blockVertCount = sm.vertexCount;
            allmeshtemps[i].blockOffset = sm.bounds.center;
            allmeshtemps[i].blockOffset.z = sm.bounds.min.z;

            // Make copies of all the data into the vert buffers, uv buffers and index buffers
            for(int o=0; o<maxObjects; o++)
            {
                Array.Copy(allmeshtemps[i].meshverts, 0, allmeshtemps[i].verts, o * sm.vertexCount, sm.vertexCount);
                Array.Copy(sm.uv, 0, allmeshtemps[i].uvs, o * sm.vertexCount, sm.vertexCount);
                Array.Copy(sm.triangles, 0, allmeshtemps[i].tris, o * (sm.triangles.Length), sm.triangles.Length);

                int offset = o * (sm.triangles.Length);
                for(int t=0; t<sm.triangles.Length; t++)
                    allmeshtemps[i].tris[t + offset] = sm.triangles[t] + o * sm.vertexCount;
                //allmeshtemps[i].tris[sm.triangles.Length + offset + 0] = 0;
                //allmeshtemps[i].tris[sm.triangles.Length + offset + 1] = 0;
                //allmeshtemps[i].tris[sm.triangles.Length + offset + 2] = 0;
            }
        }

        // Iterate until we overshoot the distance of the spline - this is not overly accurate
		dlen = (float)nodeIndex / (float)(spline.mPoints.Count - 1);
		float lastdlen = (float)(nodeIndex+1) / (float)(spline.mPoints.Count - 1);
		Vector3 pos = spline.mPoints [nodeIndex].position;
		float bstep = ( lastdlen - dlen ) / maxObjects;

        for (int objcount = 0; objcount < maxObjects; objcount++)
        {
            Vector3 t = new Vector3();

			Vector3 fwd = spline.PointOnPath(dlen + bstep);
			if(objcount == 0) fwd = spline.PointOnPath(dlen + bstep * 0.1f);
			t = fwd - pos;
			Quaternion mainRot = Quaternion.LookRotation(t);
			t.Normalize();

			bool doBackBlock = (objcount <= maxObjects - 1) || (dlen + bstep > lastdlen);

            float newdlen = dlen + bstep;
			Vector3 newpos = spline.PointOnPath(newdlen);

			Vector3 nextt = new Vector3();
			fwd = spline.PointOnPath(newdlen + bstep);
			nextt = fwd - newpos;
			Quaternion nextMainRot = Quaternion.LookRotation(nextt);
			nextt.Normalize();
			            
			if ((objcount == maxObjects - 1) || (dlen + bstep > lastdlen))
            {
                nextt = new Vector3();
				newpos = spline.mPoints[nodeIndex+1].position;
				fwd = spline.PointOnPath(lastdlen + bstep * 0.1f);
				nextt = fwd - newpos;
				nextMainRot = Quaternion.LookRotation(nextt);
				nextt.Normalize();;
            }

            //go = (GameObject)GameObject.Instantiate(obj, pos + segment_offset, mainRot);
            //go.transform.SetParent(this.transform);

            // If we have more than 1 node position done (2 or more) then we can effectively do
            // the previous and next faces
            // Iterate all mesh filters - the verts in the tolerance zone are modified
            for (int i = 0; i < allmeshes.Length; i++)
            {
                int bvc = allmeshtemps[i].blockVertCount;
                int offset = objcount * bvc;

                for (int v = 0; v < bvc; v++)
                {
                    bool written = false;
                    Vector3 vert = allmeshtemps[i].verts[v + offset];
                    // Check for back snapping to next node
                    if (doBackBlock)
                    {
						if (CheckVert(vert, totalzsize))
                        {
							Vector3 newvert = new Vector3(vert.x, vert.y, vert.z - boundz);
							allmeshtemps[i].verts[v + offset] = (nextMainRot * (newvert + segment_offset)) + newpos;
                            written = true;
                        }
                    }
                    if(!written) allmeshtemps[i].verts[v + offset] = (mainRot * (vert + segment_offset)) + pos;
                }
            }

            dlen += bstep;
			pos = spline.PointOnPath(dlen);
        }

        for (int i = 0; i < allmeshes.Length; i++)
        {
            MeshRenderer render = meshObjects[i].AddComponent<MeshRenderer>();
            render.material = allmeshtemps[i].material;
            MeshFilter filter = meshObjects[i].AddComponent<MeshFilter>();
			Mesh mesh = new Mesh();

            mesh.vertices = allmeshtemps[i].verts;
            //mesh.normals = normales;
            mesh.uv = allmeshtemps[i].uvs;
            mesh.triangles = allmeshtemps[i].tris;
			
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			;

			//meshObjects[i].transform.localPosition = allmeshtemps[i].localPos;
			//meshObjects[i].transform.localScale = allmeshtemps[i].localScl;
			//meshObjects[i].transform.localRotation = allmeshtemps[i].localRot;

			meshObjects[i].transform.SetParent(gameObject.transform);
			filter.sharedMesh = mesh;
        }
    }

	private void RepeatMesh()
	{
		// Precalc distance using count segments on hermite spline  
		//float dist1 = iTween.PathLength(spline.mPoints.GetRange(0, nodeIndex).ToArray());
		//float dist2 = iTween.PathLength(spline.mPoints.GetRange(0, nodeIndex+1).ToArray());
		float pdist1 = (float)nodeIndex / (float)(spline.mPoints.Count - 1);
		float pdist2 = (float)(nodeIndex + 1) / (float)(spline.mPoints.Count - 1);
		float calcDist = pdist1;
		float objectStep = (pdist2 - pdist1) / (float)count;
		
		Vector3 pos = spline.PointOnPath (calcDist);
	
		Quaternion rotRandom = Quaternion.identity;
	
		//Debug.Log (calcDist + "     " + spline.maxDistance);
		for (int i = 0; i < count; i++) {

			GameObject robj = obj;
			if(etype == SplineElementType.Object_Repeat_RandomGroup)
				robj = obj.transform.GetChild(UnityEngine.Random.Range(0, obj.transform.childCount-1)).gameObject;

			// step small forward to calc direction vector
			Vector3 fwd = spline.PointOnPath (calcDist + 0.01f);
			Vector3 direction = fwd - pos;
			
			if (rotation_random != 0.0f)
				rotRandom = Quaternion.Euler (0.0f, UnityEngine.Random.Range (0.0f, rotation_random), 0.0f);
			GameObject go = (GameObject)GameObject.Instantiate (robj, pos + Quaternion.LookRotation (direction) * segment_offset, 
			                                                    Quaternion.LookRotation (direction) * Quaternion.Euler (segment_rotate) * rotRandom);
			go.name = MakeName (i);
			go.transform.SetParent (this.transform);
			allobjs.Add (go);
			
			//pos += (Quaternion.AngleAxis(-90, Vector3.up) * tangent) * objectStep;
			calcDist += objectStep;
			pos = spline.PointOnPath (calcDist);
		}
	}

    public void GenerateElement()
    {
        // If unassigned then nothing to really do
        if (obj == null)
            return;

		DeleteElements();
		
		// Generation is reasonably simple:
        //  - take the obj piece. Align front verts with node tangent
        //  - align rear verts with next node tangent
        //  - interpolate any verts inbetween with hermite helper
		if (spline == null) {
			spline = this.gameObject.transform.parent.gameObject.GetComponent<SplineInterpolator> ();
			spline.Reset();
		}

		if (etype == SplineElementType.Object_Repeat) {
			RepeatMesh();
		} 

		else if (etype == SplineElementType.Object_Repeat_RandomGroup) {
			RepeatMesh();
		}

		else if(etype == SplineElementType.Object_Extrude_Length)
        {
            // Collect vertices from object - we make a mesh length for each embedded mesh
            GenerateMesh(false);
        }
		else if(etype == SplineElementType.Object_Extrude_Count)
		{
			// Collect vertices from object - we make a mesh length for each embedded mesh
			GenerateMesh(true);
		}
	}


    public static Vector3 RotateAroundAxis(Vector3 v, float a, Vector3 axis)
    {
        var q = Quaternion.AngleAxis(a, axis);
        return q * v;
    }
	
    // Pass in the mesh to be modified and the tangent difference between front and back
    void AdjustMesh( Bounds b, GameObject go, Vector3 tdiff)
    {
        // Dont adjust mesh if its not necessary
        if (tdiff == Vector3.zero)
            return;
        MeshFilter[] meshes = obj.GetComponents<MeshFilter>();
        Vector3 tnorm = Vector3.Normalize(tdiff);

        foreach(MeshFilter mf in meshes)
        {
            Vector3[] verts = mf.sharedMesh.vertices;
            // Go thru the verts - if they closely match (within tolerance) the Z extents, then modify.
            for (int i = 0; i < verts.Length; i++)
            {
                if (verts[i].z > b.max.z - tolerance && verts[i].z < b.max.z + tolerance)
                {
                    float oldZ = verts[i].z;
                    Vector3 newvert = new Vector3(verts[i].x, verts[i].y, 0.0f);
                    verts[i] = Quaternion.LookRotation(tnorm) * newvert + new Vector3(0.0f, 0.0f, oldZ);
                }
            }

            mf.sharedMesh.RecalculateBounds();
            ;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}

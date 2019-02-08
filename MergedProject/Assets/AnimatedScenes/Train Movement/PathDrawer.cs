using System.Linq;
using UnityEngine;
    
[ExecuteInEditMode]
public class PathDrawer : MonoBehaviour {
    
    public int segments = 10;
    public float width = 1.0f;
    public bool hideGizmo;
    public bool generateMesh;
    public bool deleteMesh;
    public Vector3[] path;
    public Vector3 normal = Vector3.up;
    int dottedLength = 0;

    void Start()
    {
        this.enabled = false;
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
        GeneratePath();
        if (generateMesh)
        {
            generateMesh = false;
            GenerateMesh();
        }
        if(deleteMesh)
        {
            deleteMesh = false;
            DeleteMesh();
        }
    }

    float factorial(int n)
    {
        if (n <= 1)
            return 1;
        else
            return n * factorial(n - 1);
    }

    float nCn(int top, int bottom)
    {
        return factorial(top) / (factorial(bottom) * factorial(top - bottom));
    }

    void OnDrawGizmos()
    {
        if(path != null && !hideGizmo)
        {
            Gizmos.color = Color.blue;
            for(int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }

    void GeneratePath()
    {
        if (transform.childCount > 0 && transform.GetChild(0).name == "generated path")
            Destroy(transform.GetChild(0).gameObject);

        Transform[] children = transform.GetComponentsInChildren<Transform>().Where(elem => elem.gameObject != this.gameObject).ToArray();
        int numVecs = segments + 1;
        path = new Vector3[numVecs];
        int n = children.Length - 1;
        for(int i = 0; i < numVecs; i++)
        {
            path[i] = new Vector3();
            for(int j = 0; j < children.Length; j++)
            {
                float coeff = nCn(n, j);
                float t = (float)i / (numVecs - 1);
                float oneMinusT = 1.0f - t;
                path[i] +=  coeff * Mathf.Pow(oneMinusT, n-j) * Mathf.Pow(t, j) * children[j].transform.position;
            }
        }
    }

    void DeleteMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter.sharedMesh != null)
            DestroyImmediate(filter.sharedMesh);
    }

    void GenerateMesh()
    {
        if (path == null || path.Length < 2)
            return;

        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter.sharedMesh != null)
            DestroyImmediate(filter.sharedMesh);

        Vector3[] vertices = new Vector3[path.Length * 2];
        int[] triangles = new int[path.Length * 6];
        Vector3[] normals = new Vector3[path.Length * 2];
        Vector2[] uv = new Vector2[path.Length * 2];
        Vector3 leftVector;

        Vector3[] localPath = new Vector3[path.Length];
        for (int i = 0; i < localPath.Length; i++)
            localPath[i] = transform.InverseTransformPoint(path[i]);

        for(int i = 0; i < localPath.Length; i++)
        {
            if (i == 0)
            {
                leftVector = Vector3.Cross(localPath[1] - localPath[0], normal);
            }
            else if (i == localPath.Length - 1)
            {
                leftVector = Vector3.Cross(localPath[i] - localPath[i - 1], normal);
            }
            else
            {
                Vector3 forward = localPath[i + 1] - localPath[i];
                Vector3 backward = localPath[i] - localPath[i - 1];
                leftVector = Vector3.Cross(backward + forward, normal);
            }

            leftVector.Normalize();

            vertices[i*2] = localPath[i] + (leftVector * (width / 2.0f));
            vertices[(i * 2) + 1] = localPath[i] - (leftVector * (width / 2.0f));
            
            normals[i * 2] = normal;
            normals[(i * 2) + 1] = normal;
        }

        for(int i = 0; i < localPath.Length - 1; i++)
        {
            triangles[i * 6] = i * 2;
            triangles[(i * 6) + 1] = (i * 2) + 2;
            triangles[(i * 6) + 2] = (i * 2) + 1;
            triangles[(i * 6) + 3] = (i * 2) + 2;
            triangles[(i * 6) + 4] = (i * 2) + 3;
            triangles[(i * 6) + 5] = (i * 2) + 1;
        }

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        for(int i = 4; i < uv.Length; i++)
        {
            uv[i] = new Vector2(0, 0);
        }


        Mesh mesh = new Mesh();
        mesh.name = "Generated Mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        //mesh.uv = uv;
        filter.sharedMesh = mesh;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class YardLimitsBox : MonoBehaviour {

    public float scrollSpeed = 0.2f;
    public float verticalOffset = 0.5f;
    public bool hideOnStart = true;
    public Renderer pXi;
    public Renderer nXi;
    public Renderer pZi;
    public Renderer nZi;
    public Renderer pXo;
    public Renderer nXo;
    public Renderer pZo;
    public Renderer nZo;
    Vector2 offset;
    bool isVisible;

    void Start () {
        offset = Vector2.zero;
        SetVisible(!hideOnStart);
        if (Application.isPlaying)
            FixMaterials();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            if (isVisible)
                ScrollTexture();
        }
        else
        {
            isVisible = true;
            FitColliderBounds();
        }
        UpdateVisible();
    }

    void FitColliderBounds()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // Get transforms for each boundary edge
        Transform pxT = pXi.transform;
        Transform nxT = nXi.transform;
        Transform pzT = pZi.transform;
        Transform nzT = nZi.transform;
        Transform pxT_Outer = pXo.transform;
        Transform nxT_Outer = nXo.transform;
        Transform pzT_Outer = pZo.transform;
        Transform nzT_Outer = nZo.transform;

        // Center each boundary on its axis
        Vector3 center = boxCollider.center + transform.position;
        float halfWidth = boxCollider.bounds.extents.x;
        float halfDepth = boxCollider.bounds.extents.z;
        float halfHeight = boxCollider.bounds.extents.y;
        Vector3 LocalCenter = center + (Vector3.down * halfHeight) + (Vector3.up * verticalOffset);

        pxT.position = LocalCenter + Vector3.right * halfWidth;
        nxT.position = LocalCenter - Vector3.right * halfWidth;
        pzT.position = LocalCenter + Vector3.forward * halfDepth;
        nzT.position = LocalCenter - Vector3.forward * halfDepth;
        pxT_Outer.position = pxT.position;
        nxT_Outer.position = nxT.position;
        pzT_Outer.position = pzT.position;
        nzT_Outer.position = nzT.position;

        // Scale each boundary to fill axis
        Vector3 xScale = new Vector3(boxCollider.size.z, 1, 1);
        Vector3 zScale = new Vector3(boxCollider.size.x, 1, 1);

        pxT.localScale = xScale;
        nxT.localScale = xScale;
        pzT.localScale = zScale;
        nzT.localScale = zScale;
        pxT_Outer.localScale = xScale;
        nxT_Outer.localScale = xScale;
        pzT_Outer.localScale = zScale;
        nzT_Outer.localScale = zScale;
    }

    void FixMaterials()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 xScale = new Vector3(box.size.z / 2, 1, 1);
        Vector3 zScale = new Vector3(box.size.x / 2, 1, 1);
        pXi.material.mainTextureScale = xScale;
        nXi.material.mainTextureScale = xScale;
        pZi.material.mainTextureScale = zScale;
        nZi.material.mainTextureScale = zScale;
        pXo.material.mainTextureScale = xScale;
        nXo.material.mainTextureScale = xScale;
        pZo.material.mainTextureScale = zScale;
        nZo.material.mainTextureScale = zScale;
    }

    void ScrollTexture()
    {
        offset.x += scrollSpeed * Time.deltaTime;
        pXi.material.mainTextureOffset = offset;
        nXi.material.mainTextureOffset = offset;
        pZi.material.mainTextureOffset = offset;
        nZi.material.mainTextureOffset = offset;
        pXo.material.mainTextureOffset = -offset;
        nXo.material.mainTextureOffset = -offset;
        pZo.material.mainTextureOffset = -offset;
        nZo.material.mainTextureOffset = -offset;
    }

    void UpdateVisible()
    {
        pXi.enabled = isVisible;
        nXi.enabled = isVisible;
        pZi.enabled = isVisible;
        nZi.enabled = isVisible;
        pXo.enabled = isVisible;
        nXo.enabled = isVisible;
        pZo.enabled = isVisible;
        nZo.enabled = isVisible;
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
    }
}

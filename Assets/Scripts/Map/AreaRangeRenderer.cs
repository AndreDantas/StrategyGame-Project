using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AreaRangeRenderer : MonoBehaviour
{
    public MeshRenderer meshRender;
    public Color renderColor = Color.green;
    MeshFilter meshFilter;

    public void Awake()
    {
        meshRender = GetComponent<MeshRenderer>();
        if ((meshFilter = GetComponent<MeshFilter>()) != null)
            meshFilter.mesh = null;
    }

    /// <summary>
    /// This function renders squares in the positions.
    /// </summary>
    /// <param name="squarePos">List of positions.</param>
    /// <param name="scale">Scale of each square.</param>
    public void RenderSquaresArea(List<Vector3> squarePos, float scale = 1f)
    {
        if (squarePos == null || squarePos.Count == 0 || !meshFilter)
            return;

        Mesh mesh;
        meshFilter.mesh = mesh = new Mesh();

        int verticesNumber = squarePos.Count * 4; // 4 vertices per square
        int trianglesNumber = squarePos.Count * 6; // 2 Triangles per square
        scale = MathOperations.ClampMin(scale, 0);

        Vector3[] vertices = new Vector3[verticesNumber];
        int[] triangles = new int[trianglesNumber];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < squarePos.Count; i++)
        {
            // Vertices Creation
            // Bottom left 0
            vertices[i * 4] = (squarePos[i] + new Vector3(1 - scale, 1 - scale, 0) + new Vector3(-0.5f, -0.5f, -1) - transform.position);
            uv[i * 4] = new Vector2(squarePos[i].x / scale, squarePos[i].y / scale) + new Vector2(-0.5f, -0.5f) - (Vector2)transform.position;
            // Bottom right 1
            vertices[i * 4 + 1] = (squarePos[i] + new Vector3(scale, 1 - scale, 0) + new Vector3(-0.5f, -0.5f, -1) - transform.position);
            uv[i * 4 + 1] = new Vector2(squarePos[i].x / scale, squarePos[i].y / scale) + new Vector2(-0.5f, -0.5f) - (Vector2)transform.position;
            // Top right 2
            vertices[i * 4 + 2] = (squarePos[i] + new Vector3(scale, scale, 0) + new Vector3(-0.5f, -0.5f, -1) - transform.position);
            uv[i * 4 + 2] = new Vector2(squarePos[i].x / scale, squarePos[i].y / scale) + new Vector2(-0.5f, -0.5f) - (Vector2)transform.position;
            // Top left 3
            vertices[i * 4 + 3] = (squarePos[i] + new Vector3(1 - scale, scale, 0) + new Vector3(-0.5f, -0.5f, -1) - transform.position);
            uv[i * 4 + 3] = new Vector2(squarePos[i].x / scale, squarePos[i].y / scale) + new Vector2(-0.5f, -0.5f) - (Vector2)transform.position;


            // Triangles Creation
            triangles[i * 6] = i * 4;
            triangles[i * 6 + 1] = i * 4 + 3;
            triangles[i * 6 + 2] = i * 4 + 1;

            triangles[i * 6 + 3] = i * 4 + 1;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 2;


        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        if (meshRender)
            meshRender.material.color = renderColor;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    /// <summary>
    /// Clears the rendering.
    /// </summary>
    public void Clear()
    {
        if (meshFilter)
            meshFilter.mesh = null;
    }

    /// <summary>
    /// Updates the rendering if the mesh is not null.
    /// </summary>
    /// <param name="squarePos">List of positions.</param>
    /// <param name="scale">Scale of each square.</param>
    public void UpdateRender(List<Vector3> squarePos, float scale = 1f)
    {
        if (!meshFilter)
            return;
        if (meshFilter.mesh == null)
            return;

        RenderSquaresArea(squarePos, scale);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshSphereGenerator : MonoBehaviour
{
    
    private float value, start1, stop1, start2, stop2;
    private float _x, _y, _z;
    float lon, lat;
    private float radius = 10f;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;
    float y;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
    }

    private void Update()
    {
        UpdateMesh();
    }

    float RemapFunction (float _value, float _start1, float _stop1, float _start2, float _stop2)
    {
        float outgoing = _start2 + (_stop2 - _start2) * ((_value - _start1) / (_stop1 - _start1));
        return outgoing;
    }

    IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) //longitude
        {
            lon = RemapFunction(i, 0f, xSize, -Mathf.PI, Mathf.PI);
                for (int x = 0; x <= xSize; x++)
                {
                    lat = RemapFunction(x, 0f, xSize, -Mathf.PI/2f, Mathf.PI/2f);
                    _x = radius * Mathf.Sin(lon) * Mathf.Cos(lat);
                    _y = radius * Mathf.Sin(lon) * Mathf.Sin(lat);
                    _z = radius * Mathf.Cos(lon);
                    
                    vertices[i] = new Vector3(_x, _y, _z);
                    i++;
                }
        }
        

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;            
            
                vert++;
                tris += 6;

            }
            vert++;   
            yield return new WaitForSeconds(.001f);         
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        Debug.Log(mesh.normals);
    }
}
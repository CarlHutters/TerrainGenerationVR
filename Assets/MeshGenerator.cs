using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    Vector3 crossProd;

    public int xSize = 20;
    public int zSize = 20;
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

    IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];


        //Generate grid of vertices
        for (int i = 0, z = 0; z < zSize+1; z++)
        {
            for (int x = 0; x < xSize+1; x++)
            {
                
                vertices[i] = new Vector3(x, 0, z);
                Debug.Log("Vertex " + i + " (x,z) = (" + vertices[i].x + "," + vertices[i].z + ")");
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
                if(z<zSize/2)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;
                } else 
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 2] = vert + xSize + 1;
                    triangles[tris + 1] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 5] = vert + xSize + 1;
                    triangles[tris + 4] = vert + xSize + 2;
                }

            
                vert++;
                tris += 6;

                yield return new WaitForSeconds(.1f);
            }
            vert++;            
        }

        // triangles = new int[9];

        // //Triangle 1
        // triangles[0] = 0;
        // triangles[1] = xSize + 1;
        // triangles[2] = 1;

        // //Triangle 2
        // triangles[3] = 1;
        // triangles[4] = 2;
        // triangles[5] = xSize + 2;

        // //Triangle 2
        // triangles[6] = 2;
        // triangles[7] = 3;
        // triangles[8] = xSize + 3;
        

        //crossProd = Vector3.Cross(vertices[1], vertices[xSize + 1]).normalized;

        //Debug.DrawLine(new Vector3(0,0,0), crossProd, new Color(0, 0, 1.0f), 200);
        
        yield return new WaitForSeconds(.01f);
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos() {
        if( vertices==null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}

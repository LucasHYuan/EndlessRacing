using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Material meshMaterial;
    public Vector2 dimensions;
    public float perlinScale;
    public float perlinOffset;
    public float scale;
    public float waveHeight;
    public float globalSpeed;

    GameObject[] pieces = new GameObject[2];

    void Start()
    {
        for(int i = 0; i < 2; ++i)
        {
            GenerateWorldPiece(i);
        }
    }

    void GenerateWorldPiece(int i)
    {
        pieces[i] = CreateCylinder();
        pieces[i].transform.Translate(Vector3.forward * (dimensions.y * scale * Mathf.PI) * i);

        UpdateSinglePiece(pieces[i]);

    }

    void LateUpdate()
    {
        if (pieces[1] && pieces[1].transform.position.z <= 0)
        {
            StartCoroutine(UpdateWorldPieces());
        }

    }

    IEnumerator UpdateWorldPieces()
    {
        Destroy(pieces[0]);
        pieces[0] = pieces[1];
        pieces[1] = CreateCylinder();
        pieces[1].transform.position = pieces[0].transform.position + Vector3.forward * (dimensions.y * scale * Mathf.PI);
        pieces[1].transform.rotation = pieces[0].transform.rotation;

        UpdateSinglePiece(pieces[1]);
        yield return 0;
    }


    void UpdateSinglePiece(GameObject piece)
    {
        BasicMovement bm = piece.AddComponent<BasicMovement>();
        bm.moveSpeed = -globalSpeed;

        GameObject endPoint = new GameObject();
        endPoint.transform.position = piece.transform.position + Vector3.forward * (dimensions.y * scale * Mathf.PI);
        endPoint.name = "EndPoint";
        endPoint.transform.parent = piece.transform;
    }


    public GameObject CreateCylinder()
    {
        GameObject newCylinder = new GameObject();
        newCylinder.name = "WorldPiece";

        MeshFilter meshFilter = newCylinder.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newCylinder.AddComponent<MeshRenderer>();


        meshRenderer.material = meshMaterial;
        meshFilter.mesh = GenerateMesh();

        newCylinder.AddComponent<MeshCollider>();
        return newCylinder;
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "CylinderMesh";

        Vector3[] vertices = null;
        Vector2[] uvs = null;
        int[] triangles = null;

        GenerateShapes( ref vertices, ref uvs, ref triangles );

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        //mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    void GenerateShapes(ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles)
    {
        int xCount = (int)dimensions.x;
        int zCount = (int)dimensions.y;
        int vertice_size = (xCount + 1) * (zCount + 1);
        vertices = new Vector3[vertice_size];
        uvs = new Vector2[vertice_size];

        int idx = 0;
        float radius = xCount * scale * 0.5f;
        for(int x = 0; x <= xCount; ++x)
        {
            for (int z = 0; z <= zCount; ++z)
            {
                float angle = x * Mathf.PI * 2f / xCount;
                vertices[idx] = new Vector3(
                    Mathf.Cos(angle) * radius, 
                    Mathf.Sin(angle) * radius,
                    z * scale * Mathf.PI
                );
                uvs[idx] = new Vector2(x * scale, z * scale);
                float pX = (vertices[idx].x * perlinScale) + perlinOffset;
                float pZ = (vertices[idx].z * perlinScale) + perlinOffset;


                Vector3 center = new Vector3(0, 0, vertices[idx].z);
                vertices[idx] += (center - vertices[idx]).normalized * Mathf.PerlinNoise(pX,pZ) * waveHeight;
                ++idx;
            }
        }

        triangles = new int[xCount * zCount * 6];
        
        idx = 0;
        for(int x = 0; x < xCount; ++x)
        {
            int[] boxBase = new int[]
            {
                x * (zCount + 1),
                x * (zCount + 1) + 1,
                (x + 1) * (zCount + 1),
                x * (zCount + 1) + 1,
                (x + 1) * (zCount + 1) + 1,
                (x + 1) * (zCount + 1),
            };
            for (int z = 0; z < zCount; ++z)
            {
                for(int i = 0; i < 6; ++i)
                {
                    boxBase[i] = boxBase[i] + 1;
                }
                for(int j = 0; j < 6; ++j)
                {
                    triangles[idx + j] = boxBase[j] - 1;
                }

                idx += 6;
            }
        }
    }

}

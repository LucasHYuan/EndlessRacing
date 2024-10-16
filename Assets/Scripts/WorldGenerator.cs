using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WorldGenerator : MonoBehaviour
{
    public Material meshMaterial;
    public Vector2 dimensions;
    public float perlinScale;
    public float perlinOffset;
    public float scale;
    public float waveHeight;
    public float globalSpeed;
    public float randomness;
    public float startTransitionLength;
    private Vector3[] beginPoints;
    GameObject[] pieces = new GameObject[2];
    public int startObstacleChance;
    public GameObject gate;
    public int obstacleChanceAcceleration;
    public int gateChance;
    public GameObject[] obstacles;
    private GameObject currentCyclinder;
    public BasicMovement lampMovement;
    public int showItemDistance;
    public float shadowHeight;
    int level = 0;
    int gateCount = 0;
    int gateLimit = 3;
    void Start()
    {
        beginPoints = new Vector3[(int)dimensions.x + 1];
        for(int i = 0; i < 2; ++i)
        {
            GenerateWorldPiece(i);
        }
    }

    void GenerateWorldPiece(int i)
    {
        level++;
        if(level > 3)
        {
            gateChance += 5;
            startObstacleChance -= 100;
            globalSpeed += 5;
            foreach (var piece in pieces)
            {
                piece.GetComponent<BasicMovement>().moveSpeed = -globalSpeed;
            }
        }
        pieces[i] = CreateCylinder();
        pieces[i].transform.Translate(Vector3.forward * (dimensions.y * scale * Mathf.PI) * i);
        UpdateSinglePiece(pieces[i]);

    }

    void LateUpdate()
    {
        if (pieces[1] && pieces[1].transform.position.z <= -15f)
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

    void UpdateAllItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        for (int i = 0; i < items.Length; i++)
        {
            foreach (MeshRenderer renderer in items[i].GetComponentsInChildren<MeshRenderer>())
            {
                bool show = items[i].transform.position.z < showItemDistance;
                if (show)
                    renderer.shadowCastingMode = (items[i].transform.position.y < shadowHeight) ? ShadowCastingMode.On : ShadowCastingMode.Off;
                renderer.enabled = show;
            }
        }
    }


    void UpdateSinglePiece(GameObject piece)
    {
        BasicMovement bm = piece.AddComponent<BasicMovement>();
        bm.moveSpeed = -globalSpeed;

        if(lampMovement != null)
            bm.rotateSpeed = lampMovement.rotateSpeed;

        GameObject endPoint = new GameObject();
        endPoint.transform.position = piece.transform.position + Vector3.forward * (dimensions.y * scale * Mathf.PI);
        endPoint.name = "EndPoint";
        endPoint.transform.parent = piece.transform;

        perlinOffset += randomness;

        if (startObstacleChance > 5)
            startObstacleChance -= obstacleChanceAcceleration;
    }


    public GameObject CreateCylinder()
    {
        GameObject newCylinder = new GameObject();
        newCylinder.name = "WorldPiece";
        currentCyclinder = newCylinder;
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

                if(z < startTransitionLength && beginPoints[0] != Vector3.zero)
                {
                    float perlinPercentage = z * (1f / startTransitionLength);
                    Vector3 beginPoint = new Vector3(beginPoints[x].x, beginPoints[x].y, vertices[idx].z);
                    vertices[idx] = (perlinPercentage * vertices[idx]) + (1 - perlinPercentage) * beginPoint;
                }
                else if(z == zCount)
                {
                    beginPoints[x] = vertices[idx];
                }


                if(Random.Range(0, startObstacleChance) == 0 
                    && !(gate == null && obstacles.Length == 0))
                {
                    CreateItem(vertices[idx], x);
                }

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

    public Transform GetWorldPiece()
    {
        return pieces[0].transform;
    }


    void CreateItem(Vector3 vec, int x)
    {
        Vector3 zCenter = new Vector3(0,0,vec.z);
        if (zCenter - vec == Vector3.zero || x == (int)dimensions.x / 4 || x == (int)dimensions.x / 4 * 3)
            return;
        GameObject newItem = Instantiate(
            UnityEngine.Random.Range(0, gateChance) == 0 
            ? gate 
            : obstacles[UnityEngine.Random.Range(0, obstacles.Length)]
            );
        newItem.transform.rotation = Quaternion.LookRotation(zCenter - vec, Vector3.up);
        newItem.transform.position = vec;
        newItem.transform.SetParent(currentCyclinder.transform, false);
    }

    public void UpdateGateChance()
    {
        gateCount++;
        if(gateCount > gateLimit)
        {
            gateChance++;
            gateCount = 0;
            gateLimit++;
        }
        startObstacleChance--;
        globalSpeed++;
        perlinScale += 0.005f;
        foreach(var piece in pieces)
        {
            piece.GetComponent<BasicMovement>().moveSpeed = -globalSpeed;
        }
    }
}


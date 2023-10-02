using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGeneratorV1 : MonoBehaviour
{
    public GameObject player;
    Mesh mesh;
    public GameObject[] objects;
    private int MESH_SCALE = 200;
    public AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;

    public int xSize;
    public int zSize;

    public float scale; 
    public int octaves;
    public float lacunarity;

    public int seed;
    private System.Random prng;
    private Vector2[] octaveOffsets;

    private float lastNoiseHeight;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh= mesh;
        CreateNewMap();


    }

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
        UpdateMesh();
    }

    private void CreateMeshShape ()
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0)
            scale = 0.0001f;
            
        // Create vertices array
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Assign and set height of each vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }
    }

    private Vector2[] GetOffsetSeed()
    {
        seed = Random.Range(0, 1000);
        // changes area of map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
                    
        for (int o = 0; o < octaves; o++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        float amplitude = 12;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / scale * frequency + octaveOffsets[y].y;
            float mapX = x / scale * frequency + octaveOffsets[y].x;

            // Create perlinValues  
            // The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight;
    }

    private void CreateTriangles() 
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        // loop through rows
        for (int z = 0; z < xSize; z++)
        {
            // fill all columns in row
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
        }
    }

    private void MapEmbellishments() 
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            Debug.Log(noiseHeight);

            // Stop generation if height difference between 2 vertices is too steep
            if(System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > 50)
                {
                    
                    // Chance to generate
                    if (Random.Range(1, 2) == 1)
                    {
                        GameObject objectToSpawn = objects[Random.Range(0, objects.Length)];
                        var spawnAboveTerrainBy = noiseHeight;
                        Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity);
                    }
                }
            }

            lastNoiseHeight = noiseHeight;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        GetComponent<MeshCollider>().sharedMesh = mesh;

        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);
        MapEmbellishments();
    }
}

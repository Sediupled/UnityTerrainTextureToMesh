using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGen4Tex : MonoBehaviour
{
    public Terrain curTer;
    public Material mat;
    public int[] TextureNumber;
    public string AssetName = "RoadGenDefault";
    TerrainData td;
    int aMapSize;
    Vector3 tSize;
    Vector3 tPos;
    int sizeX; 
    int sizeZ;

    int[,] indexMap;
    List<Vector3> vertices;
    List<int> triangles;
    Texture2D aMapTex;

    Mesh mesh;

    
    void Start()
    {
        if (curTer == null) return;

        td = curTer.terrainData;
        aMapSize = td.alphamapResolution;
        tSize = td.size;
        tPos = curTer.transform.position;
        sizeX = (int)tSize.x;
        sizeZ = (int)tSize.z;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        indexMap = new int[sizeX, sizeZ]; 
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                indexMap[x, z] = -1;
            }
        }


        foreach(int t in TextureNumber){
            addVerts(t);
        }

        DrawShape();
        SaveAsset();

    }

    void addVerts(int TextureNumber){
        aMapTex = td.alphamapTextures[(int)TextureNumber/4];

        Color[] pixels = aMapTex.GetPixels();

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {

                float normX = (float)x / sizeX;
                float normZ = (float)z / sizeZ;

                int aX = Mathf.Clamp((int)(normX * aMapSize), 0, aMapSize - 1);
                int aZ = Mathf.Clamp((int)(normZ * aMapSize), 0, aMapSize - 1);

                if (pixels[(aZ * aMapSize) + aX][TextureNumber%4] > 0.5f)
                {
                    float worldX = x + tPos.x;
                    float worldZ = z + tPos.z;
                    float worldY = td.GetInterpolatedHeight(normX, normZ) + tPos.y;

                    Vector3 localPos = transform.InverseTransformPoint(new Vector3(worldX, worldY, worldZ));
                    
                    vertices.Add(localPos);
                    indexMap[x, z] = vertices.Count - 1;
                }
            }
        }
    }

    void DrawShape(){
        for (int x = 0; x < sizeX - 1; x++)
        {
            for (int z = 0; z < sizeZ - 1; z++)
            {
                int bl = indexMap[x, z];
                int br = indexMap[x + 1, z];
                int tl = indexMap[x, z + 1];
                int tr = indexMap[x + 1, z + 1];

                if (bl >= 0 && br >= 0 && tl >= 0 && tr >= 0)
                {
                    triangles.Add(bl);
                    triangles.Add(tl);
                    triangles.Add(br);

                    triangles.Add(br);
                    triangles.Add(tl);
                    triangles.Add(tr);
                }
            }
        }
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); 
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = mat;
    }

    void SaveAsset(){
        #if UNITY_EDITOR
        AssetDatabase.CreateAsset(mesh, "Assets/MyStuff/Models/"+AssetName+".asset");
        AssetDatabase.SaveAssets();
        #endif
    }
}

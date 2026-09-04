using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGen4Tex : MonoBehaviour
{
    public Terrain curTer;
    public Material mat;
    public int TextureNumber;
    public string AssetName = "RoadGenDefault";
    
    void Start()
    {
        if (curTer == null) return;

        TerrainData td = curTer.terrainData;
        Texture2D aMapTex = td.alphamapTextures[(int)TextureNumber/4];
        int aMapSize = td.alphamapResolution;
        Vector3 tSize = td.size;
        Vector3 tPos = curTer.transform.position;

        int sizeX = (int)tSize.x;
        int sizeZ = (int)tSize.z;

        int[,] indexMap = new int[sizeX, sizeZ];
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Color[] pixels = aMapTex.GetPixels();

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                indexMap[x, z] = -1; 

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

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); 
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = mat;


        #if UNITY_EDITOR
        AssetDatabase.CreateAsset(mesh, "Assets/MyStuff/Models/"+AssetName+".asset");
        AssetDatabase.SaveAssets();
        #endif
    }
}

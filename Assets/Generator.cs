using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using B83.Image.BMP;
using UnityEngine;
using s1;


public class Generator : MonoBehaviour
{
    private double min_x = Double.MaxValue, max_x = Double.MinValue;
    private double min_y = Double.MaxValue, max_y = Double.MinValue;
    private double min_z = Double.MaxValue, max_z = Double.MinValue;
    private int SCALE = 1000;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMeshes();

    }

    // Update is called once per frame
    void Update()
    {
    }

    void GenerateMeshes()
    {
        bool is_jpg;
        var dataList = createDataList(
            "Assets/raw/");
        int i = 0;
        GameObject map = new GameObject("map");
        foreach (var data in dataList)
        {
            foreach (var curMesh in data.Meshes)
            {
                Mesh mesh = new Mesh();
                var _vertices = new List<Vector3>();
                var _uv = new List<Vector2>();
                var _indices = new List<int>();
                var _normals = new List<Vector3>();
                
                addVertices(ref _vertices, curMesh);
                addIndices(ref _indices, curMesh, 0);
                addUV(ref _uv, curMesh);
                addNormals(ref _normals, curMesh);
                
                mesh.vertices = _vertices.ToArray();
                mesh.triangles = _indices.ToArray();
                mesh.uv = _uv.ToArray();
                mesh.normals = _normals.ToArray();
                
                var gameObject = new GameObject("mesh" + i++);
                var filter = gameObject.AddComponent<MeshFilter>();
                var rend = gameObject.AddComponent<MeshRenderer>();
                filter.mesh = mesh;
                gameObject.transform.parent = map.transform;
                gameObject.transform.Rotate(-90,0,0);
                //var tex = new Texture2D((int) curMesh.TexW,(int) curMesh.TexH, TextureFormat.RGBA32, true);
                //check .bmp or .jpg
               // var texFile = File.ReadAllBytes("Assets/obj/30604060716363607-20-927/tex_"+ curMesh.Name+".bmp");
               // var bmpLoader = new BMPLoader();
               // var bmpImg = bmpLoader.LoadBMP(texFile);
               // tex = bmpImg.ToTexture2D();
                // var texFile = File.ReadAllBytes("Assets/obj/30607070716041707-20-927/tex_"+ curMesh.Name+".jpg");
                // tex.LoadImage(texFile);
                //rend.material.mainTexture = tex;
            }
            
        }
    }

    List<Data> createDataList(string prefix)
    {
        var lines = File.ReadAllLines(prefix + "FileNames.txt");
        return (from line in lines where !String.IsNullOrEmpty(line) select new Data(prefix + line)).ToList();
    }

    void addVertices(ref List<Vector3> ans, Data.CurrentMesh curMesh)
    {
        var geoCoord = curMesh.toGPS(curMesh.Vertices);
        //Координаты, по которым мы изначально искали карту
        var converter = new GeoCoordsConverter(54.7409485, 56.0211155);
        foreach (var coord in geoCoord)
        {
            var newCoords = converter.MapToScene(coord);
            ans.Add(new Vector3(newCoords.X, newCoords.Z, newCoords.Y));
        }
    }
    
    void addUV(ref List<Vector2> ans, Data.CurrentMesh curMesh)
    {
        ans.AddRange(from vertex in curMesh.Vertices let u = vertex.U let v = vertex.V select new Vector2((float) u, (float) v));
    }

    void addIndices(ref List<int> ans, Data.CurrentMesh curMesh, int count)
    {
        ans.AddRange(curMesh.Indices.Select(i => i + count));
    }
    void addNormals(ref List<Vector3> ans, Data.CurrentMesh curMesh)
    {
        ans.AddRange(from normal in curMesh.Normals let x = normal.X  let y = normal.Y  let z = normal.Z select new Vector3((float) x, (float) y, (float) z));
    }


    // List<Vector3> Center_Scale(ref List<Data.Vertice> ver)
    // {
    //     var center_x = (max_x + min_x) / 2;
    //     var center_y = (max_y + min_y) / 2;
    //     var center_z = (max_z + min_z) / 2;
    //     var distance_x = Math.Abs(max_x - min_x);
    //     var distance_y = Math.Abs(max_y - min_y);
    //     var distance_z = Math.Abs(max_z - min_z);
    //     var max_distance = Math.Max(Math.Max(distance_x, distance_y), distance_z);
    //     var SCALE = 10;
    //     var ans = new List<Vector3>();
    //     foreach (var vertex in ver)
    //     {
    //         var x = (vertex.X - center_x) / max_distance * SCALE;
    //         var y = (vertex.Y - center_y) / max_distance * SCALE;
    //         var z = (vertex.Z - center_z) / max_distance * SCALE;
    //         ans.Add(new Vector3((float) x, (float) y, (float) z));
    //     }
    //
    //     return ans;
    // }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using B83.Image.BMP;
using UnityEngine;
using s1;


public class Generator : MonoBehaviour
{

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
        string dir= "Assets/rawLouvre/";
        var dataList = createDataList(
            dir);
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
                var tex = new Texture2D((int) curMesh.TexW,(int) curMesh.TexH, TextureFormat.RGBA32, true);
                string texName = "30604060716363607-20-927";
                if (curMesh.Is_jpg)
                {
                    var texFile = File.ReadAllBytes("Assets/obj/"+texName+"/tex_"+ curMesh.Name+".jpg");
                    tex.LoadImage(texFile);
                }else
                {      
                    var texFile = File.ReadAllBytes("Assets/obj/"+texName+"/tex_"+ curMesh.Name+".bmp");
                    var bmpLoader = new BMPLoader();
                    var bmpImg = bmpLoader.LoadBMP(texFile);
                    tex = bmpImg.ToTexture2D();
                
                }
                rend.material.mainTexture = tex;
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
        var latitude = 48.861046;
        var longitude = 2.335324;
        var converter = new GeoCoordsConverter(latitude, longitude  );
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
}
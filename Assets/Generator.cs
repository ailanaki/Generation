using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using s1;
using Mesh = UnityEngine.Mesh;


public class Generator : MonoBehaviour
{
    
    Mesh mesh;
    private List<Vector3> _vertice;
    private List<int> _triangles;
    static  List<Vector2> _uv;
    static double min_x = Double.MaxValue, max_x = Double.MinValue;
    static double min_y = Double.MaxValue, max_y = Double.MinValue;
    static double min_z = Double.MaxValue, max_z = Double.MinValue;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        MakeMesh();
        mesh.Clear();
        mesh.vertices = _vertice.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.uv = _uv.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
    }

     void MakeMesh()
    {
        var dataList = createDataList(
            "/Users/aishayakupova/RiderProjects/s1/raw/");

        var sizeT = 0;
         _vertice = new List<Vector3>();
         _triangles = new List<int>();
         _uv = new List<Vector2>();
         var ver = new List<Data.Vertice>();
         foreach (var data in dataList)
        {
            var meshList = data.Meshes;
            foreach (var curMesh in meshList)
            {
                AddCurrentVertice(curMesh, ref ver);
                AddCurrentIndices(sizeT, curMesh, ref _triangles);
                sizeT += curMesh.Vertices.Count;
            }
        }
        _vertice = Center_Scale(ref ver);
    }
    
     List<Data> createDataList(string prefix)
    {
        var lines = File.ReadAllLines(prefix + "FileNames.txt");
        return (from line in lines where !String.IsNullOrEmpty(line) select new Data(prefix + line)).ToList();
    }

     static void AddCurrentVertice(Data.CurrentMesh currentMesh, ref List<Data.Vertice> ver)
     {
         foreach (var vertex in currentMesh.Vertices)
         {
             var _x = vertex.X;
             var _y =  vertex.Y;
             var _z = vertex.Z;
             ver.Add(new Data.Vertice(_x, _y, _z, vertex.U, vertex.V));
             _uv.Add(new Vector2((float) vertex.U, (float)vertex.V));
             min_x = Math.Min(_x, min_x);         
             min_y = Math.Min(_y, min_y);
             min_z = Math.Min(_z, min_z);
             max_x = Math.Max(_x, max_x);
             max_y = Math.Max(_y, max_y);
             max_z = Math.Max(_z, max_z);
         }
     }

    void AddCurrentIndices(int add, Data.CurrentMesh currentMesh, ref List<int> _triangles)
    {
        foreach (var inx in currentMesh.Indices)
        {
            _triangles.Add(inx + add);
        }
    }
    
    static List<Vector3> Center_Scale(ref List<Data.Vertice> ver)
    {
        var center_x = (max_x + min_x) / 2;
        var center_y = (max_y + min_y) / 2;
        var center_z = (max_z + min_z) / 2;
        var distance_x = Math.Abs(max_x - min_x);
        var distance_y = Math.Abs(max_y - min_y);
        var distance_z = Math.Abs(max_z - min_z);
        var max_distance = Math.Max(Math.Max(distance_x, distance_y), distance_z);
        var SCALE = 10;
        var ans = new List<Vector3>();
        foreach (var vertex in ver)
        {
            var x = (vertex.X - center_x) / max_distance * SCALE;
            var y = (vertex.Y - center_y) / max_distance * SCALE;
            var z = (vertex.Z - center_z) / max_distance * SCALE;
            ans.Add(new Vector3((float) x, (float)y, (float) z));
        }

        return ans;
    }
    
}
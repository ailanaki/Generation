using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using s1;


public class Generator : MonoBehaviour
{
    private double min_x = Double.MaxValue, max_x = Double.MinValue;
    private double min_y = Double.MaxValue, max_y = Double.MinValue;
    private double min_z = Double.MaxValue, max_z = Double.MinValue;
    private int SCALE = 10000;

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
        var dataList = createDataList(
            "/Users/aishayakupova/RiderProjects/s1/raw/");
        int i = 0;
        foreach (var data in dataList)
        {
            Mesh mesh = new Mesh();
            var _vertices = new List<Vector3>();
            var _indices = new List<int>();
            int count = 0;
            foreach (var curMesh in data.Meshes)
            {
                addVertices(ref _vertices, curMesh);
                addIndices(ref _indices, curMesh, count);
                count += curMesh.Vertices.Count;
            }

            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _indices.ToArray();
            var gameObject = new GameObject();
            gameObject.name = "mesh" + i++;
            var filter = gameObject.AddComponent<MeshFilter>() as MeshFilter;
            var rend = gameObject.AddComponent<MeshRenderer>() as MeshRenderer;
            filter.mesh = mesh;
        }
    }

    List<Data> createDataList(string prefix)
    {
        var lines = File.ReadAllLines(prefix + "FileNames.txt");
        return (from line in lines where !String.IsNullOrEmpty(line) select new Data(prefix + line)).ToList();
    }

    void addVertices(ref List<Vector3> ans, Data.CurrentMesh curMesh)
    {
        ans.AddRange(from vertex in curMesh.Vertices let x = vertex.X / SCALE let y = vertex.Y / SCALE let z = vertex.Z / SCALE select new Vector3((float) x, (float) y, (float) z));
    }

    void addIndices(ref List<int> ans, Data.CurrentMesh curMesh, int count)
    {
        ans.AddRange(curMesh.Indices.Select(i => i + count));
    }


    List<Vector3> Center_Scale(ref List<Data.Vertice> ver)
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
            ans.Add(new Vector3((float) x, (float) y, (float) z));
        }

        return ans;
    }
}
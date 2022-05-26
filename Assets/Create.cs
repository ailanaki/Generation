using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using s1;
using Mesh = UnityEngine.Mesh;

public class Create : MonoBehaviour
{
    Mesh mesh;
    private List<Vector3> _vertice;
    private List<int> _triangles;
    private Data.CurrentMesh _mesh;
    private int SCALE = 0;

    public Create(Data data, int count)
    {
        _mesh = data.Meshes[count];
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        _vertice = new List<Vector3>();
        foreach (var vertex in _mesh.Vertices)
        {
            var x = vertex.X / SCALE;
            var y = vertex.Y / SCALE;
            var z = vertex.Z / SCALE;
            _vertice.Add(new Vector3((float) x, (float) y, (float) z));
        }

        mesh.vertices = _vertice.ToArray();
        mesh.triangles = _mesh.Indices.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
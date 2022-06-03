using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using ExtraMath;
using Godot;
using Newtonsoft.Json;
using static System.Math;
using static Utils;
using Expression = System.Linq.Expressions.Expression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using static Networking;
using System.Net.NetworkInformation;
using static Globals;

public class PaintLine : MeshInstance
{
    List<Vector3> Points = new List<Vector3>();
    public Color Color;
    bool _update = false;
    SpatialMaterial mat;
    Mesh GenerateMesh()
    {
        var mesh = new ArrayMesh();
        var verts = Points.ToArray();
        Godot.Collections.Array arrmesh = new Godot.Collections.Array();
        arrmesh.Resize((int)ArrayMesh.ArrayType.Max);
        arrmesh[(int)ArrayMesh.ArrayType.Vertex] = Points.ToArray();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrmesh);
        mesh.SurfaceSetMaterial(0, mat);
        return mesh;
    }
    public void AddPoint(Vector3 point)
    {
        Points.Add(point);
        _update = true;
    }
    public override void _Process(float delta)
    {
        if(_update)
        {
            _update = false;
            Mesh = GenerateMesh();
        }
    }
    public PaintLine(Color col)
    {
        this.Color = col;
        mat = new SpatialMaterial();
        mat.FlagsUnshaded = true;
        mat.AlbedoColor = col;
    }
}
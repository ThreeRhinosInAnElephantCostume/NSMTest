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

public class MainScene : Spatial
{
    static PackedScene PackedVirtualCursor = ResourceLoader.Load<PackedScene>("res://VirtualCursor.tscn");
    public List<VirtualCursor> Cursors = new List<VirtualCursor>();
    Dictionary<int, PaintLine> _paintLines = new Dictionary<int, PaintLine>();
    int _lastPaintID = 0;
    void OnPeerAdded(NetworkedStateMachine.Peer peer)
    {
        Assert(!Cursors.Any(it => it.Peer == peer));
        var c = PackedVirtualCursor.Instance<VirtualCursor>();
        c.MS = this;
        Cursors.Add(c);
        Defer(() => AddChild(c));
        Defer(() => c.Peer = peer);
    }
    public int StartPaint(Color col, Vector3 start)
    {   
        int id = _lastPaintID++;
        var pl = new PaintLine(col);
        pl.Translation = new Vector3(0, 0.1f, 0);
        this.AddChild(pl);
        _paintLines.Add(id, pl);
        return id;
    }
    public void Paint(int id, Vector3 point)
    {
        var pl = _paintLines[id];
        pl.AddPoint(point);
    }
    public override void _Ready()
    {
        Client.OnPeerAdded += p => 
        {
            OnPeerAdded(p);
        };
        Client.Peers.ForEach(peer => OnPeerAdded(peer));
        Globals.OwnCursor = PackedVirtualCursor.Instance<VirtualCursor>();
        OwnCursor.MS = this;
        AddChild(OwnCursor);
        Defer(() => 
        {
            OwnCursor.IDLabel.Text = $"[{Globals.Name}]";
        });
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }
}

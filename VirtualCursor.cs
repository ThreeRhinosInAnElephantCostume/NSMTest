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
using static TestClient;

public class VirtualCursor : Spatial
{
    NetworkedStateMachine.Peer _peer;
    public Label IDLabel;
    public MainScene MS;
    MeshInstance _mesh;
    Color _color;
    void UpdateLabel()
    {
        if(_peer == null)
            return;
        if(Globals.Client.names.ContainsKey(_peer.ID))
        {
            IDLabel.Text = $"({_peer.ID})" +  Globals.Client.names[_peer.ID];
        }
    }
    int _paintID;
    bool _painting = false;
    Vector3 _lastPaint;
    public void TickPaiting(bool IsPainting, Vector3 pos)
    {
        if(IsPainting)
        {
            if(_painting)
            {
                _lastPaint = pos;
                MS.Paint(_paintID, pos);
            }   
            else
            {
                _painting = true;
                _lastPaint = pos;
                _paintID = MS.StartPaint(_color, pos);
            }
        }
        else
        {
            _painting = false;
        }
    }
    bool SubscribtionDelegate(NetworkedStateMachine.Subscribtion sub, NetworkedStateMachine.ReceivedMessage message)
    {
        Assert(message.Message is CursorMotionNM);
        var cm = (CursorMotionNM)message.Message;
        this.Translation = cm.pos;
        UpdateLabel();
        TickPaiting(cm.IsPainting, cm.pos);
        return true;
    }
    public NetworkedStateMachine.Peer Peer
    {
        get => _peer;
        set
        {
            _peer = value;
            Defer(() => 
            {
                var rng = new RNG((ulong)(Peer.ID*1000+666));
                Color col = new Color(rng.NextFloat(), rng.NextFloat(), rng.NextFloat()*0.5f+0.5f);
                var mat = new SpatialMaterial();
                mat.AlbedoColor = col;
                _mesh.MaterialOverride = mat;
                _color = col;
            });
            Client.Subscribe<CursorMotionNM>(_peer, (sub, message) => 
                {Defer(() => SubscribtionDelegate(sub, message)); return true;});
        }
    }
    public override void _Ready()
    {
        IDLabel = this.GetNodeSafe<Label>("Viewport/Control/Label");
        _mesh = this.GetNodeSafe<MeshInstance>("MeshInstance");
        Defer(UpdateLabel);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

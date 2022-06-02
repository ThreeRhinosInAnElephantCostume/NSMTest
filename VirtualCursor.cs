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
    Label _IDLabel;
    MeshInstance _mesh;
    bool SubscribtionDelegate(NetworkedStateMachine.Subscribtion sub, NetworkedStateMachine.ReceivedMessage message)
    {
        Assert(message.Message is CursorMotionNM);
        var cm = (CursorMotionNM)message.Message;
        this.Translation = cm.pos;
        if(Globals.Client.names.ContainsKey(_peer.ID))
        {
            _IDLabel.Text = $"({_peer.ID})" +  Globals.Client.names[_peer.ID];
        }
        return true;
    }
    public NetworkedStateMachine.Peer Peer
    {
        get => _peer;
        set
        {
            _peer = value;
            Defer(() => _IDLabel.Text = "ID: " + _peer.ID.ToString());
            Defer(() => 
            {
                var rng = new RNG((ulong)(Peer.ID*1000+666));
                Color col = new Color(rng.NextFloat(), rng.NextFloat(), rng.NextFloat()*0.5f+0.5f);
                var mat = new SpatialMaterial();
                mat.AlbedoColor = col;
                _mesh.MaterialOverride = mat;
            });
            Client.Subscribe<CursorMotionNM>(_peer, (sub, message) => 
                {Defer(() => SubscribtionDelegate(sub, message)); return true;});
        }
    }
    public override void _Ready()
    {
        _IDLabel = this.GetNodeSafe<Label>("Viewport/Control/Label");
        _mesh = this.GetNodeSafe<MeshInstance>("MeshInstance");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

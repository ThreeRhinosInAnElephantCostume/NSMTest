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

public class TestClient : NetworkedStateMachine
{
    public struct CursorMotionNM
    {
        public long id;
        public Vector3 pos;
    }
    public struct NameMM
    {
        public long id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string name;
    }
    public Action<string> OnOutput = s => {};
    public Action OnConnectionSuccessful = () => {};
    public Action OnConnectionFailure = () => {};
    public Dictionary<long, string> names = new Dictionary<long, string>();
    string _name;
    protected override async void Logic()
    {
        try
        {
            AwaitConnection().Wait();
        }
        catch(NetConnectionErrorException ex)
        {
            OnConnectionLost(ex);
        }
        OnOutput("connected");
        Console.WriteLine("connected");
        OnConnectionSuccessful();
        while(true)
        {
            var othername = await AwaitMessage<NameMM>(null, -1);
            if(!names.ContainsKey(othername.id))
                names.Add(othername.id, othername.name);
        }
    }
    protected override void OnConnectionLost(NetConnectionErrorException ex)
    {
        OnConnectionFailure();
    }
    public void SendCursorPos(Vector3 pos)
    {
        this.SendToAll<CursorMotionNM>(new CursorMotionNM()
        {
            id = this.ID, 
            pos = pos,
        }, false, true);
    }
    public TestClient(IPEndPoint relay, string password, string name) : base(relay, password, false)
    {
        _name = name;
        this.OnPeerAdded += (Peer p) => 
        {
            Send<NameMM>(p, new NameMM()
            {
                id = ID,
                name = _name
            });
        };
    }
}
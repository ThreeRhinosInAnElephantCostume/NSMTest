using System.Net;
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
using static Globals;

public class LogIn : Control
{
    Button _connectButton;
    Button _hostButton;
    LineEdit _IPEdit;
    LineEdit _PortEdit;
    LineEdit _nameEdit;
    Label _outputLabel;
    List<string> output = new List<string>();
    public void OutputMessage(string msg)
    {
        output.Add(msg);
        _outputLabel.Text = string.Join('\n', output);
    }
    IPEndPoint TryParseAddress()
    {
        string ipstr = _IPEdit.Text;
        string portstr = _PortEdit.Text;
        IPAddress ip = null;
        if(!IPAddress.TryParse(ipstr, out ip))
        {
            OutputMessage("Could not parse IP!");
            return null;
        }
        ushort port = 0;
        if(!ushort.TryParse(portstr, out port))
        {
            OutputMessage("Could not parse port!");
            return null;
        }
        return new IPEndPoint(ip, port);
    }
    void OnConnectPressed()
    {
        IPEndPoint endpoint = TryParseAddress();
        if(endpoint == null)
            return;
        Client = new TestClient(endpoint, _nameEdit.Text, PASSWORD);
        Client.OnConnectionFailure = () => 
        {
            Defer(() => {OutputMessage("Connection failure!");});
        };
        Client.OnConnectionSuccessful = () => 
        {
            Defer(() => 
            {
                OutputMessage("Connected!");
                _hostButton.Disabled = true;
                _connectButton.Disabled = true;
                Delay(1, () => 
                {
                    GetTree().ChangeScene("res://MainScene.tscn");
                });
            });
        };
    }
    void OnHostPressed()
    {
        _IPEdit.Text = "127.0.0.1";
        IPEndPoint endpoint = TryParseAddress();
        if(endpoint == null)
            return;
        IsHost = true;
        Relay = new Networking.Relay((ushort)endpoint.Port, PASSWORD, PASSWORD);
        OnConnectPressed();
    }
    public override void _Ready()
    {
        _hostButton = GetNode<Button>("CenterContainer/VBoxContainer/HostButton");
        _connectButton = GetNode<Button>("CenterContainer/VBoxContainer/ConnectButton");
        _IPEdit = GetNode<LineEdit>("CenterContainer/VBoxContainer/IPEdit");
        _PortEdit = GetNode<LineEdit>("CenterContainer/VBoxContainer/PortEdit");
        _nameEdit = GetNode<LineEdit>("CenterContainer/VBoxContainer/NameEdit");
        _outputLabel = GetNode<Label>("CenterContainer/VBoxContainer/Label");
        _connectButton.OnButtonPressed(OnConnectPressed);
        _hostButton.OnButtonPressed(OnHostPressed);
    }
}

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
    LineEdit _IPEdit;
    LineEdit _PortEdit;
    Label _outputLabel;
    List<string> output = new List<string>();
    public void OutputMessage(string msg)
    {
        output.Add(msg);
        _outputLabel.Text = string.Join('\n', output);
    }
    void OnConnectPressed()
    {
        string ipstr = _IPEdit.Text;
        string portstr = _PortEdit.Text;
        IPAddress ip = null;
        if(!IPAddress.TryParse(ipstr, out ip))
        {
            OutputMessage("Could not parse IP!");
            return;
        }
        ushort port = 0;
        if(!ushort.TryParse(portstr, out port))
        {
            OutputMessage("Could not parse port!");
            return;
        }
        Client = new TestClient(new IPEndPoint(ip, port));
        Client.OnConnectionFailure = () => 
        {
            Defer(() => {OutputMessage("Connection failure!");});
        };
        Client.OnConnectionSuccessful = () => 
        {
            Defer(() => 
            {
                OutputMessage("Connected!");
                
            });
        };
    }
    public override void _Ready()
    {
        _connectButton = GetNode<Button>("CenterContainer/Panel/VBoxContainer/ConnectButton");
        _IPEdit = GetNode<LineEdit>("CenterContainer/Panel/VBoxContainer/HBoxContainer/IPEdit");
        _PortEdit = GetNode<LineEdit>("CenterContainer/Panel/VBoxContainer/HBoxContainer/PortEdit");
        _outputLabel = GetNode<Label>("CenterContainer/Panel/VBoxContainer/Label");
        _connectButton.OnButtonPressed(OnConnectPressed);
    }
}

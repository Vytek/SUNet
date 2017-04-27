using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Client : SUNet {

    public string serverIP = "127.0.0.1";
    public int serverport = 8000;

    private IPEndPoint ServerConnection;

    private bool GetAsync = false;
    private string APing = "EMPTY";
    private string SPing = "EMPTY";
    private double APingTotal = 0;
    private long APingCount = 0;
    private double SPingTotal = 0;
    private long SPingCount = 0;

    //--------------------------------------------------------------------
    void Start () {
        StartConnection();
        ServerConnection = new IPEndPoint(IPAddress.Parse(serverIP), serverport);
        InvokeRepeating("Pinger", 2, 0.1f);
	}

    //--------------------------------------------------------------------
    void Pinger() {
        NetMessage nm = new NetMessage();
        nm.Push(DateTime.UtcNow);
        SendReliableMessageTo(ServerConnection, nm);
	}


    //--------------------------------------------------------------------
    protected override void OnMessageReceivedAsync(IPEndPoint fromUser, NetMessage data, bool reliable) {
        if (GetAsync) {
            double totMs = DateTime.UtcNow.Subtract(data.PopDatetime()).TotalMilliseconds;
            APingTotal += totMs;
            ++APingCount;
            APing = (float)(APingTotal/APingCount) +"  -  "+ (float)totMs;
        }
        else {
            PushToMainThread(fromUser, data);
        }
        GetAsync = !GetAsync;
    }

    //--------------------------------------------------------------------
    protected override void OnMessageReceivedSync(IPEndPoint fromUser, NetMessage data) {
        double totMs = DateTime.UtcNow.Subtract(data.PopDatetime()).TotalMilliseconds;
        SPingTotal += totMs;
        ++SPingCount;
        SPing = (float)(SPingTotal/SPingCount) +"  -  "+ (float)totMs;
    }

    //--------------------------------------------------------------------
    private void OnGUI() {
        GUI.Label(new Rect(10, 10, 256, 24), APing);
        GUI.Label(new Rect(10, 40, 256, 24), SPing);
    }
}

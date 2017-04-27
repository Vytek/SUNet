using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Server : SUNet {

    public int port = 8000;

    //--------------------------------------------------------------------
    void Start () {
        StartConnection(port);
	}

    //--------------------------------------------------------------------
    protected override void OnMessageReceivedAsync(IPEndPoint fromUser, NetMessage data, bool reliable) {
        if (reliable)
            SendReliableMessageTo(fromUser, data);
        else
            SendUnreliableMessageTo(fromUser, data); //Echo back
    }

    //--------------------------------------------------------------------
    protected override void OnMessageReceivedSync(IPEndPoint fromUser, NetMessage data) {
        Debug.Log("This is being called");
    }

}

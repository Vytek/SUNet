# SUNet
Simple and fast UDP Networking for Unity


## Features

* Lightweight
  * Small CPU and RAM usage
  * Small packet size overhead
  * Custom encapsulation of messages into byte array
  * Receive very low latency Async Messages  ( < 1ms)
  * Push Async Messages to Main Thread very fast ( ~ 2.5ms)
  
* Added functionality to UDP
  * Reliable messages (Not Ordered)
  * Unrealiable Messages
  * Low Latency for both types of messages
  * Very simple and easy to use
  
Does not use Unity's LLAPI, custom .NET Sockets solution.

## Usage samples

### Server
```csharp
//Inherit from SUNet
public class Server : SUNet {

    public int port = 8000;

    //--------------------------------------------------------------------
    void Start () {
        //Starts a connection on the local machine listening at 'port'
        StartConnection(port);
    }

    //--------------------------------------------------------------------
    //Receive Async Messages, push to main thread from here
    protected override void OnMessageReceivedAsync(IPEndPoint fromUser, NetMessage data, bool reliable) {
        if (reliable)
            SendReliableMessageTo(fromUser, data); //Echo Reliable back
        else
            SendUnreliableMessageTo(fromUser, data); //Echo Unreliable back
        
        //Pushes data into main thread receiver -> OnMessageReceivedSync
        //PushToMainThread(fromUser, data);
    }

    //--------------------------------------------------------------------
    protected override void OnMessageReceivedSync(IPEndPoint fromUser, NetMessage data) {
        //Is is in main Thread
    }

}
```
### Client
```csharp
//Simple Client that shows immediate and average ping times for reliable messages in both Asycn and Main Thread
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
        //Start a connection on a system asigned port
        StartConnection();
        
        //Connection to specific target (Server)
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
        else  PushToMainThread(fromUser, data);
        
        GetAsync = !GetAsync; // Keep changing between Async messages and Main Thread
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
```


## Comparisson tests


### rUDP and Latency 
* Normal Unreliable latency is 0.5ms on average.
* Normal Reliable latency is 2.5ms on average.
* Running a 50% packet loss on both Server and Client sides, brings up the average latency to 80 ms

Keep in mind that this is the time taken for a message to leave the client, get echoes back from the server and pushes to main thread in both sides until it is actually read on main thread.



### NetMessage serlialization
* NetMessage "Byterization" takes 7.3% of the total time on average vs normal serialization
* NetMessage "Byterization" wraps bytes into 20.8% of the total bytes on average vs normal serialization

Tested against several different types of parameters and done over and over millions of times to get a more accurate comparisson.

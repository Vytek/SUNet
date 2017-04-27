using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public abstract class SUNet : MonoBehaviour {


    #region Private_Classes

    //*************************************************************
    private struct NMessage {
        public IPEndPoint endPoint;
        public NetMessage bfData;
    }

    //*************************************************************
    private class MessageBuffer {

        public bool hasPendingMessages = false;
        private int _bufferSize;
        private NMessage[] _buffers;
        private int _curPushIndex;
        private int _curReadIndex;

        //-----------------------------------------------------------------------------------------
        public MessageBuffer(int bufferSize = 1024) {
            _bufferSize = bufferSize;
            _buffers = new NMessage[_bufferSize];
            hasPendingMessages = false;
        }

        //-----------------------------------------------------------------------------------------
        public void AddMsg(IPEndPoint client, NetMessage bf) {
            _buffers[_curPushIndex].bfData = bf;
            _buffers[_curPushIndex].endPoint = client;
            ++_curPushIndex;

            if (_curPushIndex >= _bufferSize) {
                _curPushIndex = 0;
            }
            hasPendingMessages = true;
        }

        //-----------------------------------------------------------------------------------------
        public bool GetNextMsg(out NMessage nm) {
            nm = _buffers[_curReadIndex];
            if (_curPushIndex == _curReadIndex) {
                hasPendingMessages = false;
                return false;
            }

            ++_curReadIndex;
            if (_curReadIndex >= _bufferSize) {
                _curReadIndex = 0;
            }
            return true;
        }

    }//Class Message Buffer

    //*************************************************************
    private enum RU_MessageType : byte {
        UNRELIABLE,
        RELIABLE,
        ACK
    }

    //*************************************************************
    private struct RU_Message {
        public UInt32 id;
        public DateTime time;
        public IPEndPoint source;
        public NetMessage data;
    }

    #endregion

    public int ListeningPort { get; private set; }

    protected UdpClient _socket;
    protected bool isRunning;

    private MessageBuffer _MBuffer;
    private List<RU_Message> RUMessages;
    private HashSet<UInt64> alreadyReceived;
    private UInt32 nmID = 0;

    //-----------------------------------------------------------------------------------------
    protected void StartConnection(int listenPort = 0, int bufferSize = 1024) {
        if (isRunning) {
            Debug.LogError("Connection already Started at : "+ ListeningPort);
            return;
        }
        ListeningPort = listenPort;
        _socket = new UdpClient(ListeningPort, AddressFamily.InterNetwork);
        _MBuffer = new MessageBuffer(bufferSize);
        RUMessages = new List<RU_Message>();
        alreadyReceived = new HashSet<UInt64>();
        isRunning = true;
        Receive();
        StartCoroutine(StartReadingMessages());
        StartCoroutine(ResendRelibaleMessages());
    }

    //-----------------------------------------------------------------------------------------
    protected void Receive() {
        _socket.BeginReceive(new AsyncCallback(ReceivedCallback), _socket);
    }

    //-----------------------------------------------------------------------------------------
    private void ReceivedCallback(IAsyncResult result) {
        UdpClient clientSocket = result.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        byte[] buff = clientSocket.EndReceive(result, ref source);

        ManageReliableUnreliable(source, buff);
        
        _socket.BeginReceive(new AsyncCallback(ReceivedCallback), _socket);
    }

    //-----------------------------------------------------------------------------------------
    public void SendUnreliableMessageTo(IPEndPoint target, NetMessage data) {
        data.PushFront((byte)RU_MessageType.UNRELIABLE);
        _socket.BeginSend(data.ToArray(), data.GetLength(), target, new AsyncCallback(SentCallback), _socket);
    }

    //-----------------------------------------------------------------------------------------
    public void SendReliableMessageTo(IPEndPoint target, NetMessage data) {
        data.PushFront(++nmID);
        data.PushFront((byte)RU_MessageType.RELIABLE);
        AddRUMessage(nmID, target, data);
        _socket.BeginSend(data.ToArray(), data.GetLength(), target, new AsyncCallback(SentCallback), _socket);
    }

    //-----------------------------------------------------------------------------------------
    protected void SentCallback(IAsyncResult result) {
        UdpClient target = (UdpClient)result.AsyncState;
        target.EndSend(result);
    }

    //-----------------------------------------------------------------------------------------
    protected void PushToMainThread(IPEndPoint client, NetMessage data) {
        _MBuffer.AddMsg(client, data);
    }

    //-----------------------------------------------------------------------------------------
    private void ManageReliableUnreliable(IPEndPoint source, byte[] data) {
        NetMessage nm = new NetMessage(data, 1, true);
        switch ((RU_MessageType)data[0]) {
            case RU_MessageType.RELIABLE:
                ManageReliable(source, nm);
                break;
            case RU_MessageType.ACK:
                ManageAck(nm);
                break;
            case RU_MessageType.UNRELIABLE:
                ManageUnreliable(source, nm);
                break;
            default:
                Debug.LogError("Wrong Message Type, possibly corrupt info");
                break;
        }
    }

    //-----------------------------------------------------------------------------------------
    private void ManageReliable(IPEndPoint source, NetMessage nm) {
        UInt32 id = nm.PopUInt32();
        int sourceHC = source.GetHashCode();

        NetMessage tmp = new NetMessage();
        tmp.Push(sourceHC);
        tmp.Push(id);
        UInt64 id64 = tmp.PopUInt64();
        if (alreadyReceived.Contains(id64)) return;

        tmp.Push(sourceHC);
        tmp.Push(id-1000);
        UInt64 id641k = tmp.PopUInt64();
        if (alreadyReceived.Contains(id641k)) alreadyReceived.Remove(id641k);//Only keep 1000 messages in pool
        alreadyReceived.Add(id64);

        tmp = new NetMessage();
        tmp.Push((byte)RU_MessageType.ACK);
        tmp.Push(id);
        _socket.BeginSend(tmp.ToArray(), tmp.GetLength(), source, new AsyncCallback(SentCallback), _socket);

        NetMessage tmp2 = new NetMessage(nm, 5);
        OnMessageReceivedAsync(source, tmp2, true);
    }

    //-----------------------------------------------------------------------------------------
    private void ManageAck(NetMessage nm) {
        UInt32 nmID = nm.PopUInt32();
        RemoveRUMessage(nmID);
    }

    //-----------------------------------------------------------------------------------------
    private void ManageUnreliable(IPEndPoint source, NetMessage nm) {
        NetMessage tmp = new NetMessage(nm, 1);
        OnMessageReceivedAsync(source, tmp, false);
    }

    //-----------------------------------------------------------------------------------------
    private void AddRUMessage(UInt32 nmID, IPEndPoint source, NetMessage data) {
        RU_Message tmp = new RU_Message();
        tmp.time = DateTime.UtcNow;
        tmp.id = nmID;
        tmp.source = source;
        tmp.data = data;
        RUMessages.Add(tmp);
    }

    //-----------------------------------------------------------------------------------------
    private void RemoveRUMessage(UInt32 nmID) {
        for(int i=0; i<RUMessages.Count; ++i) {
            if (RUMessages[i].id == nmID) {
                RUMessages.RemoveAt(i);
                return;
            }
            else if (RUMessages[i].id > nmID) return;
        }
    }


    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    protected IEnumerator StartReadingMessages() {
        bool continueReading = true;
        NMessage nm;
        while (isRunning) {
            continueReading = _MBuffer.GetNextMsg(out nm);
            while (continueReading) {
                OnMessageReceivedSync(nm.endPoint, nm.bfData);
                continueReading = _MBuffer.GetNextMsg(out nm);
            }
            yield return new WaitUntil(() => _MBuffer.hasPendingMessages);
        }
    }

    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    protected IEnumerator ResendRelibaleMessages() {
        while (isRunning) {
            double totalMS = 0;
            for(int i=0; i<RUMessages.Count; ++i) {
                try {
                    totalMS = DateTime.UtcNow.Subtract(RUMessages[i].time).TotalMilliseconds;
                    if (totalMS < 50) continue;
                    _socket.BeginSend(RUMessages[i].data.ToArray(), RUMessages[i].data.GetLength(), RUMessages[i].source, new AsyncCallback(SentCallback), _socket);
                    if (totalMS > 1000) {
                        RUMessages.RemoveAt(i);
                        --i;
                    }
                }
                catch (Exception) { /*Most likely an index out of range due to Async remove*/ }
            }
            yield return new WaitForFixedUpdate();
        }
    }



    //.........................................................................................
    protected abstract void OnMessageReceivedAsync(IPEndPoint source, NetMessage data, bool reliable);
    protected abstract void OnMessageReceivedSync(IPEndPoint source, NetMessage data);



    //-----------------------------------------------------------------------------------------
    private void OnDestroy() {
        _socket.Close();
    }
    
}

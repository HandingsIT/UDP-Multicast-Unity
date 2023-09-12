using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public struct UdpState
{
    public UdpClient udpClient;
    public IPEndPoint endPoint;
}

public class MultiCastReciever : IDisposable
{
    public string MulticastIP = "239.225.0.3";
    public int MulticastPort = 5500;
    public string MyIP = string.Empty;
    public bool IsMe = true;

    UdpClient _udp = null;
    IPEndPoint _localEP = null;
    int _recievedNum = 0;

    // Thread safe queue
    ConcurrentQueue<byte[]> _pckQueue = new ConcurrentQueue<byte[]>();

    public int RecieveCount { get => _pckQueue.Count; }
    public int RecievedNum { get => _recievedNum; }

    private int cancelOverflowQueueNum = 5;

    public bool GetTop( out byte[] bytePakcetArray )
    {
        bool retvalue = false;
        bytePakcetArray = null;
        if (_pckQueue.Count > 0)
        {
            byte[] msg;
            if (_pckQueue.TryDequeue(out msg))
            {
                retvalue = true;
                bytePakcetArray = msg;
            }
        }

        return retvalue;
    }

    int _packetPerSecond = 0;
    public int PacketPerSecond { get => _packetPerSecond; }
    public void Call1000msecForPacketPerSecond()
    {
        _packetPerSecond = _recievedNum;
        _recievedNum = 0;
    }

    ILogger _logger;

    public MultiCastReciever(ILogger logger)
    {
        _logger = logger;

        Bind(MulticastIP, MulticastPort, MyIP);
    }

    public MultiCastReciever(string GroupIP, string myIP, int Port, bool isMe, ILogger logger)
    {
        _logger = logger;

        MulticastIP = GroupIP;
        MulticastPort = Port;
        IsMe = isMe;
        Bind(MulticastIP, MulticastPort, myIP);
    }

    public void Dispose()
    {
        if (_udp != null)
        {
            _udp.Client.Shutdown(SocketShutdown.Both);
        }
    }

    public void Bind(string groupIP, int Port , string myIP)
    {
        _udp = new UdpClient();

        _localEP = new IPEndPoint(IPAddress.Any, Port);
        _udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _udp.ExclusiveAddressUse = false;
        _udp.Client.Bind(_localEP);

        IPAddress multicastaddress = IPAddress.Parse(groupIP);
        if (!string.IsNullOrEmpty(myIP))
        {
            IPAddress myIpAdress = IPAddress.Parse(myIP);
            _udp.JoinMulticastGroup(multicastaddress, myIpAdress);
        }
        else
        {
            _udp.JoinMulticastGroup(multicastaddress);
        }
        ReceiveMessages();
    }

    public void ReceiveMessages()
    {
        UdpState udpState = new UdpState();
        udpState.endPoint = _localEP;
        udpState.udpClient = _udp;

        _udp.BeginReceive(new AsyncCallback(ReceiveCallback), udpState);
    }

    public void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient udpclient = ((UdpState)(ar.AsyncState)).udpClient;
        IPEndPoint endpoint = ((UdpState)(ar.AsyncState)).endPoint;

        byte[] receiveBytes = udpclient.EndReceive(ar, ref endpoint);

        if (_pckQueue.Count < cancelOverflowQueueNum)
        {
           //  _logger.Log($"{nameof(ReceiveCallback)}[{MulticastIP},{MulticastPort}]: {receiveBytes.Length}");

            _pckQueue.Enqueue(receiveBytes);

            _recievedNum++;
        }

        ReceiveMessages();
    }

}

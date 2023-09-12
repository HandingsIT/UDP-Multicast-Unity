using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MultiCastSender : IDisposable
{
    // public 
    public string MulticastIP = "239.225.0.3";
    public int MulticastPort = 5500;

    public float SendInterval = 0.05f;
    public bool WantSending = true;

    // private 
    UdpClient _udp = null;
    bool _msgSending = false;
    IPEndPoint _remoteEP = null;

    // Thread safe queue
    ConcurrentQueue<byte[]> _pckQueue = new ConcurrentQueue<byte[]>();

    // Unity Logger
    ILogger _logger;

    public MultiCastSender( ILogger logger )
    {
        _logger = logger;

        Connect(MulticastIP, MulticastPort);
    }

    public MultiCastSender(string ip, int port, ILogger logger)
    {
        _logger = logger;

        MulticastIP = ip;
        MulticastPort = port;
        Connect(MulticastIP, MulticastPort);
    }

    public void Connect(string ip, int port)
    {
        _udp = new UdpClient();

        IPAddress groupAddress = IPAddress.Parse(ip);
        //        _udp.JoinMulticastGroup(groupAddress);

        _remoteEP = new IPEndPoint(groupAddress, port);
    }

    public void Dispose()
    {
        Disconnect();

        _udp.Client.Shutdown(SocketShutdown.Both);
    }

    public void Disconnect()
    {
        _udp.Close();
    }

    public void SendToQueue(byte[] pckMessage)
    {
        if (pckMessage != null && pckMessage.Length > 0)
        {
            _pckQueue.Enqueue(pckMessage);
        }
    }

    void SendCallback(IAsyncResult ar)
    {
        UdpClient udp = (UdpClient)ar.AsyncState;
        int sendlength = udp.EndSend(ar);
        _msgSending = false;
    }

    public void UpdSendWithQueue()
    {
        if (WantSending && _pckQueue.Count > 0 && !_msgSending)
        {
            byte[] msg;
            if (_pckQueue.TryDequeue(out msg))
            {
                _msgSending = true;

                _logger.Log($"{nameof(UpdSendWithQueue)}[{MulticastIP},{MulticastPort}]: {msg.Length}");

                _udp.BeginSend(msg, msg.Length, _remoteEP, SendCallback, _udp);
            }
        }
    }


}

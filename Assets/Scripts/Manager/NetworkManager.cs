using UnityEngine;

public class PacketRecieveEventArgs
{
    public PacketRecieveEventArgs(byte[] bytePacket, CasterID casterID)
    {
        BytePacket = bytePacket;
        CasterID = casterID;
    }

    public byte[] BytePacket { get; }
    public CasterID CasterID { get; }
}


public class NetworkManager : MonoBehaviourSingleton<NetworkManager>
{
    private const int OneSecMilliSeconds = 1000;

    public delegate void PacketRecievedHandler(object sender, PacketRecieveEventArgs e);
    public event PacketRecievedHandler PacketRecieved;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SendPerOneSec), 1.0f, 1.0f);
        InvokeRepeating(nameof(PacketHandle), 1.0f, 0.005f);
    }
    
    void PacketHandle()
    {
        // Send Packet all
        Multicaster.Instance.MultiCastSenderManager.UpdSendWithQueue();

        // Get queue from udp
        foreach (var recieverInfo in Multicaster.Instance.MultiCastRecieverManager)
        {
            var reciever = recieverInfo.Value;

            while (reciever.GetTop(out byte[] bytePacket))
            {
                if (PacketRecieved != null)
                {
                    PacketRecieved.Invoke(this, new PacketRecieveEventArgs(bytePacket, recieverInfo.Key));
                }
            }
        }

    }


    void SendPerOneSec()
    {
        int tickCount = OneSecMilliSeconds;

        SendOperationStatus(tickCount);
    }

    private static void SendOperationStatus(int tickCount)
    {
        Debug.Log($"{nameof(SendOperationStatus)}:{GlobalState.Instance.OperationStatus}");
        
        var utf8bytesPakcet = PacketFactory.Instance.GetPacketOperationStatus(GlobalState.Instance.OperationStatus, "channel", tickCount);
        Multicaster.Instance.SendToQueue(CasterID.TCS, utf8bytesPakcet);
    }

}

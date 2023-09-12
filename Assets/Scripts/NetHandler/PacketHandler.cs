using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PacketHandler : MonoBehaviour, IPacketHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        NetworkManager.Instance.PacketRecieved += Instance_PacketRecieved;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.PacketRecieved -= Instance_PacketRecieved;
    }

    private void Instance_PacketRecieved(object sender, PacketRecieveEventArgs e)
    {
        byte[] packetBytes = e.BytePacket;
        if (packetBytes != null)
        {
            // Bytes --> String
            string jsonPacket = Encoding.UTF8.GetString(packetBytes);

            // Convert to Default Packet
            PacketBase packetBase = JsonUtility.FromJson<PacketBase>(jsonPacket);
            if (packetBase != null)
            {
                // Find Caster ID
                CasterID sendCasterId = CasterID.Unknown;
                Enum.TryParse(packetBase.sender, out sendCasterId);

                // Find Real Packet
                PacketName packetName;
                if (Enum.TryParse( packetBase.name, out packetName))
                {
                    switch (packetName)
                    {
                        case PacketName.MGS:
                            PacketOperationStatus packetOPS = JsonUtility.FromJson<PacketOperationStatus>(jsonPacket);
                            OnPacketOperationStatus(sendCasterId, packetOPS);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }


    // ------------------------------------------------  OPS  --------------------------------------------------------------
    public void OnPacketOperationStatus(CasterID casterId, PacketOperationStatus packet)
    {
        GlobalState.Instance.OperationalState = packet.data;

        Debug.Log($"{nameof(PacketHandler)}:{casterId}, {packet.name}");
    }
}

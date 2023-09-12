using System;
using System.Collections.Generic;
using System.Data;

public class PacketFactory : MonoBehaviourSingleton<PacketFactory>
{
    static int _packetSerial = 1000;
    int PacketSerial { get { return _packetSerial++; } }

    private void FillHeader(PacketName packetName, SystemModule myModule, SystemModule receiveModule, int deviceIndex, PacketBase packet, int tickSeconds)
    {
        packet.name = packetName.ToString();
        packet.unique = Guid.NewGuid().ToString();
        packet.version = 1;
        packet.sender = myModule.ToString();
        packet.receiver = receiveModule.ToString();
        packet.deviceIndex = deviceIndex;
        packet.serial = PacketSerial;
        packet.tick = tickSeconds;
        packet.dateTime = DateTime.UtcNow.ToString("o");
    }

    public byte[] GetPacketOperationStatus(MyGameStatus operationStatus, string channel, int tickSeconds)
    {

        PacketOperationStatus packet = new PacketOperationStatus();

        // Set Packet Base
        FillHeader(PacketName.MGS, SystemModule.GS, SystemModule.All, 0, packet, tickSeconds);

        // Set Packet Data
        packet.data.status = operationStatus.ToString();
        packet.data.channel = channel;

        return packet.ToUtf8Bytes();
    }

    public byte[] GetPacketSystemControl(TargetType targetType, Command command, SystemControlState systemControllState, string uniqueCode, int tickSeconds)
    {

        PacketSystemControl packet = new PacketSystemControl();

        // Set Packet Base
        FillHeader(PacketName.SC, SystemModule.GS, SystemModule.All, 0, packet, tickSeconds);

        //Set Packet Data
        packet.data.code = uniqueCode;
        packet.data.target = targetType.ToString() ;
        packet.data.command = command.ToString();
        packet.data.state = systemControllState.ToString() ;

        return packet.ToUtf8Bytes();
    }
}

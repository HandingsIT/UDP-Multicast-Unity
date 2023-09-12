using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketHandler 
{
    // UDP
    void OnPacketOperationStatus(CasterID casterId, PacketOperationStatus packet);

}

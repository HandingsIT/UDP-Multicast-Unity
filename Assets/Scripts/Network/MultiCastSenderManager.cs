using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiCastSenderManager : Dictionary<CasterID, MultiCastSender>
{
    private ILogger Debug = null;

    public MultiCastSenderManager(ILogger debug)
    {
        Debug = debug;
    }

    public void Attach(CasterID casterID, string address, int port)
    {
        if (ContainsKey(casterID))
        {
            Detach(casterID);
        }

        MultiCastSender caster = null;

        try
        {
            caster = new MultiCastSender(address, port, Debug);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            caster = null;
        }

        if (caster != null)
        {
            Add(casterID, caster);
        }
    }

    public void Detach(CasterID casterID)
    {
        if (ContainsKey(casterID))
        {
            var caster = this[casterID];
            if (caster != null)
            {
                caster.Dispose();
            }

            this.Remove(casterID);
        }
    }

    public void SendToQueue(CasterID target, byte[] pckMessage)
    {
        if (ContainsKey(target))
        {
            this[target].SendToQueue(pckMessage);
        }
    }

    public void UpdSendWithQueue()
    {
        foreach (var sender in this)
        {
            sender.Value.UpdSendWithQueue();
        }
    }
}

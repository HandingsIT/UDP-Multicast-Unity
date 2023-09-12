using System;
using System.Collections.Generic;
using UnityEngine;


public class MultiCastRecieverManager : Dictionary<CasterID, MultiCastReciever>
{
    private ILogger Debug = null;

    public MultiCastRecieverManager(ILogger debug)
    {
        Debug = debug;
    }

    public void Attach(CasterID casterID, string groupAddress, int port, string myAddress, bool isMe)
    {
        if (ContainsKey(casterID))
        {
            Detach(casterID);
        }

        MultiCastReciever caster = null;

        try
        {
            caster = new MultiCastReciever(groupAddress,myAddress, port, isMe, Debug);
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
}

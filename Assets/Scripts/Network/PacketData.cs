using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

// NuGet > Install System.Text.Json
// https://docs.microsoft.com/ko-kr/dotnet/standard/serialization/system-text-json-overview
// https://docs.microsoft.com/ko-kr/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0

[Serializable]
public enum SystemModule
{
    None,
    All,    // 모든 시스템
    GS,     // Game System ( 게임 시스템 )
    TSC     // Training System Controll
}

[Serializable]
public enum PacketName
{
    None,
    MGS,    // My Game Status
    SC,     // System Controll
}

public interface IPacket
{
    string ToJson(bool intented);
    void UpdateFromJson(string jsonData);
    byte[] ToUtf8Bytes();
    string ToUft8Base64(bool addDelimeter);
}

[Serializable]
public class PacketBase : IPacket
{
    public string name;
    public string unique;
    public int version;
    public string sender;
    public string receiver;
    public int deviceIndex;
    public int serial;
    public int tick;
    public string dateTime;

    private const string PacketDelimiter = "\r\n";

    public virtual string ToJson(bool intented = false)
    {
        return JsonUtility.ToJson(this);
    }

    public virtual void UpdateFromJson(string jsonData)
    {
        JsonUtility.FromJsonOverwrite(jsonData, this);
    }

    public virtual byte[] ToUtf8Bytes()
    {
        string jsonstring = ToJson();

        return Encoding.UTF8.GetBytes(jsonstring);
    }

    public string ToUft8Base64(bool addDelimeter = true)
    {
        string base64str = Convert.ToBase64String(ToUtf8Bytes());

        if (addDelimeter)
        {
            base64str += PacketDelimiter;
        }

        return base64str;
    }

}

// ------------------------------------------------------------------------
[Serializable]
public enum MyGameStatus
{
    Unknown,
    SystemTest,
    SystemReady,
    GameReady,
    GameStart,
    GamePlay,
    GamePause,
    GameStop,
    SystemStop,
    GameFinish,
}

[Serializable]
public class DataMyGameStatus
{
    public string status;     //OperationStatus
    public string channel;
}

[Serializable]
public class PacketOperationStatus : PacketBase
{
    public DataMyGameStatus data = new DataMyGameStatus();

}

// ------------------------------------------------------------------------
// RPC - Remote Process Controll System
[Serializable]
public enum TargetType
{
    NONE,
    OS,
    APP,
}

[Serializable]
public enum Command
{
    NONE,
    SYSTEM_ON,      // can't implement.. on is manual 
    SYSTEM_SHUTDOWN,
    SYSTEM_REBOOT,
    PROCESS_LAUNCH,
    PROCESS_CLOSE,
    PROCESS_RESTART,
}

[Serializable]
public enum SystemControlState
{
    Unknown,
    Init,
    Ready,
    Bad,
    LaunchStart,
    Launching,
    Launched,
    UnloadStart,
    Unloading,
    Unloaded,
    ShutDownStart,
    ShutDowning,
}

[Serializable]
public class DataSystemControl
{
    public string code;     
    public string target;
    public string command;
    public string state;
}

[Serializable]
public class PacketSystemControl : PacketBase
{
    public DataSystemControl data = new DataSystemControl();
}


using System.Collections.Generic;
using UnityEngine;

public enum CasterID
{
    Unknown,
    All,        // All ID
    GS,         // Game System
    TSC,        // Training System Controll
}

[System.Serializable]
public struct MulticastInfo
{
    public CasterID CasterID;
    public string Groupip;
    public string MyIp;
    public int Port;
    public bool IsMe;
}

public class Multicaster : MonoBehaviourSingleton<Multicaster>
{
    public CasterID myID = CasterID.GS;
    public MulticastInfo[] Recievers;
    public MulticastInfo[] Senders;

    MultiCastRecieverManager _recieverManager = new MultiCastRecieverManager(UnityEngine.Debug.unityLogger);
    public MultiCastRecieverManager MultiCastRecieverManager { get => _recieverManager; }

    MultiCastSenderManager _senderManager = new MultiCastSenderManager(UnityEngine.Debug.unityLogger);
    public MultiCastSenderManager MultiCastSenderManager { get => _senderManager; }

    IniFile _iniFile = new IniFile();
    // Start is called before the first frame update
    void Start()
    {
        string path = string.Empty;
        string fileName = "Config.ini";
      
#if UNITY_ANDROID
        path = Application.persistentDataPath + "/" + fileName;

        if(!System.IO.File.Exists(path)) 
        {
            WWW wwwfile = new WWW(path);
            while (!wwwfile.isDone) { }

            var filepath = Application.persistentDataPath + "/" + fileName;
            System.IO.File.WriteAllBytes(filepath, wwwfile.bytes);

            path = filepath;
        }
#else
        path = Application.streamingAssetsPath + "/" + fileName;
#endif

        _iniFile.Load(path);

        GetRecieverInfoFromFile();
        GetSerderInfoFromFile();

        CreateCastRecievers();
        CreateCastSenders();
    }


    void GetRecieverInfoFromFile()
    {

        if (_iniFile != null)
        {
            List<MulticastInfo> castlist = new List<MulticastInfo>();

            GetAddressInfo(ref castlist, "UDP_RECIEVERS", "GS1_IP", "GS1_PORT", "MY_IP", CasterID.GS, true);

            Recievers = castlist.ToArray();
        }
    }

    void GetSerderInfoFromFile()
    {

        if (_iniFile != null)
        {
            List<MulticastInfo> castlist = new List<MulticastInfo>();

            GetAddressInfo(ref castlist, "UDP_SENDERS", "GS1_IP", "GS1_IP", "MY_IP", CasterID.GS, true);

            Senders = castlist.ToArray();
        }
    }


    void GetAddressInfo(ref List<MulticastInfo> castList, string dividerName, string groupIp, string port, string myIp, CasterID casterID, bool isMe)
    {
        if (_iniFile == null) return;

        string groupIpAddress = _iniFile[dividerName][groupIp].GetString().Trim();
        string myIpAddress = _iniFile[dividerName][myIp].GetString().Trim();
        int readPort = _iniFile[dividerName][port].ToInt();

        if (!string.IsNullOrWhiteSpace(groupIpAddress))
        {
            // Set to List
            MulticastInfo info = new MulticastInfo()
            {
                CasterID = casterID,
                Groupip = groupIpAddress,
                MyIp = myIpAddress,
                Port = readPort,
                IsMe = isMe
            };

            castList.Add(info);

#if UNITY_ANDROID // For Android Debuging
            Debug.Log($"CasterID : {info.CasterID}, Groupip : {info.Groupip}, MyIp : {info.MyIp}, Port : {info.Port}, IsMe : {info.IsMe}");
#endif
        }
    }

    void CreateCastRecievers()
    {
        foreach (var reciever in Recievers)
        {
            _recieverManager.Attach(reciever.CasterID, reciever.Groupip, reciever.Port, reciever.MyIp, reciever.IsMe);
        }
    }

    void CreateCastSenders()
    {
        foreach (var sender in Senders)
        {
            _senderManager.Attach(sender.CasterID, sender.Groupip, sender.Port);
        }
    }

    public void SendToQueue(CasterID target, byte[] pckMessage)
    {
        _senderManager.SendToQueue(target, pckMessage);
    }
}

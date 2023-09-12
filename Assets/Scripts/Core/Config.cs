using UnityEngine;
using System.IO;
using UnityEngine.Playables;
using System.Runtime.CompilerServices;

public class Config : MonoBehaviourSingleton<Config>
{
    // SYSTEM
    public bool DevMode { get; internal set; }
    public bool EnableLog { get; internal set; } = true;

    private void Awake()
    {
        // SYSTEM
        DevMode = false;
        EnableLog = true;

        LoadConfig("Config.ini");
    }

    void LoadConfig(string fileName)
    {
        IniFile iniFile = new IniFile();
        string path = string.Empty;

#if UNITY_ANDROID
        //android path => "jar:file://" + Application.dataPath + "!/assets/" + "Config.ini";
        path = Application.streamingAssetsPath + "/" + fileName;
        
        WWW wwwfile = new WWW(path);
        while (!wwwfile.isDone) { }

        var filepath = Application.persistentDataPath + "/" + fileName;
        File.WriteAllBytes(filepath, wwwfile.bytes);

        path = filepath;
#else
        path = Application.streamingAssetsPath + "/" + fileName;
#endif
        iniFile.Load(path);

        //System Setting
        DevMode = iniFile["SYSTEM"]["DEVMODE"].ToBool();
        EnableLog = iniFile["SYSTEM"]["ENABLE_LOG"].ToBool();
    }

}

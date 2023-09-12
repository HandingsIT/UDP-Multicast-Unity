using System.Collections.Generic;
using System;

public class OperationStatusChangeArgs
{
    public OperationStatusChangeArgs(MyGameStatus operationStatus) { OperationStatus = operationStatus; }
    public MyGameStatus OperationStatus { get; }
}


public class GlobalState : MonoBehaviourSingleton<GlobalState>
{
    private void Awake()
    {
        SetOperationStatus(MyGameStatus.SystemReady);
    }

    /// <summary>
    /// My  Status & Module
    /// </summary>
    public MyGameStatus OperationStatus { get; internal set; } = MyGameStatus.Unknown;
    public void SetOperationStatus(MyGameStatus operationStatus)
    {
        OperationStatus = operationStatus;

        InvokeOperationStatus(OperationStatus);
    }

    SystemModule _myModule = SystemModule.GS;
    public SystemModule MySystemModule
    {
        get { return _myModule; }
    }

    public void SetMyModule(SystemModule systemModule)
    {
        _myModule = systemModule;
    }


    #region Get TCSPacketHandler Datas
    // OPeration Status (OPS)
    public DataMyGameStatus OperationalState { get; set; } = new DataMyGameStatus();

    public delegate void OperationStatusHandler(object sender, OperationStatusChangeArgs e);
    public event OperationStatusHandler OperationStatusChanged;
    void InvokeOperationStatus(MyGameStatus operationStatus)
    {
        if (OperationStatusChanged != null)
        {
            OperationStatusChanged.Invoke(this, new OperationStatusChangeArgs(operationStatus));
        }
    }

    #endregion

}

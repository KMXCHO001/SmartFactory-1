using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActUtlType64Lib;
using TMPro; // MX Component v5 Library ���


/// <summary>
/// OpenPLC, ClosePLC
/// </summary>
public class MxComponent : MonoBehaviour
{
    enum Connection
    {
        Connected,
        Disconnected,
    }

    ActUtlType64 mxComponent;
    Connection connection = Connection.Disconnected;
    public TMP_Text log;

    void Start()
    {
        mxComponent = new ActUtlType64();
        mxComponent.ActLogicalStationNumber = 1;
    }

    public void OnConnectPLCBtnClkEvent()
    {
        if(connection == Connection.Disconnected)
        {
            int returnValue = mxComponent.Open();
            if(returnValue == 0)
            {
                print("���ῡ �����Ͽ����ϴ�.");

                connection = Connection.Connected;
            }
            else
            {
                print("���ῡ �����߽��ϴ�. returnValue: 0x" + returnValue.ToString("X")); // 16������ ����
            }
        }
        else
        {
            print("���� �����Դϴ�.");
        }
    }

    public void OnDisconnectPLCBtnClkEvent()
    {
        if(connection == Connection.Connected)
        {
            int returnValue = mxComponent.Close();
            if (returnValue == 0)
            {
                print("���� �����Ǿ����ϴ�.");
                connection = Connection.Disconnected;
            }
            else
            {
                print("���� ������ �����߽��ϴ�. returnValue: 0x" + returnValue.ToString("X")); // 16������ ����
            }
        }
        else
        {
            print("���� ���� �����Դϴ�.");
        }
    }

    public void OnReadDataBtnClkEvent()
    {
        if(connection == Connection.Connected) 
        {
            int data = 0;
            int returnValue = mxComponent.GetDevice("M0", out data);
            if (returnValue != 0)
                print("returnValue: 0x" + returnValue.ToString("X"));
            else
                log.text = $"M0: {data}";
        }
    }

    public void OnWriteDataBtnClkEvent()
    {
        if (connection == Connection.Connected)
        {
            int returnValue = mxComponent.SetDevice("M0", 1);
            if (returnValue != 0)
                print("returnValue: 0x" + returnValue.ToString("X"));
            else
                log.text = $"M0: 1";
        }
    }

    private void OnDestroy()
    {
        OnDisconnectPLCBtnClkEvent();
    }
}

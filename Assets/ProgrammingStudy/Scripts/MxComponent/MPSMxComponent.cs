using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActUtlType64Lib; // MX Component v5 Library ���
using TMPro;
using UnityEngine.UI;
using System;


namespace MPS
{
    /// <summary>
    /// OpenPLC, ClosePLC
    /// </summary>
    public class MPSMxComponent : MonoBehaviour
    {
        public static MPSMxComponent instance;
        public enum Connection
        {
            Connected,
            Disconnected,
        }

        ActUtlType64 mxComponent;
        public Connection connection = Connection.Disconnected;
        public List<Button> offButtons = new List<Button>(); // MX������Ʈ�� ������ �� �� non-interactable �ϰ� ����� ��ư��

        public Sensor supplySensor;      // 1. ���� ���� ����
        public Piston supplyCylinder;    // 2. ���� �Ǹ���
        public Piston machiningCylinder; // 3. ���� �Ǹ���
        public Piston deliveryCylinder;  // 4. ���� �Ǹ���
        public Sensor objectDetector;    // 6. ��ü ���� ����
        public Conveyor conveyor;        // 7. �����̾�
        public Sensor metalDetector;     // 8. �ݼ� ���� ����
        public Piston dischargeCylinder; // 9. ���� �Ǹ���
        public MeshRenderer redLamp;     // 10. ����
        public MeshRenderer yellowLamp;
        public MeshRenderer greenLamp;

        public bool isCylinderMoving = false;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            mxComponent = new ActUtlType64();
            mxComponent.ActLogicalStationNumber = 1;

            redLamp.material.color = Color.black;
            yellowLamp.material.color = Color.black;
            greenLamp.material.color = Color.black;
        }

        private void Update()
        {
            GetTotalDeviceData();
        }

        private void GetTotalDeviceData()
        {
            supplySensor.plcInputValue          = GetDevice("");
            supplyCylinder.plcInputValues[0]    = GetDevice("");
            supplyCylinder.plcInputValues[1]    = GetDevice("");
            machiningCylinder.plcInputValues[0] = GetDevice("");
            deliveryCylinder.plcInputValues[0]  = GetDevice("");
            deliveryCylinder.plcInputValues[1]  = GetDevice("");
            conveyor.plcInputValue              = GetDevice("");
            metalDetector.plcInputValue         = GetDevice("");
            dischargeCylinder.plcInputValues[0] = GetDevice("");
            dischargeCylinder.plcInputValues[1] = GetDevice("");
            SetLampActive(redLamp, GetDevice(""));
            SetLampActive(yellowLamp, GetDevice(""));
            SetLampActive(greenLamp, GetDevice(""));
        }

        void SetLampActive(MeshRenderer renderer, int value)
        {
            switch(renderer.name)
            {
                case "Red Lamp":
                    if (value > 0)
                        renderer.material.color = Color.red;
                    else
                        renderer.material.color = Color.black;
                    break;
                case "Yellow Lamp":
                    if (value > 0)
                        renderer.material.color = Color.yellow;
                    else
                        renderer.material.color = Color.black;
                    break;
                case "Green Lamp":
                    if (value > 0)
                        renderer.material.color = Color.green;
                    else
                        renderer.material.color = Color.black;
                    break;
            }
        }

        int GetDevice(string device)
        {
            if (connection == Connection.Connected)
            {
                int data = 0;
                int returnValue = mxComponent.GetDevice(device, out data);

                if (returnValue != 0)
                    print(returnValue.ToString("X"));

                return data;
            }
            else
                return 0;
        }

        public void OnConnectPLCBtnClkEvent()
        {
            if (connection == Connection.Disconnected)
            {
                int returnValue = mxComponent.Open();
                if (returnValue == 0)
                {
                    print("���ῡ �����Ͽ����ϴ�.");

                    SetOffButtonsActive(false);

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

        private void SetOffButtonsActive(bool isActive)
        {
            foreach(Button btn in offButtons)
            {
                btn.interactable = isActive;
            }
        }

        public void OnDisconnectPLCBtnClkEvent()
        {
            if (connection == Connection.Connected)
            {
                int returnValue = mxComponent.Close();
                if (returnValue == 0)
                {
                    print("���� �����Ǿ����ϴ�.");

                    SetOffButtonsActive(true);

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

        // �ǽ�2. ���� �Ǹ���(A) ����, ���� ��, ���� �Ǹ���(B) ����, ����
        // ����: ��� ������ �۵��ð��� 1��
        // Vector3.Lerp ���
        IEnumerator MoveCylinder(Transform cylinder, Vector3 positionA, Vector3 positionB, float duration)
        {
            isCylinderMoving = true;
            float currentTime = 0;

            while (true)
            {
                currentTime += Time.deltaTime;

                if (currentTime >= duration)
                {
                    currentTime = 0;
                    break;
                }

                cylinder.position = Vector3.Lerp(positionA, positionB, currentTime / duration);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            isCylinderMoving = false;
        }

        private void OnDestroy()
        {
            OnDisconnectPLCBtnClkEvent();
        }
    }

}

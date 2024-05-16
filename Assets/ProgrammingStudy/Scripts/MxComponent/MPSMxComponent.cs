using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActUtlType64Lib; // MX Component v5 Library 사용
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;


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
        public List<Button> offButtons = new List<Button>(); // MX컴포넌트에 연결이 될 때 non-interactable 하게 만드는 버튼들

        public Sensor supplySensor;      // 1. 공급 감지 센서
        public Piston supplyCylinder;    // 2. 공급 실린더
        public Piston machiningCylinder; // 3. 가공 실린더
        public Piston deliveryCylinder;  // 4. 송출 실린더
        public Sensor objectDetector;    // 5. 물체 감지 센서
        public Conveyor conveyor;        // 6. 컨베이어
        public Sensor metalDetector;     // 7. 금속 감지 센서
        public Piston dischargeCylinder; // 8. 배출 실린더
        public MeshRenderer redLamp;     // 9. 램프
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
            if (connection == Connection.Connected)
            {
                short[] yData = ReadDeviceBlock("Y0", 5); // Short지만, 10개의 비트를 가져옴
                string newYData = ConvertDateIntoString(yData);
                
                supplySensor.plcInputValue              = newYData[4 ] - '0';  // Y4
                supplyCylinder.plcInputValues[0]        = newYData[10] - 48;  // Y10
                supplyCylinder.plcInputValues[1]        = newYData[11] - 48;
                machiningCylinder.plcInputValues[0]     = newYData[20] - 48;
                deliveryCylinder.plcInputValues[0]      = newYData[30] - 48;
                deliveryCylinder.plcInputValues[1]      = newYData[31] - 48;
                conveyor.plcInputValue                  = newYData[0 ] - 48;
                metalDetector.plcInputValue             = newYData[6 ] - 48;
                dischargeCylinder.plcInputValues[0]     = newYData[40] - 48;
                dischargeCylinder.plcInputValues[1]     = newYData[41] - 48;  // 50ms * 10 = 0.5s
                SetLampActive(redLamp,    newYData[1] - 48);
                SetLampActive(yellowLamp, newYData[2] - 48);
                SetLampActive(greenLamp,  newYData[3] - 48);
            }
        }

        string ConvertDateIntoString(short[] data)
        {
            string newYData = "";
            print(data.Length + "yData length");
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    newYData += "0000000000";
                    continue;
                }

                string temp = Convert.ToString(data[i], 2);// 100
                string temp2 = new string(temp.Reverse().ToArray()); // reverse 100 -> 001  
                newYData += temp2; // 0000000000 + 001

                if (temp2.Length < 10)
                {
                    int zeroCount = 10 - temp2.Length; // 7 -> 7개의 0을 newYData에 더해준다. (0000000)
                    for (int j = 0; j < zeroCount; j++)
                        newYData += "0";
                } // 0000000000 + 001 + 0000000 -> 총 20개의 비트
            }

            return newYData;
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

        short[] ReadDeviceBlock(string startDeviceName, int _blockSize)
        {
            if (connection == Connection.Connected)
            {
                short[] devices = new short[_blockSize];
                int returnValue = mxComponent.ReadDeviceBlock2(startDeviceName, _blockSize, out devices[0]);

                if (returnValue != 0)
                    print(returnValue.ToString("X"));

                return devices;
            }
            else
                return null;
        }

        public bool SetDevice(string device, int value)
        {
            if (connection == Connection.Connected)
            {
                int returnValue = mxComponent.SetDevice(device, value);

                if (returnValue != 0)
                    print(returnValue.ToString("X"));

                return true;
            }
            else
                return false;
        }

        public void OnConnectPLCBtnClkEvent()
        {
            if (connection == Connection.Disconnected)
            {
                int returnValue = mxComponent.Open();
                if (returnValue == 0)
                {
                    print("연결에 성공하였습니다.");

                    SetOffButtonsActive(false);

                    StartCoroutine(CoSendDevice());

                    connection = Connection.Connected;
                }
                else
                {
                    print("연결에 실패했습니다. returnValue: 0x" + returnValue.ToString("X")); // 16진수로 변경
                }
            }
            else
            {
                print("연결 상태입니다.");
            }
        }

        IEnumerator CoSendDevice()
        {
            yield return new WaitForSeconds(0.5f);

            supplyCylinder.SetSwitchDevicesByCylinderMoving(false, true);
            machiningCylinder.SetSwitchDevicesByCylinderMoving(false, true);
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
                    print("연결 해지되었습니다.");

                    SetOffButtonsActive(true);

                    connection = Connection.Disconnected;
                }
                else
                {
                    print("연결 해지에 실패했습니다. returnValue: 0x" + returnValue.ToString("X")); // 16진수로 변경
                }
            }
            else
            {
                print("연결 해지 상태입니다.");
            }
        }

        // 실습2. 공급 실린더(A) 전진, 후진 후, 송출 실린더(B) 전진, 후진
        // 조건: 모든 시퀀스 작동시간은 1초
        // Vector3.Lerp 사용
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActUtlType64Lib; // MX Component v5 Library 사용
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
            if(connection == Connection.Connected)
            {
                //supplySensor.plcInputValue          = GetDevice("Y4");
                supplyCylinder.plcInputValues[0]    = GetDevice("Y10");
                supplyCylinder.plcInputValues[1]    = GetDevice("Y11");
                machiningCylinder.plcInputValues[0] = GetDevice("Y20");
                //deliveryCylinder.plcInputValues[0]  = GetDevice("Y30");
                //deliveryCylinder.plcInputValues[1]  = GetDevice("Y31");
                //conveyor.plcInputValue              = GetDevice("Y0");
                //metalDetector.plcInputValue         = GetDevice("Y6");
                //dischargeCylinder.plcInputValues[0] = GetDevice("Y40");
                //dischargeCylinder.plcInputValues[1] = GetDevice("Y41");
                //SetLampActive(redLamp, GetDevice("Y1"));
                //SetLampActive(yellowLamp, GetDevice("Y2"));
                //SetLampActive(greenLamp, GetDevice("Y3"));
            }
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

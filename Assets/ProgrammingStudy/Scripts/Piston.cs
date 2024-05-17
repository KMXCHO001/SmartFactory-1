using MPS;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Piston : MonoBehaviour
{
    public enum Option
    {
        SingleSolenoid = 1,
        DoubleSolenoid = 2
    }

    // 상태
    [Header("현재 상태")]
    [Tooltip("솔레노이드를 편솔 또는 양솔로 설정합니다.")]
    public Option option = Option.SingleSolenoid;
    [Tooltip("GX Works의 Global Device Comment에 설정한 디바이스 값을 입력해 주세요.")]
    public string rearSwitchDeviceName;
    public string frontSwitchDeviceName;
    public int[] plcInputValues; // 편솔의 경우 입력 1개, 양솔의 경우 입력 2개
    public bool isBackward = true;
    public bool isCylinderMoving = false;
    [Tooltip("실린더가 이동을 마치는 데 걸리는 시간입니다.")]
    public float runTime = 2;
    float elapsedTime;

    // 초기화
    [Space(20)]
    [Header("초기화")]
    public Transform pistonRod;
    public Transform switchForward;
    public Transform switchBackward;
    public Image forwardButtonImg;
    public Image backwardButtonImg;
    public float minRange;
    public float maxRange;
    [Tooltip("실린더가 후진했을 때의 위치입니다.")]
    Vector3 minPos;
    [Tooltip("실린더가 전진했을 때의 위치입니다.")]
    Vector3 maxPos;

    // 옵션
    [Space(20)]
    [Header("옵션")]
    [Tooltip("금속감지 센서 연결을 위한 변수입니다.")]
    public Sensor sensor;
    [Tooltip("실린더가 움직일 때 재생되는 오디오 클립 입니다.")]
    public AudioClip clip;


    // Start is called before the first frame update
    void Start()
    {
        DeviceInfo info = new DeviceInfo("신태욱", "123456", 55555, 5555, "2024.05.30", "2026.06.30");
        JsonSerialization.Instance.devices.Add(info);

        SetCylinderSwitchActive(true);
        SetCylinderBtnActive(true);

        minPos = new Vector3(pistonRod.transform.localPosition.x, minRange, pistonRod.transform.localPosition.z);
        maxPos = new Vector3(pistonRod.transform.localPosition.x, maxRange, pistonRod.transform.localPosition.z);

        plcInputValues = new int[(int)option];
    }

    private void Update()
    {
        if (MPSMxComponent.instance.connection == MPSMxComponent.Connection.Connected)
        {
            if(option == Option.SingleSolenoid) // 편솔
            {
                // 실린더 전진
                if (plcInputValues[0] > 0 && !isCylinderMoving && isBackward)
                    StartCoroutine(CoMove(isBackward));
                else if(plcInputValues[0] == 0 && !isCylinderMoving && !isBackward)
                    StartCoroutine(CoMove(!isBackward));
            }
            else if(option == Option.DoubleSolenoid) // 양솔
            {
                // 실린더 전진
                if (plcInputValues[0] > 0 && !isCylinderMoving && isBackward)
                    StartCoroutine(CoMove(isBackward));

                // 실린더 후진
                if (plcInputValues[1] > 0 && !isCylinderMoving && !isBackward)
                    StartCoroutine(CoMove(!isBackward));
            }
        }
    }

    public void MovePistonRod(Vector3 startPos, Vector3 endPos, float _elapsedTime, float _runTime)
    {
        Vector3 newPos = Vector3.Lerp(startPos, endPos, _elapsedTime / _runTime); // t값이 0(minPos) ~ 1(maxPos) 로 변화
        pistonRod.transform.localPosition = newPos;
    }

    public void OnDischargeObjectBtnEvent()
    {
        print("작동!");
        if(sensor != null && sensor.isMetalObject)
        {
            print("배출 완료");
            OnCylinderButtonClickEvent(true);
        }
    }

    // PistonRod가 Min, Max 까지
    // 참고: LocalTransform.position.y가 -0.3 ~ 1.75 까지 이동
    public void OnCylinderButtonClickEvent(bool direction)
    {
        StartCoroutine(CoMove(isBackward));

        if(clip.name.Contains("screw-driver"))  
            AudioManager.instance.SetVolume(0.5f);
    
        AudioManager.instance.PlayAudioClip(clip);
    }

    public void SetSwitchDevicesByCylinderMoving(bool _isCylinderMoving, bool _isBackward)
    {
        if (_isCylinderMoving)
        {
            MPSMxComponent.instance.SetDevice(rearSwitchDeviceName, 0);
            MPSMxComponent.instance.SetDevice(frontSwitchDeviceName, 0);
            print($"isBackward: {_isBackward}, {rearSwitchDeviceName}: 0");
            print($"isBackward: {_isBackward}, {frontSwitchDeviceName}: 0");

            return;
        }

        if (_isBackward)
        {
            MPSMxComponent.instance.SetDevice(rearSwitchDeviceName, 1);
            print($"isBackward: {_isBackward}, {rearSwitchDeviceName}: 1");
        }
        else
        {
            MPSMxComponent.instance.SetDevice(frontSwitchDeviceName, 1);
            print($"isBackward: {_isBackward}, {frontSwitchDeviceName}: 1");
        }
    }

    IEnumerator CoMove(bool direction)
    {
        isCylinderMoving = true;

        SetButtonActive(false);
        SetCylinderBtnActive(false);
        SetCylinderSwitchActive(false);

        SetSwitchDevicesByCylinderMoving(isCylinderMoving, isBackward); // 스위치 값 변경

        elapsedTime = 0;

        while (elapsedTime < runTime)
        {
            elapsedTime += Time.deltaTime;

            if (isBackward)
            {
                print(name + " 전진중...");

                MovePistonRod(minPos, maxPos, elapsedTime, runTime);
            }
            else
            {
                print(name + " 후진중...");

                MovePistonRod(maxPos, minPos, elapsedTime, runTime);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }


        isBackward = !isBackward; // 초기값(true) -> false
        isCylinderMoving = false;

        SetSwitchDevicesByCylinderMoving(isCylinderMoving, isBackward);

        SetCylinderSwitchActive(true);
        SetCylinderBtnActive(true);
        SetButtonActive(true);
    }

    private void SetCylinderSwitchActive(bool isActive)
    {
        if(isActive)
        {
            if (isBackward)
            {
                switchBackward.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                switchForward.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
        else
        {
            switchForward.GetComponent<MeshRenderer>().material.color = Color.white;
            switchBackward.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    void SetCylinderBtnActive(bool isActive)
    {
        if (isActive)
        {
            if (isBackward)
            {
                forwardButtonImg.color = Color.white;
                backwardButtonImg.color = Color.green;
            }
            else
            {
                forwardButtonImg.color = Color.green;
                backwardButtonImg.color = Color.white;
            }
        }
        else
        {
            forwardButtonImg.color = Color.white;
            forwardButtonImg.color = Color.white;
        }

    }

    void SetButtonActive(bool isActive)
    {
        if (MPSMxComponent.instance.connection == MPSMxComponent.Connection.Connected)
            return;

        forwardButtonImg.GetComponent<Button>().interactable = isActive;
        backwardButtonImg.GetComponent<Button>().interactable = isActive;
    }
}

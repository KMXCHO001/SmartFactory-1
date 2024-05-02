using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DeviceInfo
{
    public string name;
    public string serialNumber;
    public int operationTime;
    public int operationCount;
    public string freeWarrenty;
    public string paidWarrenty;

    public DeviceInfo(string name, string serialNumber, int operationTime, int operationCount, string freeWarrenty, string paidWarrenty)
    {
        this.name = name;
        this.serialNumber = serialNumber;
        this.operationTime = operationTime;
        this.operationCount = operationCount;
        this.freeWarrenty = freeWarrenty;
        this.paidWarrenty = paidWarrenty;
    }
}

public class JsonSerialization : MonoBehaviour
{
    public static JsonSerialization Instance;

    // Object(Class) -> JSON
    public class Person
    {
        public string name;
        public int age;

        public Person(string name, int age)
        {
            this.name = name;
            this.age = age;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // ��ư�� ������, ��� device�� ������ �����Ѵ�.

    // Start is called before the first frame update
    void Start()
    {
        // ���� �����͸� �����ϴ� ���
        DeviceInfo info = new DeviceInfo("���¿�", "123456", 55555, 5555, "2024.05.30", "2026.06.30");

        string json = JsonUtility.ToJson(info); // ����ȭ(serialization)
        print(json);

        FileStream fs = new FileStream("Assets/file.json", FileMode.Create); // ������ ����, �ݴ� �⺻���� ����� ���
        StreamWriter sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json);
        sw.Close();
        fs.Close();


        // �������� �����͸� �����ϴ� ���
        DeviceInfo info1 = new DeviceInfo("���¿�", "123456", 55555, 5555, "2024.05.30", "2026.06.30");
        DeviceInfo info2 = new DeviceInfo("���¿�1", "123456", 55555, 5555, "2024.05.30", "2026.06.30");
        DeviceInfo info3 = new DeviceInfo("���¿�2", "123456", 55555, 5555, "2024.05.30", "2026.06.30");
        DeviceInfo info4 = new DeviceInfo("���¿�3", "123456", 55555, 5555, "2024.05.30", "2026.06.30");
        DeviceInfo info5 = new DeviceInfo("���¿�4", "123456", 55555, 5555, "2024.05.30", "2026.06.30");
        List<DeviceInfo> devices = new List<DeviceInfo>();
        devices.Add(info1);
        devices.Add(info2);
        devices.Add(info3);
        devices.Add(info4);
        devices.Add(info5);

        string json2 = JsonConvert.SerializeObject(devices);
        print(json2);

        fs = new FileStream("Assets/file2.json", FileMode.Create); // ������ ����, �ݴ� �⺻���� ����� ���
        sw = new StreamWriter(fs); // ���� ������ ������ ����, ���ڵ� ó��
        sw.Write(json2);
        sw.Close();
        fs.Close();

        // DeviceInfo��� �����̳� Ŭ������ ����� �˰� ���� ��� ���
        List<DeviceInfo> newDevices = new List<DeviceInfo>();
        newDevices = JsonConvert.DeserializeObject<List<DeviceInfo>>(json2);
        DeviceInfo deviceFound = newDevices.Find(x => x.name == "���¿�3");
        print(deviceFound.freeWarrenty);

        // ������ ������ ��Ģ�� ���, JObject, JArray
        string json3 = @"{
          'channel': {
            'title': 'ABC',
            'link': 'http://ABC.com',
            'description': 'ABC's blog.',
            'item': [
              {
                'title': 'Json.NET 1.3 + New license + Now on CodePlex',
                'description': 'Annoucing the release of Json.NET 1.3',
                'link': 'http://ABC.aspx',
                'categories': [
                  'Json.NET',
                  'CodePlex'
                ]
              },
              {
                'title': 'LINQ to JSON beta',
                'description': 'Annoucing LINQ to JSON',
                'link': 'http://ABC.aspx',
                'categories': [
                  'Json.NET',
                  'LINQ'
                ]
              }
            ]
          }
        }";

        JObject jObj = JObject.Parse(json3);
        string title = (string)jObj["channel"]["title"];
        string description = (string)jObj["channel"]["title"][1]["description"];


        /* 
        Person person = new Person("���¿�", 20);

        // ��ü -> JSON
        string json = JsonUtility.ToJson(person);

        print(json);

        Person person2 = JsonUtility.FromJson<Person>(json);
        print($"{person2.name}, {person2.age}");*/
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            FileStream fs = new FileStream("Assets/file.json", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string json = sr.ReadToEnd();
            print(json);
            sr.Close();
            fs.Close();
        }
    }
}

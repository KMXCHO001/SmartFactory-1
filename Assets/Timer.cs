using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ð��� ����� Ư�� ������ �����Ѵ�.

public class Timer : MonoBehaviour
{
    public float currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        //print("��� �ð�: " + currentTime);
    }
}

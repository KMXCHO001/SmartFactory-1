using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        // 6. IF, Switch ���ǹ�

        bool isActive = true;

        if (isActive)
        {
            print("�۵� �� �Դϴ�.");
            Debug.Log("�۵� �� �Դϴ�.");
        }
        else
        {
            print("�������� �Դϴ�.");
        }

        int number = 5;

        // �� ������
        if (number == 0) print("");
        else if (number != 0) print("");
        else if (number > 0) print("");
        else if (number < 0) print("");
        else if (number >= 0) print("");
        else if (number <= 0) print("");

        // �� ������ AND(&&), OR(||), NOT(!)
        if (number == 0 && isActive == true) print("");
        else if (number == 0 || isActive == false) print("");
        else if (!isActive) print("");

        // ��ȣ�� ���� ������ �۵�
        int status = 0;

        switch (status)
        {
            case 0:
                print("0�� �����Դϴ�.");
                break;
            case 1:
                print("1�� �����Դϴ�.");
                break;
            case 2:
                print("2�� �����Դϴ�.");
                break;
        }

        // �ǽ�3. PLC 5�� ������ ��ȣ���� if�� �Ǵ� switch case������ �ۼ��� ���ϴ�.
        // ����: status / Ȳ��(0), ����(1), ���(2)
        // M10(0), M11(1), M12(2) -> print("Ȳ�� ���� ON");
        // Timer -> print("5�� Timer On");

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                print("Ȳ�� ���� ON");
            }
            else if (i == 1)
            {
                print("���� ���� ON");
            }
            else if (i == 2)
            {
                print("��� ���� ON");
            }

            print("5�� Timer On");
        }
    }

    void Update()
    {
        
    }
}

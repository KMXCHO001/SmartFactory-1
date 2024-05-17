using MPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool isObjectDetected = false; // flag 변수, bool 변수
    public bool isMetalObject = false;
    public MeshRenderer led;
    public AudioClip clip;
    public int plcInputValue;

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayAudioClip(clip);
        AudioManager.instance.SetPlayTime(4f);

        print("MetalObject 감지");
        MPSMxComponent.instance.SetDevice("Y5", 1);

        if (other.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            isObjectDetected = true;

            if (GetComponent<MeshRenderer>() != null && GetComponent<MeshRenderer>().isVisible)
            {
                GetComponent<MeshRenderer>().material.color = Color.green;
            }

            if (this.gameObject.layer == LayerMask.NameToLayer("Destination"))
            {
                print(this.gameObject.name);
            }
        }            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("MetalObject"))
        {
            isMetalObject = true;
        }
        /*  else
        {
            isMetalObject = false;
            print("NonMetalObject 감지");
        }*/

        led.material.color = Color.green;
        isObjectDetected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        led.material.color = Color.white;
        isObjectDetected = false;
        MPSMxComponent.instance.SetDevice("Y5", 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAndTorque : MonoBehaviour
{
    public float force = 20f;
    Rigidbody rb;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            Vector3 dir = Vector3.up;
            rb.AddForce(dir * force);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 dir = transform.up;
            rb.AddTorque(dir * force);


            float turn = Input.GetAxis("Horizontal");

            rb.AddTorque(dir * force * turn);

            if(Input.GetKey(KeyCode.Q)) 
            {
                rb.velocity = Vector3.zero;
                rb.rotation = Quaternion.identity;
            }
        }
    }
}

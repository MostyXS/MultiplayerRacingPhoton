using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] Vector3 thrustForce = new Vector3(0,0,45f);
    [SerializeField] Vector3 rotationTorque = new Vector3(0f, 8f, 0f);

    [HideInInspector]
    public bool ControlsEnabled = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!ControlsEnabled) return;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(thrustForce);
        }

        //backward
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(-thrustForce);

        }

        //turning left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(-rotationTorque);
        }

        //turning right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(rotationTorque);
        }


    }
}

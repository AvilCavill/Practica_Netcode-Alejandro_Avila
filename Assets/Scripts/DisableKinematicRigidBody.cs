using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableKinematicRigidBody : MonoBehaviour
{
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}

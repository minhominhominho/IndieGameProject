using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testHips : MonoBehaviour
{

    public GameObject hip;
    private Rigidbody rb;

    private void Start()
    {
        rb = hip.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("CC");
            rb.AddForce(Vector3.right * 1000);
        }
    }
}

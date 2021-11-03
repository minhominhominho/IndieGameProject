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
        var hmove = (Input.GetKey(KeyCode.RightArrow)?1:0) - (Input.GetKey(KeyCode.LeftArrow)?1:0);
        if (hmove != 0)
        {
            Debug.Log("CC");
            rb.AddForce(Vector3.right * hmove * 1000);
        }
        /*
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("CC");
            rb.AddForce(Vector3.right * 1000);
        }*/
    }
}

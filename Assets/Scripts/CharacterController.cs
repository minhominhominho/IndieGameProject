using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private bool isStart = false;

    public void startButton()
    {
        isStart = true;
    }



    void Update()
    {
        if (isStart)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {

                Debug.Log("Move to 1");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                Debug.Log("Move to 2");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                Debug.Log("Move to 3");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                Debug.Log("Move to 4");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Debug.Log("Move to 5");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                Debug.Log("Move to 6");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                Debug.Log("Move to 7");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                Debug.Log("Move to 8");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                Debug.Log("Move to 9");
            }
        }
    }
}

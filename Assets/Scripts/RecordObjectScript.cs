using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordObjectScript : MonoBehaviour
{
    private bool isNumpad = true;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void setNumPadMode(bool setMode)
    {
        isNumpad = setMode;
    }

    public bool getMode()
    {
        return isNumpad;
    }
}

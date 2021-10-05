using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class DancePadManger : MonoBehaviour
{
    [SerializeField]
    private string songName = null;
    private AudioSource song;

    [SerializeField]
    private List<GameObject> padNumbers = null;

    [SerializeField]
    private GameObject redEffect = null;
    private float redEffectFullSize = 0.91172f; // test and define it

    private List<int> notes = new List<int>();

    private bool isLeftFoot = true;

    private bool isStart = false;


    void Update()
    {
        if (isStart)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                inputFunc(1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                inputFunc(2);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                inputFunc(3);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                inputFunc(4);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                inputFunc(5);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                inputFunc(6);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                inputFunc(7);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                inputFunc(8);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                inputFunc(9);
            }
        }
    }

    private void inputFunc(int num)
    {
        if (isLeftFoot)
        {
            // Move character's left foot
        }
        else
        {
            // Move character's right foot
        }

        // Red effect is stored in each pad tile so when hit the number, clear pad tile's child(red effect)
        if (padNumbers[num - 1].transform.childCount > 0)
        {
            Debug.Log("Clear " + num + "!");
            Destroy(padNumbers[num - 1].transform.GetChild(0).gameObject);
        }
        else
        {
            Debug.Log("Wrong input " + num + "!");
        }
    }

    public void startButton()
    {
        if (!isStart)
        {
            isStart = true;
            song = GetComponent<AudioSource>();
            string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/test1.csv";

            using (Stream s = File.OpenRead(filePath))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    foreach(char c in sr.ReadToEnd())
                    {
                        notes.Add((int)Char.GetNumericValue(c));
                    }
                }
            }

            StartCoroutine(dancePadControl(notes));
        }
    }

    IEnumerator dancePadControl(List<int> list)
    {
        song.Play();
        foreach (int n in list)
        {
            if (n != 0)
            { StartCoroutine(fillRed(n - 1)); }
            yield return new WaitForSecondsRealtime(1/60f);
        }

    }

    IEnumerator fillRed(int num)
    {
        GameObject redOb = Instantiate(redEffect, padNumbers[num].transform);
        redOb.SetActive(true);
        redOb.transform.position = padNumbers[num].transform.position;
        float off = (redEffectFullSize - redOb.transform.localScale.x) / 50;

        while (true)
        {
            try
            {
                if (redOb.activeInHierarchy && redOb.transform.localScale.x < redEffectFullSize)
                {
                    redOb.transform.localScale = new Vector3(redOb.transform.localScale.x + off, redOb.transform.localScale.y + off, redOb.transform.localScale.z + off);
                }
                else
                    break;
            }
            catch { }

            yield return new WaitForSecondsRealtime(0.01f);
        }
        yield return new WaitForSecondsRealtime(0.05f);
        Destroy(redOb);
    }
}

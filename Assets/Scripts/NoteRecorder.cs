using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public class NoteRecorder : MonoBehaviour
{
    [SerializeField]
    private string songName = null;
    private AudioSource song = null;
    private List<string> notes = new List<string>();

    void Start()
    {
        song = GetComponent<AudioSource>();
        Invoke("playSong", 3);
    }

    void playSong()
    {
        song.Play();
    }

    public void writeCSV()
    {
        string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/{songName}.csv";
       
        try
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    foreach (string s in notes)
                    {
                        outStream.WriteLine(s);
                    }

                }
            }
        }
        catch
        {
            Debug.Log("Fail");
        }
    }

    void Update()
    {
        if (song.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                notes.Add((1).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                notes.Add((2).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                notes.Add((3).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                notes.Add((4).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                notes.Add((5).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                notes.Add((6).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                notes.Add((7).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                notes.Add((8).ToString());
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                notes.Add((9).ToString());
            }
            else
            {
                notes.Add((0).ToString());
            }
        }
    }
}
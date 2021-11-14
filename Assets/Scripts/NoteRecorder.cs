using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public class NoteRecorder : MonoBehaviour
{
    [SerializeField] private string songName = null;
    [SerializeField] private float BPM;
    [SerializeField] private float DIVIDER;
    private int startingGap = 2;

    private AudioSource song = null;
    private List<string> notesNum = new List<string>();
    private List<DateTime> notesDatetime = new List<DateTime>();
    private List<int> notesTiming = new List<int>();
    private DateTime startingTime;

    void Start()
    {
        song = GetComponent<AudioSource>();
        Invoke("playSong", 3);
    }

    void playSong()
    {
        song.Play();
        startingTime = DateTime.Now;
    }

    public void writeCSV()
    {
        float beatCount = (float)BPM / DIVIDER;
        for (int i = 0; i < notesNum.Count; i++)
        {
            notesTiming.Add((int)Math.Round(notesDatetime[i].Subtract(startingTime).TotalMilliseconds / 1000 / (1 / beatCount)));
        }

        string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/made105bpm.csv";

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8))
            {
                string temp = BPM.ToString() + ',' + DIVIDER.ToString() + ',' + '0';
                outStream.WriteLine(temp);
                for(int i = 0; i < notesNum.Count; i++)
                {
                    temp = $"{notesNum[i]},{notesTiming[i]}";
                    outStream.WriteLine(temp);
                }
            }
        }
    }

    void Update()
    {
        if (song.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                notesNum.Add((1).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                notesNum.Add((2).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                notesNum.Add((3).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                notesNum.Add((4).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                notesNum.Add((5).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                notesNum.Add((6).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                notesNum.Add((7).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                notesNum.Add((8).ToString());
                notesDatetime.Add(DateTime.Now);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                notesNum.Add((9).ToString());
                notesDatetime.Add(DateTime.Now);
            }
        }
    }
}
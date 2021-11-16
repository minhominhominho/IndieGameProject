using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;


public class NoteRecorder : MonoBehaviour
{
    [SerializeField] private string songName = null;
    [SerializeField] private float BPM;
    [SerializeField] private float DIVIDER;
    [SerializeField] private Image[] notes;

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

    //public void writeCSV()
    //{
    //    float beatCount = BPM / DIVIDER;
    //    for (int i = 0; i < notesNum.Count; i++)
    //    {
    //        notesTiming.Add((int)Math.Round(notesDatetime[i].Subtract(startingTime).TotalMilliseconds / 1000 / (1 / beatCount)));
    //    }

    //    string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/made105bpm.csv";

    //    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
    //    {
    //        using (StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8))
    //        {
    //            string temp = BPM.ToString() + ',' + DIVIDER.ToString() + ',' + '0';
    //            outStream.WriteLine(temp);
    //            for(int i = 0; i < notesNum.Count; i++)
    //            {
    //                temp = $"{notesNum[i]},{notesTiming[i]}";
    //                outStream.WriteLine(temp);
    //            }
    //        }
    //    }
    //}

    public void writeCSV()
    {
        float beatCount = BPM / DIVIDER;


        for (int i = 0; i < notesNum.Count; i++)
        {
            if (i == 0)
                notesTiming.Add((int)Math.Round(notesDatetime[i].Subtract(startingTime).TotalMilliseconds / 1000 / (1 / beatCount)));
            else
                notesTiming.Add(notesTiming[i - 1] + (int)Math.Round(notesDatetime[i].Subtract(startingTime).TotalMilliseconds / 1000 / (1 / beatCount)));
           
            startingTime = notesDatetime[i];
        }

        string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/{songName}.csv";

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8))
            {
                string temp = BPM.ToString() + ',' + DIVIDER.ToString() + ',' + 0;
                outStream.WriteLine(temp);
                for (int i = 0; i < notesNum.Count; i++)
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
                StartCoroutine(ShowColor(1));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                notesNum.Add((2).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(2));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                notesNum.Add((3).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(3));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                notesNum.Add((4).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(4));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                notesNum.Add((5).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(5));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                notesNum.Add((6).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(6));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                notesNum.Add((7).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(7));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                notesNum.Add((8).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(8));
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                notesNum.Add((9).ToString());
                notesDatetime.Add(DateTime.Now);
                StartCoroutine(ShowColor(9));
            }
        }
    }

    IEnumerator ShowColor(int i)
    {
        notes[i - 1].color = new Color(255,0, 0);
        yield return new WaitForSecondsRealtime(1/(BPM / DIVIDER));
        notes[i - 1].color = new Color(255, 255, 255);
    }
}
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

    private List<Note> notes = new List<Note>();

    private bool isStart = false;
    private float startingPoint;
    private float beatInterval;
    private bool isLeftFoot = true;


    void Update()
    {
        if (isStart)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                inputFunc(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                inputFunc(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                inputFunc(3);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                inputFunc(4);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                inputFunc(5);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                inputFunc(6);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                inputFunc(7);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                inputFunc(8);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
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
            Destroy(padNumbers[num - 1].transform.GetChild(0).gameObject);
        }
        else
        {
            // Input wrong number
        }
    }

    class Note
    {
        public int _noteNum { get; set; }
        public int _order { get; set; }
        public Note(int noteNum, int order)
        {
            this._noteNum = noteNum;
            this._order = order;
        }
    }

    public void startNoteCreation()
    {
        if (!isStart)
        {
            isStart = true;
            song = GetComponent<AudioSource>();
            string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/{songName}.csv";

            using (Stream s = File.OpenRead(filePath))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string line = sr.ReadLine();
                    float bpm = float.Parse(line.Split(',')[0]);
                    float divider = float.Parse(line.Split(',')[1]);
                    startingPoint = float.Parse(line.Split(',')[2]);
                    float beatCount = (float)bpm / divider;
                    beatInterval = 1 / beatCount;
                    sr.ReadLine();

                    while ((line = sr.ReadLine()) != null)
                    {
                        Note note = new Note(int.Parse(line.Split(',')[0]),int.Parse(line.Split(',')[1]));
                        notes.Add(note);
                    }
                }
            }

            song.Play();

            for (int i = 0; i < notes.Count; i++)
            {
                StartCoroutine(futureNote(notes[i]));
            }
        }
    }

    private IEnumerator futureNote(Note note)
    { 
        yield return new WaitForSecondsRealtime(startingPoint + (note._order-1) * beatInterval);
        StartCoroutine(makeNote(note._noteNum - 1));
    }

    // for testing
    //private IEnumerator makeNote_T(int num)
    //{
    //    GameObject redOb = Instantiate(redEffect, padNumbers[num].transform);
    //    redOb.SetActive(true);
    //    redOb.transform.position = padNumbers[num].transform.position;
    //    float off = (redEffectFullSize - redOb.transform.localScale.x) / 50;

    //    yield return new WaitForSecondsRealtime(0.2f);
    //    Destroy(redOb);
    //}

    private IEnumerator makeNote(int num)
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
        yield return new WaitForSecondsRealtime(beatInterval);
        Destroy(redOb);
    }
}

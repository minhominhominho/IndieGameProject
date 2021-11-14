using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Text;
using UnityEngine.UI;
using TMPro;

[SerializeField]
public class DancePadManger : MonoBehaviour
{
    [SerializeField] private string songName = null;
    [SerializeField] private AudioSource speaker = null;
    [SerializeField] private AudioClip resultAudio = null;
    [SerializeField] private TextMeshProUGUI inplayScore = null;
    [SerializeField] private Image inplayLife = null;
    [SerializeField] private int playerMaxLife;

    [SerializeField] private List<GameObject> padNumbers = null;
    [SerializeField] private List<GameObject> miniPadNumbers = null;
    [SerializeField] private GameObject redEffect = null;
    [SerializeField] private GameObject miniRedEffect = null;
    [SerializeField] private RectTransform miniDancepadArea = null;
    [SerializeField] private RectTransform centerOfMass = null;
    [SerializeField] private TextMeshProUGUI eMsg = null;
    private float redEffectFullSize = 0.91172f; // test and define it

    [SerializeField] private GameObject resultPanel = null;
    [SerializeField] private TextMeshProUGUI resultText = null;
    [SerializeField] private TextMeshProUGUI resultScore = null;
    private int playerScore = 0;
    private int playerLife;


    private List<Note> notes = new List<Note>();
    private Vector3 mLastPos;
    private Vector3 mPos;

    private AudioSource song;
    private bool isStart = false;
    private bool isMouseDragging = false;
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

            if (Input.GetMouseButtonDown(1))
            {
                isMouseDragging = true;
                mPos = Input.mousePosition;
                mPos = new Vector3(mPos.x / Screen.width * Screen.width / miniDancepadArea.localScale.x, mPos.y / Screen.height * Screen.height / miniDancepadArea.localScale.y, 0);
                mLastPos = mPos;
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                isMouseDragging = false;
            }

            if (isMouseDragging)
            {
                mPos = Input.mousePosition;
                mPos = new Vector3(mPos.x / Screen.width * Screen.width / miniDancepadArea.localScale.x, mPos.y / Screen.height * Screen.height / miniDancepadArea.localScale.y, 0);
                centerOfMass.localPosition = new Vector3(centerOfMass.localPosition.x + mPos.x - mLastPos.x, centerOfMass.localPosition.y + mPos.y - mLastPos.y, 0);
                mLastPos = mPos;
            }

            if(!song.isPlaying)
            {
                isStart = false;
                StartCoroutine(showResult());
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
            if (padNumbers[num - 1].transform.GetChild(0).gameObject.transform.localScale.x / redEffectFullSize > 0.95)
            {
                eMsg.text = "PERFECT";
                playerScore += 10;
            }
            else if (padNumbers[num - 1].transform.GetChild(0).gameObject.transform.localScale.x / redEffectFullSize > 0.8)
            {
                eMsg.text = "GOOD";
                playerScore += 5;
            }
            else
            { eMsg.text = "BAD";
                playerScore += 1;
            }

            inplayScore.text = "Score " + playerScore.ToString();

            Destroy(padNumbers[num - 1].transform.GetChild(0).gameObject);
            Destroy(miniPadNumbers[num - 1].transform.GetChild(0).gameObject);
        }
        else
        {
            eMsg.text = "MISS";
            playerLife -= 1;

            inplayLife.fillAmount = (float)playerLife / playerMaxLife;

            if (playerLife == 0)
            {
                isStart = false;
                StartCoroutine(showResult());
            }
        }

       //StartCoroutine(setEmpty());
    }

    //IEnumerator setEmpty()
    //{
    //    yield return new WaitForSecondsRealtime(beatInterval);
    //    eMsg.text = "";
    //}

    IEnumerator showResult()
    {
        if (song.isPlaying)
        { 
            song.Pause();
            resultText.text = "Fail!";
        }
        else
        {
            resultText.text = "Clear!";
        }

        speaker.PlayOneShot(resultAudio);

        resultScore.text = "Score " + playerScore.ToString();

        resultPanel.SetActive(true);

        yield return new WaitForSeconds(0.1f);
    }

    public void startNoteCreation()
    {
        if (!isStart)
        {
            playerLife = playerMaxLife;
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

    private IEnumerator makeNote(int num)
    {
        GameObject redOb = Instantiate(redEffect, padNumbers[num].transform);
        GameObject miniRedOb = Instantiate(miniRedEffect, miniPadNumbers[num].transform);
        redOb.SetActive(true);
        miniRedOb.SetActive(true);
        redOb.transform.position = padNumbers[num].transform.position;
        miniRedOb.transform.position = miniPadNumbers[num].transform.position;
        float off = (redEffectFullSize - redOb.transform.localScale.x) / 50;

        while (true)
        {
            try
            {
                if (redOb.activeInHierarchy && redOb.transform.localScale.x < redEffectFullSize)
                {
                    redOb.transform.localScale = new Vector3(redOb.transform.localScale.x + off, redOb.transform.localScale.y + off, redOb.transform.localScale.z + off);
                    miniRedOb.transform.localScale = new Vector3(miniRedOb.transform.localScale.x + off, miniRedOb.transform.localScale.y + off, miniRedOb.transform.localScale.z + off);
                }
                else
                    break;
            }
            catch { }

            yield return new WaitForSecondsRealtime(beatInterval / 50 * 2);
        }
        yield return new WaitForSecondsRealtime(beatInterval);

        try
        {
            if (redOb.activeInHierarchy)
            {
                eMsg.text = "MISS";
                playerLife -= 1;

                inplayLife.fillAmount = (float)playerLife / playerMaxLife;

                if (playerLife == 0)
                {
                    isStart = false;
                    StartCoroutine(showResult());
                }

                Destroy(redOb);
                Destroy(miniRedOb);
            }
        }
        catch { }
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
}

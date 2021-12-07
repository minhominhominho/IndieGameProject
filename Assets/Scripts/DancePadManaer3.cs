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
public class DancePadManger3 : MonoBehaviour
{
    private AudioSource song;
    [SerializeField] private string songName = null;
    [SerializeField] private AudioSource speaker = null;
    [SerializeField] private AudioClip resultAudio = null;

    [SerializeField] private Image songBar = null;
    [SerializeField] private TextMeshProUGUI inplayScore = null;
    [SerializeField] private Image inplayLife = null;
    [SerializeField] private Color[] lifeColor = null;
    [SerializeField] private int playerMaxLife;

    [SerializeField] private GameObject resultPanel = null;
    [SerializeField] private TextMeshProUGUI resultText = null;
    [SerializeField] private TextMeshProUGUI resultScore = null;
    private int playerScore = 0;
    private int playerLife;

    private List<Note> notes = new List<Note>();
    private float bpm;
    private float divider;
    private float beatInterval;
    private float makingTime;
    private float startingPoint;
    private int effectTiming = 1;

    [SerializeField] private GameObject playerRightFoot = null;
    [SerializeField] private GameObject playerLeftFoot = null;
    [SerializeField] private List<GameObject> padNumbers = null;
    [SerializeField] private List<GameObject> miniPadNumbers = null;
    [SerializeField] private GameObject redEffect = null;
    [SerializeField] private GameObject miniRedEffect = null;
    [SerializeField] private RectTransform miniDancepadArea = null;
    [SerializeField] private RectTransform centerOfMass = null;
    [SerializeField] private TextMeshProUGUI eMsg = null;
    private float redEffectFullSize = 1f;//0.91172f; // test and define it

    [SerializeField] private GameObject[] effects = null;

    private Vector3 mLastPos;
    private Vector3 mPos;

    private bool isStart = false;
    private bool isMouseDragging = false;
    private bool isLeftFoot = true;

    [SerializeField] private GameObject hipController = null;
    [SerializeField] private PlayerRagdollLogic ragdollLogic = null;

    [HideInInspector] public int rightFoot = 5;
    [HideInInspector] public int leftFoot = 5;

    public void setDancePadActive(bool check)
    {
        isStart = check;
    }

    void Update()
    {
        ///if (isStart)
        if (true)
        {
            if (song)
            {
                if (!song.isPlaying)
                {
                    StartCoroutine(showResult());
                }
                else
                {
                    songBar.fillAmount = song.time / song.clip.length;
                }
            }

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

            playerLeftFoot.transform.position = leftTargetPosition;
            playerRightFoot.transform.position = rightTargetPosition;

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

                // Remap CenterOfMass's position to Hip controller's position
                // CenterOfMass: [-0.75, 0.75], HipController: [-0.44, 0.44]
                if (hipController)
                {
                    hipController.transform.position = new Vector3((float)(centerOfMass.localPosition.x / 0.75 * 0.44), hipController.transform.position.y, (float)(centerOfMass.localPosition.y / 0.75 * 0.44));
                }
            }
        }
    }

    [HideInInspector] public Vector3 leftTargetPosition;
    [HideInInspector] public Vector3 rightTargetPosition;
    public Vector3 targetPosition;
    public bool isLeft = false;
    public bool isRight = false;

    private void inputFunc(int num)
    {
        if (isLeft)
        {
            isLeft = false;
            leftTargetPosition = padNumbers[num - 1].transform.position + new Vector3(0, 0.1f, 0);
        }
        else
        {
            isLeft = true;
            rightTargetPosition = padNumbers[num - 1].transform.position + new Vector3(0, 0.1f, 0);
        }


        // Red effect is stored in each pad tile so when hit the number, clear pad tile's child(red effect)
        if (padNumbers[num - 1].transform.childCount > 0)
        {
            if (padNumbers[num - 1].transform.GetChild(0).gameObject.transform.localScale.x / redEffectFullSize > 0.95)
            {
                GameObject effect = Instantiate(effects[3], padNumbers[num - 1].transform.parent);
                effect.transform.localPosition = padNumbers[num - 1].transform.localPosition;
                effect.SetActive(true);
                eMsg.text = "PERFECT";
                playerScore += 10;
                Destroy(effect, 1f);
            }
            else if (padNumbers[num - 1].transform.GetChild(0).gameObject.transform.localScale.x / redEffectFullSize > 0.8)
            {
                GameObject effect = Instantiate(effects[2], padNumbers[num - 1].transform.parent);
                effect.transform.localPosition = padNumbers[num - 1].transform.localPosition;
                effect.SetActive(true);
                eMsg.text = "GOOD";
                playerScore += 5;
                Destroy(effect, 1f);
            }
            else
            {
                GameObject effect = Instantiate(effects[1], padNumbers[num - 1].transform.parent);
                effect.transform.localPosition = padNumbers[num - 1].transform.localPosition;
                effect.SetActive(true);
                eMsg.text = "BAD";
                playerScore += 1;
                Destroy(effect, 1f);
            }

            inplayScore.text = "Score " + playerScore.ToString();

            Destroy(padNumbers[num - 1].transform.GetChild(0).gameObject);
            Destroy(miniPadNumbers[num - 1].transform.GetChild(0).gameObject);
        }
        else
        {
            damageLife(num-1);
        }
    }


    private void damageLife(int num)
    {
        GameObject effect = Instantiate(effects[0], padNumbers[num].transform.parent);
        effect.transform.localPosition = padNumbers[num].transform.localPosition;
        effect.SetActive(true);
        eMsg.text = "MISS";
        playerLife -= 1;
        Destroy(effect, 1f);

        if (playerLife < playerMaxLife / 2)
            inplayLife.color = lifeColor[1];
        else if (playerLife < playerMaxLife)
            inplayLife.color = lifeColor[0];

        inplayLife.fillAmount = (float)playerLife / playerMaxLife;

        if (playerLife == 0)
        {
            StartCoroutine(showResult());
        }
    }

    IEnumerator showResult()
    {
        isStart = false;
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

    public void startNoteCalaculation()
    {
        playerLife = playerMaxLife;

        song = GetComponent<AudioSource>();
        string filePath = $"{Environment.CurrentDirectory}/Assets/NoteData/{songName}.csv";

        using (Stream s = File.OpenRead(filePath))
        {
            using (StreamReader sr = new StreamReader(s))
            {
                string line = sr.ReadLine();
                bpm = float.Parse(line.Split(',')[0]);
                divider = float.Parse(line.Split(',')[1]);
                startingPoint = float.Parse(line.Split(',')[2]);
                float beatCount = (float)bpm / divider;
                beatInterval = 1 / beatCount;
                makingTime = beatInterval;


                while (makingTime < 0.5f)
                {
                    makingTime += beatInterval;
                    effectTiming++;
                }


                while ((line = sr.ReadLine()) != null)
                {
                    Note note = new Note(int.Parse(line.Split(',')[0]), int.Parse(line.Split(',')[1]));
                    notes.Add(note);
                }
            }
        }
    }

    public void startNoteCreation()
    {
        if (song)
        {
            song.Play();

            for (int i = 0; i < notes.Count; i++)
            {
                StartCoroutine(futureNote(notes[i]));
            }
        }
    }

    private IEnumerator futureNote(Note note)
    {
        yield return new WaitForSecondsRealtime(startingPoint + (note._order - effectTiming) * beatInterval - 0.12f);

        int num = note._noteNum - 1;

        GameObject redOb = Instantiate(redEffect, padNumbers[num].transform);
        GameObject miniRedOb = Instantiate(miniRedEffect, miniPadNumbers[num].transform);
        redOb.SetActive(true);
        miniRedOb.SetActive(true);
        redOb.transform.position = padNumbers[num].transform.position;
        miniRedOb.transform.position = miniPadNumbers[num].transform.position;
        float off = (redEffectFullSize - redOb.transform.localScale.x) / 25;

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

            yield return new WaitForSecondsRealtime(makingTime / 25);
        }

        yield return new WaitForSecondsRealtime(makingTime / 10);
        try
        {
            if (redOb.activeInHierarchy)
            {
                damageLife(num);

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

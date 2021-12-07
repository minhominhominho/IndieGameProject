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
    private AudioSource song;
    public string songName = null;
    public AudioSource speaker = null;
    public AudioClip resultAudio = null;

    public Image songBar = null;
    public TextMeshProUGUI inplayScore = null;
    public Image inplayLife = null;
    public Color[] lifeColor = null;
    public int playerMaxLife;

    public GameObject resultPanel = null;
    public TextMeshProUGUI resultText = null;
    public TextMeshProUGUI resultScore = null;
    private int playerScore = 0;
    private int playerLife;

    private List<Note> notes = new List<Note>();
    private float bpm;
    private float divider;
    private float beatInterval;
    private float makingTime;
    private float startingPoint;
    private int effectTiming = 1;

    public List<GameObject> padNumbers = null;
    public List<GameObject> miniPadNumbers = null;
    public GameObject redEffect = null;
    public GameObject miniRedEffect = null;
    public RectTransform miniDancepadArea = null;
    public RectTransform centerOfMass = null;
    public Color[] centerOfMassColor = null;
    public TextMeshProUGUI eMsg = null;
    private float redEffectFullSize = 1f;//0.91172f; // test and define it

    public GameObject[] effects = null;

    private Vector3 mLastPos;
    private Vector3 mPos;

    private bool isStart = false;
    private bool isFall = false;
    private bool isMouseDragging = false;

    public GameObject playerRightFoot = null;
    public GameObject playerLeftFoot = null;
    public GameObject hipController = null;
    public PlayerRagdollLogic ragdollLogic = null;
    public float hipDistanceOffset = 0.5f;
    [HideInInspector] public int warningCount = 0;

    [HideInInspector] public Vector3 leftTargetPosition;
    [HideInInspector] public Vector3 rightTargetPosition;
    public Vector3 targetPosition;
    public bool isLeft = false;
    public bool isRight = false;

    public void setDancePadActive(bool check)
    {
        isStart = check;
        leftTargetPosition = playerLeftFoot.transform.position;
        rightTargetPosition = playerRightFoot.transform.position;
    }

    void Update()
    {
        if (isStart)
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

            ragdollLogic.leftFootTarget = leftTargetPosition;
            ragdollLogic.rightFootTarget = rightTargetPosition;


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

            Vector3 heapVec = new Vector3(hipController.transform.position.x, 0, hipController.transform.position.z);
            Vector3 leftVec = new Vector3(playerLeftFoot.transform.position.x, 0, playerLeftFoot.transform.position.z);
            Vector3 rightVec = new Vector3(playerRightFoot.transform.position.x, 0, playerRightFoot.transform.position.z);
            if (Vector3.Distance(leftVec, rightVec) + hipDistanceOffset/2 > Vector3.Distance(leftVec, heapVec) + Vector3.Distance(rightVec, heapVec))
            {
                warningCount = 0;
                centerOfMass.GetComponent<Image>().color = centerOfMassColor[0];
            }
            else if (Vector3.Distance(leftVec, rightVec) + hipDistanceOffset > Vector3.Distance(leftVec, heapVec) + Vector3.Distance(rightVec, heapVec))
            {
                warningCount = 0;
                centerOfMass.GetComponent<Image>().color = centerOfMassColor[1];
            }
            else if (Vector3.Distance(leftVec, rightVec) + hipDistanceOffset < Vector3.Distance(leftVec, heapVec) + Vector3.Distance(rightVec, heapVec))
            {
                warningCount++;
                centerOfMass.GetComponent<Image>().color = centerOfMassColor[2];
                if (warningCount > 60)
                {
                    ragdollLogic.Falldown();
                    isFall = true;
                    StartCoroutine(showResult());
                }
            }
        }
    }

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
            if (padNumbers[num - 1].transform.GetChild(0).gameObject.transform.localScale.x / redEffectFullSize > 0.99)
            {
                GameObject effect = Instantiate(effects[3], padNumbers[num - 1].transform.parent);
                effect.transform.localPosition = padNumbers[num - 1].transform.localPosition;
                effect.SetActive(true);
                eMsg.text = "PERFECT";
                playerScore += 10;
                Destroy(effect, 1f);
            }
            else if (padNumbers[num - 1].transform.GetChild(0).gameObject.transform.localScale.x / redEffectFullSize > 0.6)
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
            damageLife(num - 1);
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
            if (isFall)
            {
                resultText.text = "Fall!";
            }
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
        yield return new WaitForSecondsRealtime(startingPoint + (note._order - effectTiming) * beatInterval);   // last float is offset

        int num = note._noteNum - 1;

        GameObject redOb = Instantiate(redEffect, padNumbers[num].transform);
        GameObject miniRedOb = Instantiate(miniRedEffect, miniPadNumbers[num].transform);
        redOb.SetActive(true);
        miniRedOb.SetActive(true);
        redOb.transform.position = padNumbers[num].transform.position;
        miniRedOb.transform.position = miniPadNumbers[num].transform.position;
        float off = (redEffectFullSize - redOb.transform.localScale.x) / 25;
        int check = 0;
        Color c = redOb.GetComponent<Image>().color;

        while (true)
        {
            try
            {
                if (redOb.activeInHierarchy && redOb.transform.localScale.x < redEffectFullSize)
                {
                    if (redOb.transform.localScale.x / redEffectFullSize > 0.6 && check == 0)
                    {
                        check++;
                        redOb.GetComponent<Image>().color = new Vector4(c.r, c.g, c.b, 0.6f);
                        miniRedOb.GetComponent<Image>().color = new Vector4(c.r, c.g, c.b, 0.6f);
                    }
                    else if (redOb.transform.localScale.x / redEffectFullSize > 0.99 && check == 1)
                    {
                        check++;
                        redOb.GetComponent<Image>().color = new Vector4(c.r, c.g, c.b, 1);
                        miniRedOb.GetComponent<Image>().color = new Vector4(c.r, c.g, c.b, 1);
                    }
                    redOb.transform.localScale = new Vector3(redOb.transform.localScale.x + off, redOb.transform.localScale.y + off, redOb.transform.localScale.z + off);
                    miniRedOb.transform.localScale = new Vector3(miniRedOb.transform.localScale.x + off, miniRedOb.transform.localScale.y + off, miniRedOb.transform.localScale.z + off);
                }
                else
                    break;
            }
            catch { }

            yield return new WaitForSecondsRealtime(makingTime / 25);
        }

        yield return new WaitForSecondsRealtime(makingTime / 5);
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

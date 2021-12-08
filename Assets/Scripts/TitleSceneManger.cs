using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleSceneManger : MonoBehaviour
{
    public AudioSource speaker = null;
    public AudioClip clickSound = null;
    public AudioClip titleSound = null;
    public TextAsset songInfoTextAsset = null;
    private string[] songInfoArr;
    [Space]
    [Header("Objects")]
    public GameObject title = null;
    public GameObject menu = null;
    public GameObject stageSelect = null;
    public GameObject[] stagePages = null;
    public GameObject stageSelectLeftButton = null;
    public GameObject stageSelectRighttButton = null;
    public GameObject stageInfo = null;
    public TextMeshProUGUI songName = null;
    public TextMeshProUGUI artist = null;
    public TextMeshProUGUI genre = null;
    public TextMeshProUGUI difficulty = null;
    private int stageIndex = 1;
    public GameObject credit = null;
    public GameObject setting = null;
    public GameObject blackPanel = null;
    public GameObject NumPadMode = null;
    public GameObject QWEASDZXCMode = null;
    public RecordObjectScript RecordObject = null;

    private string currentSelectSong;
    private Coroutine startcoroutine;


    private void Awake()
    {
        title.SetActive(true);
        menu.SetActive(true);
        credit.SetActive(false);
        setting.SetActive(false);
        blackPanel.SetActive(false);

        songInfoArr = songInfoTextAsset.text.Split("\n"[0]);
    }

    private void Start()
    {
        startcoroutine = StartCoroutine(fadeTitle());
    }

    IEnumerator fadeTitle()
    {
        speaker.PlayOneShot(titleSound);
        Image titleBG = title.transform.Find("BG").GetComponent<Image>();
        Image titleLogo = title.transform.Find("Logo").GetComponent<Image>();
        //Image titleBG = title.GetComponentInChildren<Image>();
        //TextMeshProUGUI titleText = title.GetComponentInChildren<TextMeshProUGUI>();

        yield return new WaitForSeconds(3f);
        GetComponent<AudioSource>().Play();

        while (titleBG.color.a > 0)
        {
            titleBG.color = new Color(titleBG.color.r, titleBG.color.g, titleBG.color.b, titleBG.color.a - 0.01f);
            titleLogo.color = new Color(titleLogo.color.r, titleLogo.color.g, titleLogo.color.b, titleLogo.color.a - 0.01f);

            yield return new WaitForSeconds(0.01f);
        }

        title.SetActive(false);
        menu.SetActive(true);
        credit.SetActive(false);
        setting.SetActive(false);
        blackPanel.SetActive(false);
    }

    public void StartButton()
    {
        // speaker.PlayOneShot(clickSound);
        StartCoroutine(startAfterFadeOut(currentSelectSong));
    }

    private IEnumerator startAfterFadeOut(string songTitle)
    {
        blackPanel.SetActive(true);

        blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        while (blackPanel.GetComponent<Image>().color.a <= 1.0f)
        {
            GetComponent<AudioSource>().volume -= 0.005f;
            blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, blackPanel.GetComponent<Image>().color.a + 0.005f);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        SceneManager.LoadScene("Stage_" + songTitle);
    }

    public void StageSelectButton()
    {
        // speaker.PlayOneShot(clickSound);
        menu.SetActive(false);
        stageSelect.SetActive(true);
    }

    public void ShowStageInfoButton(string songIndexName)
    {
        currentSelectSong = songIndexName;
        for (int i = 0; i< songInfoArr.Length; i++)
        {
            string[] temp = songInfoArr[i].Split(","[0]);
            if(temp[0] == songIndexName)
            {
                songName.text = "Song Name : " + temp[1];
                artist.text = "Artist : " + temp[2];
                genre.text = "Genre : " + temp[3];
                difficulty.text = "Difficulty : " + temp[4];
                break;
            }
        }
        stageInfo.SetActive(true);
    }


    public void CloseStageInfoButton()
    {
        stageInfo.SetActive(false);
    }

    public void ShowRightStagesButton()
    {
        stageIndex++;
        stageSelectLeftButton.SetActive(true);
        if (stageIndex == stagePages.Length)
            stageSelectRighttButton.SetActive(false);
        foreach(GameObject page in stagePages)
        {
            page.SetActive(false);
        }

        stagePages[stageIndex - 1].SetActive(true);
    }

    public void ShowLefttStagesButton()
    {
        stageIndex--;
        stageSelectRighttButton.SetActive(true);
        if (stageIndex == 1)
            stageSelectLeftButton.SetActive(false);
        foreach (GameObject page in stagePages)
        {
            page.SetActive(false);
        }

        stagePages[stageIndex - 1].SetActive(true);
    }

    public void SettingButton()
    {
        // speaker.PlayOneShot(clickSound);
        menu.SetActive(false);
        setting.SetActive(true);
    }

    public void ChangeModeButton()
    {
        if (NumPadMode.activeInHierarchy)
        {
            RecordObject.setNumPadMode(false);
            NumPadMode.SetActive(false);
            QWEASDZXCMode.SetActive(true);
        }
        else
        {
            RecordObject.setNumPadMode(true);
            NumPadMode.SetActive(true);
            QWEASDZXCMode.SetActive(false);
        }
    }

    public void CreditButton()
    {
        // speaker.PlayOneShot(clickSound);
        menu.SetActive(false);
        credit.SetActive(true);
    }

    public void BackToTitleButton()
    {
        //  speaker.PlayOneShot(clickSound);
        menu.SetActive(true);
        stageSelect.SetActive(false);
        setting.SetActive(false);
        credit.SetActive(false);
    }

    public void EndButton()
    {
        // speaker.PlayOneShot(clickSound);
        Application.Quit();
    }
}

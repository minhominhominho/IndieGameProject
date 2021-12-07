using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleSceneManger : MonoBehaviour
{
    [SerializeField] private AudioSource speaker = null;
    [SerializeField] private AudioClip clickSound = null;
    [SerializeField] private AudioClip titleSound = null;
    [Space]
    [Header("Objects")]
    [SerializeField] private GameObject title = null;
    [SerializeField] private GameObject menu = null;
    [SerializeField] private GameObject stageSelect = null;
    [SerializeField] private GameObject credit = null;
    [SerializeField] private GameObject setting = null;
    [SerializeField] private GameObject blackPanel = null;

    private Coroutine startcoroutine;


    private void Awake()
    {
        title.SetActive(true);
        menu.SetActive(true);
        credit.SetActive(false);
        setting.SetActive(false);
        blackPanel.SetActive(false);
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

    public void StartButton(string songTitle)
    {
        // speaker.PlayOneShot(clickSound);
        StartCoroutine(startAfterFadeOut(songTitle));
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
        SceneManager.LoadScene(songTitle);
    }

    public void StageSelectButton()
    {
        // speaker.PlayOneShot(clickSound);
        menu.SetActive(false);
        stageSelect.SetActive(true);
    }

    public void SettingButton()
    {
        // speaker.PlayOneShot(clickSound);
        menu.SetActive(false);
        setting.SetActive(true);
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

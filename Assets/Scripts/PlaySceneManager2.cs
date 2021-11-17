using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlaySceneManager2 : MonoBehaviour
{
    [SerializeField] private GameObject blackPanel = null;
    [SerializeField] private TextMeshProUGUI timerText = null;
    [SerializeField] private DancePadManger2 dancePadManger = null;
    [SerializeField] private AudioSource speaker = null;
    [SerializeField] private AudioClip clickAudio = null;
    [SerializeField] private GameObject cam = null;
    [SerializeField] private Vector3 orgCamPos;


    private void Start()
    {
        blackPanel.SetActive(true);
        StartCoroutine(startAfterFadeOut());
    }

    private IEnumerator startAfterFadeOut()
    {
        blackPanel.SetActive(true);

        blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        while (blackPanel.GetComponent<Image>().color.a >= 0.5f)
        {
            blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, blackPanel.GetComponent<Image>().color.a - 0.01f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        StartCoroutine(movieCamera());

        timerText.text = "3";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.text = "2";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.text = "1";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.text = "Start!";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.text = "";
        blackPanel.SetActive(false);

        dancePadManger.startNoteCreation();
    }

    IEnumerator movieCamera()
    {
        Vector3 camPos = cam.transform.position;

        while (true)
        {
            cam.transform.position -= (cam.transform.position - orgCamPos) * 0.02f;

            if (orgCamPos.z + 0.001f < camPos.z && orgCamPos.z - 0.001f < camPos.z) break;

            yield return new WaitForSeconds(0.01f);
        }

    }


    public void retryButton(string songTitle)
    {
        speaker.PlayOneShot(clickAudio);
        StartCoroutine(fadeout(songTitle));
    }

    public void musicSelectButton()
    {
        speaker.PlayOneShot(clickAudio);
        StartCoroutine(fadeout());
    }

    IEnumerator fadeout(string songTitle = "")
    {
        blackPanel.SetActive(true);
        blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        while (blackPanel.GetComponent<Image>().color.a < 1f)
        {
            blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, blackPanel.GetComponent<Image>().color.a + 0.01f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        if (songTitle != "")
        {
            SceneManager.LoadScene(songTitle.ToString());
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
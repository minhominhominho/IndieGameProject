using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManger : MonoBehaviour
{
    [SerializeField]
    private string titleSceneName = null;
    [SerializeField]
    private AudioSource clickSound = null;
    [Space]
    [Header("Objects")]
    [SerializeField]
    private GameObject title = null;
    [SerializeField]
    private GameObject credit = null;
    [SerializeField]
    private GameObject setting = null;
    [SerializeField]
    private GameObject blackPanel = null;
    

    private void Awake()
    {
        title.SetActive(true);
        credit.SetActive(false);
        setting.SetActive(false);
        blackPanel.SetActive(false);
    }

    public void StartButton()
    {
        clickSound.Play();
        StartCoroutine(startAfterFadeOut());
    }

    private IEnumerator startAfterFadeOut()
    {
        blackPanel.SetActive(true);
        Color c = blackPanel.GetComponent<Image>().color;
        c = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        while (c.a <= 1.0f)
        {
            c = new Color(0.0f, 0.0f, 0.0f, c.a + 0.005f);
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadScene(titleSceneName.ToString());
    }

    public void SettingButton()
    {
        clickSound.Play();
        title.SetActive(false);
        setting.SetActive(true);
    }

    public void CreditButton()
    {
        clickSound.Play();
        title.SetActive(false);
        credit.SetActive(true);
    }

    public void BackToTitleButton()
    {
        clickSound.Play();
        title.SetActive(true);
        setting.SetActive(false);
        credit.SetActive(false);
    }

    public void EndButton()
    {
        clickSound.Play();
        Application.Quit();
    }
}

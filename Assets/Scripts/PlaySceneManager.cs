using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlaySceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject blackPanel = null;

    private void Start()
    {
        blackPanel.SetActive(true);
        StartCoroutine(startAfterFadeOut());
    }


    private IEnumerator startAfterFadeOut()
    {
        blackPanel.SetActive(true);

        blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        while (blackPanel.GetComponent<Image>().color.a >= 0.0f)
        {
            blackPanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, blackPanel.GetComponent<Image>().color.a - 0.005f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        blackPanel.SetActive(false);
    }
}

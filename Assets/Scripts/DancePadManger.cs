using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DancePadManger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] character = null;
    [SerializeField]
    private List<GameObject> padNumbers = null;
    [SerializeField]
    private GameObject target = null;
    private List<int> numberSerial = new List<int> { 9,5, 1, 4, 3, 7, 8, 2, 6 };
    // 나중에 게임 매니저 만들어서 거기서 리스트 파라미터로 넘겨주면서 댄스패드매니저의 메소드 부르기
    private int lrCount = 1;
    private bool isStart = false;
    // 나중에 통합


    void Update()
    {
        if (isStart)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                inputFunc(1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                inputFunc(2);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                inputFunc(3);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                inputFunc(4);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                inputFunc(5);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                inputFunc(6);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                inputFunc(7);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                inputFunc(8);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                inputFunc(9);
            }
        }
    }

    private void inputFunc(int num)
    {
        character[0].SetActive(false);

        if (lrCount == 1)
        {
            character[lrCount].SetActive(false);
            lrCount = 2;
        }
        else
        {
            character[lrCount].SetActive(false);
            lrCount = 1;
        }

        character[lrCount].transform.position = padNumbers[num-1].transform.position;
        character[lrCount].SetActive(true);

        if (padNumbers[num-1].transform.childCount > 0)
        {
            Debug.Log("Clear " + num + "!");
            Destroy(padNumbers[num-1].transform.GetChild(0).gameObject);
        }
        else
            Debug.Log("Wrong input " + num + "!");

        
    }


    public void startButton()
    {
        if (!isStart)
        {
            isStart = true;
            StartCoroutine(dancePadControl(numberSerial));
        }
    }

    IEnumerator dancePadControl(List<int> list)
    {
        foreach (int n in list)
        {
            StartCoroutine(fillRed(n - 1));
            yield return new WaitForSeconds(0.7f);
        }

    }

    IEnumerator fillRed(int num)
    {
        GameObject redOb = Instantiate(target, padNumbers[num].transform);
        redOb.SetActive(true);

        redOb.transform.position = padNumbers[num].transform.position;

        while (true)
        {
            try
            {
                if (redOb.activeInHierarchy && redOb.transform.localScale.x < 0.9f)
                {
                    redOb.transform.localScale = new Vector3(redOb.transform.localScale.x + 0.01f, redOb.transform.localScale.y + 0.01f, redOb.transform.localScale.z + 0.01f);
                }
            }
            catch { }

            yield return new WaitForSeconds(0.01f);
        }
    }
}

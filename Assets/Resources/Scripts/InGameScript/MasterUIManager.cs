using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterUIManager : MonoBehaviour
{
    [SerializeField]
    private Text GameTimer;
    [SerializeField]
    private GameObject DeadText;
    [SerializeField]
    private GameObject CountText;
    [SerializeField]
    private GameObject TimerLayer;
    [SerializeField]
    private GameObject GameSetLayer;
    [SerializeField]
    private Text[] PlayerRank;
    [SerializeField]
    private GameObject CountLayer;
    [SerializeField]
    private GameObject ResumeLayer;
    [SerializeField]
    private GameObject MainHealthBar;

    private int CurrentTime;
    private bool isMouseVisible = true;
    public bool IsMouseVisible
        { get { return isMouseVisible; } }
    private bool IsGameEnd;

    void Start()
    {
        CurrentTime = StaticObjects.GamePlayTime;

        // 마우스 초기화
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        SetMouseLockInScene();
    }

    private void SetMouseLockInScene()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsGameEnd)
        {
            isMouseVisible = !isMouseVisible;

            Cursor.visible = isMouseVisible;
            if (isMouseVisible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            ResumeLayer.SetActive(isMouseVisible);
        }
    }

    public IEnumerator ActivateDeadScene(float _cnt)
    {
        DeadText.SetActive(true);
        CountLayer.SetActive(true);
        while (_cnt > 0)
        {
            CountText.GetComponent<Text>().text = _cnt + "";
            CountText.GetComponent<Animator>().SetTrigger("Count");
            yield return new WaitForSeconds(1f);
            --_cnt;
        }
        DeadText.SetActive(false);
        CountLayer.SetActive(false);
    }

    public IEnumerator ActivateGameStart(float _cnt)
    {
        GameTimer.GetComponent<Text>().text = CurrentTime / 60 + " : " + CurrentTime % 60;
        while (_cnt > 0)
        {
            CountText.GetComponent<Text>().text = _cnt + "";
            CountText.GetComponent<Animator>().Play("CountDown_Start", -1, 0f);
            yield return new WaitForSeconds(1f);
            --_cnt;
        }
        isMouseVisible = false;
        TimerLayer.SetActive(true);
        CountLayer.SetActive(false);
        MainHealthBar.SetActive(true);
    }

    public IEnumerator ClockingGameTimeCoroutine()
    {
        Text _timer = GameTimer.GetComponent<Text>();
        int _sec = CurrentTime % 60;
        int _min = CurrentTime / 60;
        _timer.text = _min + " : " + _sec;
        while (_min >= 0)
        {
            yield return new WaitForSeconds(1f);
            --_sec;

            if (_sec < 0)
            {
                _sec = 59;
                --_min;
            }
            _timer.text = _min + " : " + _sec;
        }

        TimerLayer.SetActive(false);
        GameSetLayer.SetActive(true);
        MainHealthBar.SetActive(false);

        IsGameEnd = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }

    public void ViewPlayerKillList(Dictionary<string, int> KillDict)
    {
        var _dict = from pair in KillDict orderby pair.Value descending
                    select pair;

        int i = 0;
        foreach (KeyValuePair<string, int> _pair in _dict)
        {
            PlayerRank[i].text = _pair.Key.Split('_')[0];
            if (KillDict.Count > i) i++;
        }
    }
}

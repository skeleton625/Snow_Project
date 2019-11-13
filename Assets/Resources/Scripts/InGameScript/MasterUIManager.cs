using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MasterUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CountScene;
    [SerializeField]
    private GameObject DeadText;
    [SerializeField]
    private GameObject CountText;
    [SerializeField]
    private GameObject ResumeScene;
    [SerializeField]
    private GameObject HealthBar;

    private int MasterPlayerNum;
    private UIController MasterUI;
    private bool MouseVisible = true;

    void Start()
    {
        MasterPlayerNum = 
            GameObject.Find("StaticObjects").GetComponent<PlayerManager>().MasterPlayerNum;
        MasterUI = new UIController(GameObject.Find(MasterPlayerNum+""), HealthBar);

        MouseVisible = false;
        Cursor.visible = MouseVisible;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLockInScene();
        MasterUI.SetPlayerHealthBar();
    }

    private void MouseLockInScene()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MouseVisible = !MouseVisible;

            Cursor.visible = MouseVisible;
            if (MouseVisible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            ResumeScene.SetActive(MouseVisible);
        }
    }

    public void VisibleCountScene(float _cnt, bool _isDead)
    {
        StartCoroutine(CountdownCoroutine(_cnt, _isDead));
    }

    private IEnumerator CountdownCoroutine(float _cnt, bool _isDead)
    {
        if (_isDead)
            DeadText.SetActive(true);

        CountScene.SetActive(true);

        yield return new WaitForSeconds(_cnt);

        CountScene.SetActive(false);

        if (_isDead)
            DeadText.SetActive(false);
    }
}

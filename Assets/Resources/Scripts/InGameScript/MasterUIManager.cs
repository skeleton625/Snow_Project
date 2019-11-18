using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    private bool isMouseVisible = true;
    public bool IsMouseVisible
        { get { return isMouseVisible; } }

    void Start()
    {
        InGameObjects staticObject = 
            GameObject.Find("StaticObjects").GetComponent<InGameObjects>();
        GameObject model = staticObject.GetPlayerModels(StaticObjects.MasterPlayerNumber);
        StartCoroutine(ActivateCountScene(3, model));

        // 마우스 초기화
        Cursor.visible = isMouseVisible;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLockInScene();
    }

    private void MouseLockInScene()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseVisible = !isMouseVisible;

            Cursor.visible = isMouseVisible;
            if (isMouseVisible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            ResumeScene.SetActive(isMouseVisible);
        }
    }

    public IEnumerator ActivateDeadScene(float _cnt)
    {
        DeadText.SetActive(true);
        CountScene.SetActive(true);
        while (_cnt > 0)
        {
            CountText.GetComponent<Text>().text = _cnt + "";
            CountText.GetComponent<Animator>().SetTrigger("Count");
            yield return new WaitForSeconds(1f);
            --_cnt;
        }
        CountScene.SetActive(false);
        DeadText.SetActive(false);
    }

    private IEnumerator ActivateCountScene(float _cnt, GameObject _model)
    {
        while (_cnt > 0)
        {
            CountText.GetComponent<Text>().text = _cnt + "";
            CountText.GetComponent<Animator>().Play("CountDown_Start", -1, 0f);
            yield return new WaitForSeconds(1f);
            --_cnt;
        }
        isMouseVisible = false;
        CountScene.SetActive(false);
    }
}

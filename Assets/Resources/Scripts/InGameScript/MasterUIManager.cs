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
    [SerializeField]
    private GameObject HealthBar;

    private UIController MasterUI;
    private bool isMouseVisible;
    public bool IsMouseVisible
        { get { return isMouseVisible; } }

    void Start()
    {
        InGameObjects staticObject = 
            GameObject.Find("StaticObjects").GetComponent<InGameObjects>();
        GameObject model = staticObject.GetPlayerModels(StaticObjects.MasterPlayerNumber);
        MasterUI = new UIController(model, HealthBar);

        isMouseVisible = false;
        Cursor.visible = isMouseVisible;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(ActivateCountScene(3, model));
        GameObject _player = GameObject.Find(StaticObjects.MasterPlayerNumber + "");
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
        //_model.GetComponent<PlayerController>().enabled = false;
        //_model.GetComponent<AttackController>().enabled = false;
        while (_cnt > 0)
        {
            CountText.GetComponent<Text>().text = _cnt + "";
            CountText.GetComponent<Animator>().Play("CountDown_Start", -1, 0f);
            yield return new WaitForSeconds(1f);
            --_cnt;
        }
        //_model.GetComponent<PlayerController>().enabled = true;
        //_model.GetComponent<AttackController>().enabled = true;
        CountScene.SetActive(false);
    }
}

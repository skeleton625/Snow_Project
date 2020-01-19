using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Popups;
    [SerializeField]
    private GameObject[] UI;
    [SerializeField]
    private GameObject[] RoomButtons;
    [SerializeField]
    private GameObject CharModels;
    [SerializeField]
    private GameObject CharSelected;

    [SerializeField]
    private InputField UserNameInput;
    [SerializeField]
    private InputField RoomNameInput;
    [SerializeField]
    private InputField PlayTimeInput;

    [SerializeField]
    private Text UserNameText;
    [SerializeField]
    private Text RoomHeaderText;

    [SerializeField]
    private Text[] PopupsText;
    [SerializeField]
    private Text[] RoomButtonText;
    [SerializeField]
    private Text[] PlayerNamesText;
    [SerializeField]
    private Image[] PlayerNamesImage;

    
    public static MenuUIManager instance;

    private void Awake()
    {
        // 자기 스크립트 싱글톤 설정
        instance = this;
    }

    public string GetInputPlayerName()
    {
        string _name = UserNameInput.text;
        // 이름을 입력하지 않았을 시, "NoName"으로 명명
        if (_name == "")
            _name = "NoName";
        // 플레이어가 입력한 이름을 메인 로비 플레이어 텍스트에 입력
        SetPlayerNameInLobby(_name);
        StaticObjects.MasterPlayerName = _name;
        // 플레이어가 입력한 이름을 반환
        return _name;
    }

    public string GetInputRoomName()
    {
        StaticObjects.GamePlayTime = int.Parse(PlayTimeInput.text);
        return RoomNameInput.text;
    }

    public void SetPlayerNameInLobby(string _name)
    {
        UserNameText.text = _name;
    }

    public void SetPreRoomName(string _name, int _len)
    {
        RoomHeaderText.text = _name;
        for(int i = 0; i < 4; i++)
        {
            if (i >= _len)
            {
                SetPlayerNameInRoom(i, "None");
                SetPlayerReadyInRoom(i, false);
            }
        }
    }

    public void SetUIActive(int _num, bool _isActive)
    {
        if (_num == 3)
            CharModels.SetActive(true);
        else
            CharModels.SetActive(false);
        UI[_num].SetActive(_isActive);
    }

    public void DisplayPreRoomButtons(List<string> _rooms)
    {
        for (int i = 0; i < RoomButtons.Length; i++)
            RoomButtons[i].SetActive(false);

        for (int i = 0; i < _rooms.Count; i++)
        {
            RoomButtons[i].SetActive(true);
            RoomButtons[i].name = _rooms[i];
            RoomButtonText[i].text = _rooms[i];
        }

    }

    public void InitPlayerReadyInRoom()
    {
        for (int i = 0; i < 4; i++)
        {
            PlayerNamesImage[i].color = Color.white;
            PlayerNamesText[i].text = "None";
        }
    }

    public void SetPlayerNameInRoom(int _playerNum, string _name)
    {
        PlayerNamesText[_playerNum].text = _name;
    }

    public void SetPlayerReadyInRoom(int _playerNum, bool _isReady)
    {
        if (_isReady)
        {
            PlayerNamesImage[_playerNum].color = Color.green;
            if (_playerNum == StaticObjects.MasterPlayerNum)
                CharSelected.SetActive(true);
        }
        else
        {
            PlayerNamesImage[_playerNum].color = Color.white;
            if (_playerNum == StaticObjects.MasterPlayerNum)
                CharSelected.SetActive(false);
        }
            
    }

    public void VerifyPopup(int _num, string _inform)
    {
        switch (_num)
        {
            case 1:
                Popups[0].SetActive(true);
                PopupsText[0].text = _inform;
                break;
            case 3:
                Popups[1].SetActive(true);
                PopupsText[1].text = _inform;
                break;
            default:
                Debug.Log("Unexpected Error Form");
                break;
        }
    }
}

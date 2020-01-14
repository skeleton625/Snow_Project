using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InMenuObjectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] PlayerModels;
    private int PreModelNum;

    public static InMenuObjectManager instance;

    private void Awake()
    {
        instance = this;
        PreModelNum = 0;
    }

    public void SetActivateModel(bool _isLeft)
    {
        // 현재 활성화된 플레이어 모델 비활성화
        PlayerModels[PreModelNum].SetActive(false);
        // 방향에 따른 다음 플레이어 모델 번호 계산
        if (_isLeft)
            PreModelNum = PreModelNum - 1 < 0 ? PlayerModels.Length - 1 : PreModelNum - 1;
        else
            PreModelNum = PreModelNum + 1 == PlayerModels.Length ? 0 : PreModelNum + 1;
        // 결정된 플레이어 모델 활성화
        PlayerModels[PreModelNum].SetActive(true);
    }

    public void DesideActivateModel()
    {
        // 현재 플레이어가 선택한 모델을 게임 모델로 정의
        StaticObjects.MasterPlayerModelNum = PreModelNum;
    }
}

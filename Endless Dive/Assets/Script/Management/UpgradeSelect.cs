using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption
{
    public Action action;//선택했을 때 실행될 함수
    public string name;//선택지 이름
    public string description;//선택지 설명

    public UpgradeOption(Action action, string name, string description)
    {
        this.action = action;
        this.name = name;
        this.description = description;
    }
}

public class UpgradeSelect : MonoBehaviour
{
    [SerializeField] GameObject player;//플레이어
    List<UpgradeOption> choices;//선택지들
    public GameManager GM;//게임메니저

    void Awake()
    {
        GM = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
        choices = new List<UpgradeOption>()//선택지들 초기화
        {
            new UpgradeOption(UpgradeATK, "공격력 Up", "공격력 +1"),
        };
    }

    void OnEnable()//활성화시 랜덤으로 이벤트 선택
    {
        SetFuction();
    }

    void SetFuction()//선택지의 기능 및 텍스트들을 바꿔주는 함수
    {
        int randIndex = UnityEngine.Random.Range(0, choices.Count);

        transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();

        transform.GetChild(0).GetComponent<Text>().text = choices[randIndex].name;
        transform.GetChild(1).GetComponent<Text>().text = choices[randIndex].description;
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => choices[randIndex].action());
    }

    public void UpgradeATK()//공격력 업그레이드
    {
        // Debug.Log($"공격력(업글 전) : {player.GetComponent<PlayerStat>().ATK.FinalValue}");
        player.GetComponent<PlayerStat>().ATK.AddModifier(new StatModifier(1, StatModType.Flat, this));
        // Debug.Log($"공격력(업글 후) :  {player.GetComponent<PlayerStat>().ATK.FinalValue}");
        Complete();
    }

    void Complete()//해당 오브젝트 비활성화 및 다음 업그레이드 선택지를 활성화 시켜 줌
    {
        transform.parent.gameObject.SetActive(false);
        GM.UpgradeOn();
    }
}

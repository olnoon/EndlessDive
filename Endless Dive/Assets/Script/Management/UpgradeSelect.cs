using System;
using System.Collections.Generic;
using Unity.Mathematics;
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
        choices = new List<UpgradeOption>()//선택지들 초기화
        {
            new UpgradeOption(UpgradeATK, "공격력 Up", "공격력 +1"),
        };
    }

    void Start()
    {
        player = FindAnyObjectByType<GameManager>().players[0];
        SetFuction();
    }

    void SetFuction()//선택지의 기능 및 텍스트들을 바꿔주는 함수
    {
        transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        
        int index = transform.GetSiblingIndex();

        PlayerSkill skill = player.GetComponents<PlayerSkill>()[index];

        var data = FindAnyObjectByType<DataManager>().GetLevelData(skill.skillSOs[0].skillLvl+1);

        transform.GetChild(0).GetComponent<Text>().text = $"{skill.skillSOs[0].skillType} 업그레이드";
        transform.GetChild(1).GetComponent<Text>().text = $"현재 레벨 : {skill.skillSOs[0].skillLvl}\n필요한 자원 : {data.level_up_RequiredEnergy}";//\n{choices[randIndex].description}";

        if (player.GetComponent<PlayerStat>().currentAether < data.level_up_RequiredEnergy)
        {
            transform.GetChild(2).GetComponent<Button>().interactable = false;
            return;
        }
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("111111");
            skill.skillSOs[0].skillLvl++;
            player.GetComponent<PlayerStat>().currentAether -= Mathf.RoundToInt(data.level_up_RequiredEnergy);
            foreach (Transform child in transform.parent)
            {
                child.GetComponent<UpgradeSelect>().SetFuction();
            }
        });
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

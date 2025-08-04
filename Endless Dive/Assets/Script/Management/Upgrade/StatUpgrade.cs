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


public class StatUpgrade : MonoBehaviour
{
    [SerializeField] GameObject player;//플레이어
    public GameManager GM;//게임메니저
    List<UpgradeOption> choices;//선택지들
    public GameObject levelUpgrader;
    public Text remainMineralText;
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
    }

    public void SetStatUpgrade()
    {
        int randIndex = UnityEngine.Random.Range(0, choices.Count);

        transform.GetChild(0).GetComponent<Text>().text = choices[randIndex].name;
        transform.GetChild(1).GetComponent<Text>().text = choices[randIndex].description;
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            choices[randIndex].action();
            Complete();
        });
    }

    public void UpgradeATK()//공격력 업그레이드
    {
        // Debug.Log($"공격력(업글 전) : {player.GetComponent<PlayerStat>().ATK.FinalValue}");
        player.GetComponent<PlayerStat>().ATK.AddModifier(new StatModifier(1, StatModType.Flat, this));
        // Debug.Log($"공격력(업글 후) :  {player.GetComponent<PlayerStat>().ATK.FinalValue}");
    }

    void Complete()//해당 오브젝트 비활성화 및 다음 업그레이드 선택지를 활성화 시켜 줌
    {
        levelUpgrader.transform.parent.gameObject.SetActive(true);
        levelUpgrader.GetComponent<LevelUpgrade>().SetLevelUpgrade();
        transform.parent.parent.gameObject.SetActive(false);
        GM.ShowMinerals(remainMineralText);
    }
}

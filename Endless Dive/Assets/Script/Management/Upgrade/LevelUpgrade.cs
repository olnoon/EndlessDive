using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpgrade : MonoBehaviour
{
    [SerializeField] GameObject player;//플레이어
    public GameManager GM;//게임메니저
    public GameObject statUpgraderParent;
    public Text remainMineralText;

    void Awake()
    {
        GM = FindAnyObjectByType<GameManager>();
        GM.ShowMinerals(remainMineralText);
    }

    void Start()
    {
        player = FindAnyObjectByType<GameManager>().players[0];
        SetFuction();
    }

    void SetFuction()//선택지의 기능 및 텍스트들을 바꿔주는 함수
    {
        SetLevelUpgrade();
        // transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();

        // int index = transform.GetSiblingIndex();

        // PlayerSkill skill = player.GetComponents<PlayerSkill>()[index];

        // var data = FindAnyObjectByType<DataManager>().GetLevelData(skill.skillSOs[0].skillLvl+1);

        // transform.GetChild(0).GetComponent<Text>().text = $"{skill.skillSOs[0].skillType} 업그레이드";
        // transform.GetChild(1).GetComponent<Text>().text = $"현재 레벨 : {skill.skillSOs[0].skillLvl}\n필요한 자원 : {data.level_up_RequiredEnergy}";//\n{choices[randIndex].description}";

        // if (player.GetComponent<PlayerStat>().currentAether < data.level_up_RequiredEnergy)
        // {
        //     transform.GetChild(2).GetComponent<Button>().interactable = false;
        //     return;
        // }
        // transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        // {
        //     skill.skillSOs[0].skillLvl++;
        //     player.GetComponent<PlayerStat>().currentAether -= Mathf.RoundToInt(data.level_up_RequiredEnergy);
        //     foreach (Transform child in transform.parent)
        //     {
        //         child.GetComponent<UpgradeSelect>().SetFuction();
        //     }
        // });
    }

    public void SetLevelUpgrade()
    {
        transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();

        PlayerStat stat = player.GetComponent<PlayerStat>();

        var data = FindAnyObjectByType<DataManager>().GetLevelData(stat.Level+1);

        transform.GetChild(0).GetComponent<Text>().text = $"레벨 업!";
        transform.GetChild(1).GetComponent<Text>().text = $"현재 레벨 : {stat.Level}\n필요한 자원 : {data.level_up_RequiredEnergy}";//\n{choices[randIndex].description}";

        if (player.GetComponent<PlayerStat>().currentAether < data.level_up_RequiredEnergy)
        {
            transform.GetChild(2).GetComponent<Button>().interactable = false;
            return;
        }
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            stat.Level++;
            stat.currentAether -= Mathf.RoundToInt(data.level_up_RequiredEnergy);
            Complete();
        });
    }
    void Complete()//해당 오브젝트 비활성화 및 다음 업그레이드 선택지를 활성화 시켜 줌
    {
        statUpgraderParent.transform.parent.gameObject.SetActive(true);
        foreach (Transform child in statUpgraderParent.transform)
        {
            child.GetComponent<StatUpgrade>().SetStatUpgrade();
        }
        transform.parent.gameObject.SetActive(false);
        GM.ShowMinerals(remainMineralText);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption
{
    public Action action;
    public string name;
    public string description;

    public UpgradeOption(Action action, string name, string description)
    {
        this.action = action;
        this.name = name;
        this.description = description;
    }
}

public class UpgradeSelect : MonoBehaviour
{
    [SerializeField] GameObject player;
    List<UpgradeOption> choices;

    void Awake()
    {
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
        choices = new List<UpgradeOption>()
        {
            new UpgradeOption(UpgradeATK, "공격력 Up", "공격력 +1"),
        };
    }

    void OnEnable()
    {
        SetFuction();
    }

    void SetFuction()//기능 및 텍스트들을 바꿔주는 함수
    {
        int randIndex = UnityEngine.Random.Range(0, choices.Count);

        transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();

        transform.GetChild(0).GetComponent<Text>().text = choices[randIndex].name;
        transform.GetChild(1).GetComponent<Text>().text = choices[randIndex].description;
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => choices[randIndex].action());
    }

    public void UpgradeATK()//공격력 업그레이드
    {
        Debug.Log($"공격력(업그레이드 전) : {player.GetComponent<PlayerStat>().ATK.FinalValue}");
        player.GetComponent<PlayerStat>().ATK.AddModifier(new StatModifier(1, StatModType.Flat, this));
        Debug.Log($"공격력 : {player.GetComponent<PlayerStat>().ATK.FinalValue}");
        Complete();
    }

    void Complete()//해당 오브젝트 비활성화
    {
        FindAnyObjectByType<GameManager>().PauseTime(1);
        transform.parent.gameObject.SetActive(false);
    }
}

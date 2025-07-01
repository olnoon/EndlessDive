using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelect : MonoBehaviour
{
    [SerializeField] GameObject player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerMoveSet>().gameObject;
    }

    void OnEnable()
    {
        SetFuction();
    }

    void SetFuction()//기능 및 텍스트들을 바꿔주는 함수
    {
        transform.GetChild(0).GetComponent<Text>().text = "공격력 Up";
        transform.GetChild(1).GetComponent<Text>().text = "공격력 +1";
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(UpgradeATK);
    }

    public void UpgradeATK()//공격력 업그레이드
    {
        Debug.Log($"공격력(업그레이드 전) : {player.GetComponent<PlayerStat>().stat.atk.FinalValue}");
        player.GetComponent<PlayerStat>().ATK.AddModifier(new StatModifier(1, StatModType.Flat, this));
        Debug.Log($"공격력 : {player.GetComponent<PlayerStat>().stat.atk.FinalValue}");
        Complete();
    }

    void Complete()//해당 오브젝트 비활성화 및 시간 흐르게 하기
    {
        Time.timeScale = 1f;
        transform.parent.gameObject.SetActive(false);
    }
}

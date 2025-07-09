//상황에 따라 함수들을 SO로 옮겨야 할 수도 있음.
using System.Collections;
using UnityEngine;

enum BuffKind
{
    ATK,
    speed,
    maxHP,
    poison
}
public class Buff : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] BuffKind buffKind;
    [SerializeField] bool isBuff;
    [SerializeField] int buffingIntensity;
    void OnTriggerEnter2D(Collider2D collision)//플레이어인지 적인지 판단
    {
        if (collision.gameObject.GetComponent<PlayerMoveSet>() != null)
        {
            BuffPlayer(collision.gameObject);
        }
        else if (collision.gameObject.GetComponent<EnemyMove>() != null)
        {
            BuffEnemy(collision.gameObject);
        }
    }

    void BuffEnemy(GameObject collision)//적 버프
    {
        int sign = isBuff ? 1 : -1;
        switch (buffKind)
        {
            case BuffKind.ATK:
                Debug.Log($"공격력(버프 전) : {collision.GetComponent<EnemyStat>().ATK.FinalValue}");
                collision.GetComponent<EnemyStat>().ATK.AddModifier(new StatModifier(buffingIntensity * sign, StatModType.Flat, this));
                Debug.Log($"공격력(버프 후) :  {collision.GetComponent<EnemyStat>().ATK.FinalValue}");
                break;
            case BuffKind.speed:
                Debug.Log($"속도(버프 전) : {collision.GetComponent<EnemyMove>().speed.FinalValue}");
                collision.GetComponent<EnemyMove>().speed.AddModifier(new StatModifier(buffingIntensity * sign, StatModType.Flat, this));
                Debug.Log($"속도(버프 후) :  {collision.GetComponent<EnemyMove>().speed.FinalValue}");
                break;
            case BuffKind.maxHP:
                collision.GetComponent<EnemyStat>().HP.AddModifier(new StatModifier(buffingIntensity * sign, StatModType.Flat, this));
                break;
        }
        StartCoroutine(ClearBuffEnemy(collision));
    }

    void BuffPlayer(GameObject collision)//플레이어 버프
    {
        int sign = isBuff ? 1 : -1;
        switch (buffKind)
        {
            case BuffKind.ATK:
                Debug.Log($"공격력(버프 전) : {collision.GetComponent<PlayerStat>().ATK.FinalValue}");
                collision.GetComponent<PlayerStat>().ATK.AddModifier(new StatModifier(buffingIntensity * sign, StatModType.Flat, this));
                Debug.Log($"공격력(버프 후) :  {collision.GetComponent<PlayerStat>().ATK.FinalValue}");
                break;
            case BuffKind.speed:
                Debug.Log($"속도(버프 전) : {collision.GetComponent<PlayerMoveSet>().speed.FinalValue}");
                collision.GetComponent<PlayerMoveSet>().speed.AddModifier(new StatModifier(buffingIntensity * sign, StatModType.Flat, this));
                Debug.Log($"속도(버프 후) :  {collision.GetComponent<PlayerMoveSet>().speed.FinalValue}");
                break;
            case BuffKind.maxHP:
                collision.GetComponent<PlayerStat>().HP.AddModifier(new StatModifier(buffingIntensity * sign, StatModType.Flat, this));
                break;
        }
        StartCoroutine(ClearBuffPlayer(collision));
    }

    IEnumerator ClearBuffPlayer(GameObject collision)//지속시간이 끝나면 플레이어의 버프 삭제
    {
        yield return new WaitForSeconds(duration);
        switch (buffKind)
        {
            case BuffKind.ATK:
                Debug.Log($"공격력(해제 전) : {collision.GetComponent<PlayerStat>().ATK.FinalValue}");
                collision.GetComponent<PlayerStat>().ATK.RemoveModifiersFromSource(this);
                Debug.Log($"공격력(해제 후) :  {collision.GetComponent<PlayerStat>().ATK.FinalValue}");
                break;
            case BuffKind.speed:
                Debug.Log($"속도(해제 전) : {collision.GetComponent<PlayerMoveSet>().speed.FinalValue}");
                collision.GetComponent<PlayerMoveSet>().speed.RemoveModifiersFromSource(this);
                Debug.Log($"속도(해제 후) :  {collision.GetComponent<PlayerMoveSet>().speed.FinalValue}");
                break;
            case BuffKind.maxHP:
                collision.GetComponent<PlayerStat>().HP.RemoveModifiersFromSource(this);
                break;
        }
    }

    IEnumerator ClearBuffEnemy(GameObject collision)//지속시간이 끝나면 적의 버프 삭제
    {
        yield return new WaitForSeconds(duration);
        switch (buffKind)
        {
            case BuffKind.ATK:
                Debug.Log($"공격력(해제 전) : {collision.GetComponent<EnemyStat>().ATK.FinalValue}");
                collision.GetComponent<EnemyStat>().ATK.RemoveModifiersFromSource(this);
                Debug.Log($"공격력(해제 후) :  {collision.GetComponent<EnemyStat>().ATK.FinalValue}");
                break;
            case BuffKind.speed:
                Debug.Log($"속도(해제 전) : {collision.GetComponent<EnemyMove>().speed.FinalValue}");
                collision.GetComponent<EnemyMove>().speed.RemoveModifiersFromSource(this);
                Debug.Log($"속도(해제 후) :  {collision.GetComponent<EnemyMove>().speed.FinalValue}");
                break;
            case BuffKind.maxHP:
                collision.GetComponent<EnemyStat>().HP.RemoveModifiersFromSource(this);
                break;
        }
    }
}

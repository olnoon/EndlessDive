using System.Collections;
using UnityEngine;

public class AbnormalStatus : MonoBehaviour
{
    [SerializeField] float damagePerTick;//tickInterval당 들어가는 버프 혹은 디버프의 세기
    [SerializeField] float tickInterval;//한번 버프가 들어갈 때 마다의 시간
    [SerializeField] int repeatCount;//버프가 들어가는 반복 횟수
    [SerializeField] BuffToWhat buffKind;//들어가는 버프의 종류(Ex.HP, 공격력)
    public BuffSO buffSetSO;//해당 스크립트의 변수들의 초깃값을 갖고 있는 SO

    void Start()
    {
        if (buffSetSO != null)
        {
            damagePerTick = buffSetSO.damagePerTick;
            tickInterval = buffSetSO.tickInterval;
            repeatCount = buffSetSO.repeatCount;
            buffKind = buffSetSO.buffKind;
        }
        DetermineBuffTarget();
    }

    void DetermineBuffTarget()//버프대상(해당 스크립트를 갖고 있는 오브젝트)이 EnemyStat스크립트를 갖고 있는 적인지 판단
    {
        if (GetComponent<EnemyStat>() != null)//적일때
        {
            StartCoroutine(DamageToEnemyStat());
        }
        else//그 외
        {
            StartCoroutine(DamageToPlayerStat());
        }
    }

    public IEnumerator DamageToEnemyStat()//repeatCount가 0이 될때 까지 버프/디버프를 주는 while문을 실행후 0이 되면 해당스크립트 삭제
    {
        while (repeatCount > 0)
        {
            switch (buffKind)
            {
                //아직 currentHP밖에 없지만 추가할 예정
                case BuffToWhat.currentHP:
                    GetComponent<EnemyStat>().HP.TakeDamage(Mathf.RoundToInt(damagePerTick));
                    GetComponent<EnemyStat>().DetectDamage();
                break;
            }
            yield return new WaitForSeconds(tickInterval);
            repeatCount--;
        }
        Destroy(this);
    }

    public IEnumerator DamageToPlayerStat()//repeatCount가 0이 될때 까지 버프/디버프를 주는 while문을 실행후 0이 되면 해당 스크립트 삭제
    {
        while (repeatCount > 0)
        {
            switch (buffKind)
            {
                //아직 currentHP밖에 없지만 추가할 예정
                case BuffToWhat.currentHP:
                    GetComponent<EnemyStat>().HP.TakeDamage(Mathf.RoundToInt(damagePerTick));
                    GetComponent<EnemyStat>().DetectDamage();
                break;
            }
            yield return new WaitForSeconds(tickInterval);
            repeatCount--;
        }
        Destroy(this);
    }
    
    // IEnumerator ClearBuffPlayer(GameObject collision)//지속시간이 끝나면 플레이어의 버프 삭제
    // {
    //     yield return new WaitForSeconds(duration);
    //     switch (buffKind)
    //     {
    //         case BuffToWhat.ATK:
    //             Debug.Log($"공격력(해제 전) : {collision.GetComponent<PlayerStat>().ATK.FinalValue}");
    //             collision.GetComponent<PlayerStat>().ATK.RemoveModifiersFromSource(this);
    //             Debug.Log($"공격력(해제 후) :  {collision.GetComponent<PlayerStat>().ATK.FinalValue}");
    //             break;
    //         case BuffToWhat.speed:
    //             Debug.Log($"속도(해제 전) : {collision.GetComponent<PlayerMoveSet>().speed.FinalValue}");
    //             collision.GetComponent<PlayerMoveSet>().speed.RemoveModifiersFromSource(this);
    //             Debug.Log($"속도(해제 후) :  {collision.GetComponent<PlayerMoveSet>().speed.FinalValue}");
    //             break;
    //         case BuffToWhat.maxHP:
    //             collision.GetComponent<PlayerStat>().HP.RemoveModifiersFromSource(this);
    //             break;
    //     }
    // }

    // IEnumerator ClearBuffEnemy(GameObject collision)//지속시간이 끝나면 적의 버프 삭제
    // {
    //     yield return new WaitForSeconds(duration);
    //     switch (buffKind)
    //     {
    //         case BuffToWhat.ATK:
    //             Debug.Log($"공격력(해제 전) : {collision.GetComponent<EnemyStat>().ATK.FinalValue}");
    //             collision.GetComponent<EnemyStat>().ATK.RemoveModifiersFromSource(this);
    //             Debug.Log($"공격력(해제 후) :  {collision.GetComponent<EnemyStat>().ATK.FinalValue}");
    //             break;
    //         case BuffToWhat.speed:
    //             Debug.Log($"속도(해제 전) : {collision.GetComponent<EnemyMove>().speed.FinalValue}");
    //             collision.GetComponent<EnemyMove>().speed.RemoveModifiersFromSource(this);
    //             Debug.Log($"속도(해제 후) :  {collision.GetComponent<EnemyMove>().speed.FinalValue}");
    //             break;
    //         case BuffToWhat.maxHP:
    //             collision.GetComponent<EnemyStat>().HP.RemoveModifiersFromSource(this);
    //             break;
    //     }
    // }
}
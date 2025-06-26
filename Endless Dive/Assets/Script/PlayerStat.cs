using UnityEngine;
using System.Collections.Generic;

public class PlayerStat : MonoBehaviour
{
    public int currectHP;
    public int MaxHP;
    public PlayerStatsSetSO stat;

    void Start()
    {
        SetHP();
    }

    void Update()
    {
        if (currectHP <= 0)
        {
            GameOver();
        }
    }

    void SetHP()//최대체력을 설정해주는 함수
    {
        MaxHP = stat.hp.MaxFinal;
        currectHP = MaxHP;
    }

    void GetDamage(int atk)//대미지 받는 함수
    {
        currectHP -= atk;
    }

    void Attack(GameObject target)//대미지 주는 함수
    {
        target.GetComponent<PlayerStat>().GetDamage(stat.atk.FinalValue);//EnemyStat으로 바꿀 예정
    }

    void GameOver()//게임오버
    {
        Debug.Log("GameOver");
    }
}

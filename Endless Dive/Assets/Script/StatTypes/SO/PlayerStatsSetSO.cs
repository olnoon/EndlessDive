using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Stats Set")]
public class PlayerStatsSetSO : ScriptableObject
{
    [Header("레벨 (Level)")]
    public SingleStatSO Level;
    [Header("체력 (HP)")]
    public GaugeStatSO hp;

    [Header("공격력 (ATK)")]
    public RatioStatSO atk;

    [Header("물리 기반 공격력 (phyATK)")]
    public SingleStatSO phyAtk;

    [Header("에너지 기반 공격력 (enATK)")]
    public SingleStatSO enAtk;

    [Header("치명타확률 (CRI)")]
    public RatioStatSO cri;

    [Header("치명타 대미지 (CATK)")]
    public RatioStatSO catK;

    [Header("쿨타임 (DEX)")]
    public RatioStatSO dex;

    [Header("근접사거리 (MeleeRange)")]
    public RatioStatSO meleeRange;

    [Header("방어력 (ARM)")]
    public SingleStatSO ARM;
    
    [Header("아이템 획득 범위 (PickupRange)")]
    public RatioStatSO pickupRange;
    
    [Header("채굴량 (Mining)")]
    public RatioStatSO mining;
}
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Stats Set")]
public class PlayerStatsSetSO : ScriptableObject
{
    [Header("체력 (HP)")]
    public GaugeStatSO hp;

    [Header("스킬 자원 (Cost)")]
    public GaugeStatSO cost;

    [Header("탄약")]
    public GaugeStatSO ammo;

    [Header("행동력 (AP)")]
    public GaugeStatSO ap;

    [Header("공격력 (ATK)")]
    public SingleStatSO atk;

    [Header("보호막 (Shield)")]
    public SingleStatSO shield;

    [Header("치명타율 (CRI)")]
    public RatioStatSO cri;

    [Header("치명타 대미지 (CRIDAM)")]
    public RatioStatSO criDam;
}
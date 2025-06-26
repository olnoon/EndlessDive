using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Stats Set")]
public class PlayerStatsSetSO : ScriptableObject
{
    [Header("체력 (HP)")]
    public GaugeStatSO hp;

    [Header("공격력 (ATK)")]
    public SingleStatSO atk;

    [Header("치명타율 (CRI)")]
    public RatioStatSO cri;

    [Header("치명타 대미지 (CRIDAM)")]
    public RatioStatSO criDam;
}
//수치상태저장해놓는 거
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/RatioStat")]
public class RatioStatSO : ScriptableObject
{
    [Header("기본 비율")]
    [Range(0f, 1f)]
    public float baseRatio;

    [Header("고정 증가")]
    public float bonusFlat;

    [Header("퍼센트 증가")]
    public float bonusRate;

    [Header("임시 비율")]
    public float tempRatio;

    // 계산된 최종 비율 (0~1로 클램프)
    public float FinalRatio => Mathf.Clamp01((baseRatio + bonusFlat) * (1f + bonusRate) + tempRatio);
}
//버프, 디버프
/// <summary>
/// 능력치 변경 방식 구분용 열거형.
/// Flat: 고정 수치 (예: +20 공격력)
/// Percent: 퍼센트 수치 (예: +10% 공격력)
/// </summary>
public enum StatModType
{
    Flat,     // 고정 수치 증가/감소
    Percent   // 비율 기반 증가/감소 (0.1f → +10%)
}

/// <summary>
/// 버프 또는 디버프 하나를 표현하는 클래스.
/// 능력치에 어떤 방식으로 얼마만큼 영향을 미치는지 정보를 담고 있음.
/// </summary>
public class StatModifier
{
    /// <summary>
    /// 변화량.
    /// - Flat일 경우: 절대값 (예: +30)
    /// - Percent일 경우: 비율 (예: 0.1f → +10%)
    /// </summary>
    public readonly float Value;

    /// <summary>
    /// 변화 방식: 고정 수치(Flat) 또는 퍼센트(Percent)
    /// </summary>
    public readonly StatModType Type;

    /// <summary>
    /// 이 버프/디버프의 출처.
    /// 예: 스킬, 아이템, 상태이상 등. 나중에 제거할 때 어떤 효과인지 구분하는 데 사용됨.
    /// </summary>
    public readonly object Source;

    /// <summary>
    /// 새로운 StatModifier 생성자
    /// </summary>
    /// <param name="value">변화량 (정수든 실수든)</param>
    /// <param name="type">Flat 또는 Percent</param>
    /// <param name="source">버프의 출처 (식별용)</param>
    public StatModifier(float value, StatModType type, object source)
    {
        Value = value;
        Type = type;
        Source = source;
    }
}

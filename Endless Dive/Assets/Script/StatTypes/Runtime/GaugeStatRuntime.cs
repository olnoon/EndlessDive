using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력, 코스트 등 '현재치와 최대치'를 함께 관리하는 자원형 능력치 클래스.
/// 최대치는 버프/디버프에 따라 변동 가능.
/// </summary>
public class GaugeStatRuntime
{
    public int Current { get; private set; } // 현재 자원 수치

    private int baseMax;
    private List<StatModifier> modifiers = new(); // 최대치에 적용할 버프/디버프

    public GaugeStatRuntime(int baseMax)
    {
        this.baseMax = baseMax;
        Current = MaxFinal; // 시작 시 최대치로 설정
    }

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
        Current = Mathf.Min(Current, MaxFinal); // 최대치가 줄어들 수도 있으니 제한
    }

    public void RemoveModifiersFromSource(object source)
    {
        modifiers.RemoveAll(m => m.Source == source);
        Current = Mathf.Min(Current, MaxFinal);
    }

    /// <summary>
    /// 최종 계산된 최대치 (버프 포함)
    /// </summary>
    public int MaxFinal
    {
        get
        {
            float final = baseMax;
            float percentBonus = 0f;

            foreach (var mod in modifiers)
            {
                if (mod.Type == StatModType.Flat)
                    final += mod.Value;
                else if (mod.Type == StatModType.Percent)
                    percentBonus += mod.Value;
            }

            final *= (1f + percentBonus);
            return Mathf.FloorToInt(final);
        }
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(Current + amount, MaxFinal);
    }

    public void TakeDamage(int amount)
    {
        Current = Mathf.Max(Current - amount, 0);
    }

    public void ResetToMax()
    {
        Current = MaxFinal;
    }
}

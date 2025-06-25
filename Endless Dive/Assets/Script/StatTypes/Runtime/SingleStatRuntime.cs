using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격력, 방어력 등 '최대치 개념이 없는' 능력치를 계산하는 클래스.
/// 외부 버프/디버프에 따라 수치가 동적으로 변화함.
/// </summary>
public class SingleStatRuntime
{
    private int baseValue; // ScriptableObject 등에서 가져온 기본 수치
    private List<StatModifier> modifiers = new(); // 적용된 버프/디버프 목록

    public SingleStatRuntime(int baseValue)
    {
        this.baseValue = baseValue;
    }

    /// <summary>
    /// 새로운 버프/디버프를 추가
    /// </summary>
    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
    }

    /// <summary>
    /// 특정 출처(Source)에서 온 모든 버프/디버프 제거
    /// </summary>
    public void RemoveModifiersFromSource(object source)
    {
        modifiers.RemoveAll(m => m.Source == source);
    }

    /// <summary>
    /// 모든 보정을 반영한 최종 수치 반환
    /// </summary>
    public int FinalValue
    {
        get
        {
            float final = baseValue;
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
}

using UnityEngine;

[CreateAssetMenu(fileName = "BuffSO", menuName = "Scriptable Objects/BuffSO")]
public class BuffSO : ScriptableObject
{
    public float damagePerTick;
    public float tickInterval;
    public int repeatCount;
    public BuffToWhat buffKind;
}

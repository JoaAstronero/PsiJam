using UnityEngine;
using System;

public class BuffManager : MonoBehaviour
{
    public static event Action<string> OnBuffApplied;
    public static event Action<string> OnDebuffApplied;

    // Ejemplo de lógica para buffs y debuffs
    public void ApplyBuff(string buffName)
    {
        Debug.Log($"[BuffManager] Buff aplicado: {buffName}");
        OnBuffApplied?.Invoke(buffName);
    }
    public void ApplyDebuff(string debuffName)
    {
        Debug.Log($"[BuffManager] Debuff aplicado: {debuffName}");
        OnDebuffApplied?.Invoke(debuffName);
    }
}

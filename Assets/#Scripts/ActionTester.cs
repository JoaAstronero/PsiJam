using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTester : MonoBehaviour
{
    [Header("Stats")]
    public float maxEnergy = 100f;
    private float currentEnergy;

    private void Start()
    {
        currentEnergy = maxEnergy;
        // Al iniciar, avisamos a la UI en qué estado estamos
        GameEvents.OnStat1Changed?.Invoke(currentEnergy, maxEnergy);
    }

    // Esta función se engancha a la acción "Jump" del Input System
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            currentEnergy = Mathf.Max(0, currentEnergy - 10f); // baja de 10 en 10
            Debug.Log($"Jump pressed, energy = {currentEnergy}");
            GameEvents.OnStat1Changed?.Invoke(currentEnergy, maxEnergy);
        }
    }
}

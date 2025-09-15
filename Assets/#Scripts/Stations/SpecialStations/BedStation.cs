using UnityEngine;

public class BedStation : SpecialStation
{
    private GameManager gameManager;

    private void Awake()
    {
        workstationType = GameEnums.WorkstationType.Bed;
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado en la escena.");
        }
    }
    public override void Interact()
    {
        Debug.Log("¡El jugador va a dormir!");
        // Aquí va la lógica de dormir: restaurar energía, avanzar el día, etc.
        gameManager.AdvanceDay();
        // Puedes mostrar una animación, UI, etc.
    }
}
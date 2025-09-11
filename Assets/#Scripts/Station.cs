using UnityEngine;

public class Station : MonoBehaviour, IWorkstation
{
    // ====> VARIABLES <====

    // --- Referencias de Componentes ---
    public GameEnums.WorkstationType workstationType;

    // --- Variables Internas ---
    private bool isWorking = false;
    private float workDuration = 0f;
    private GameEnums.ProjectType currentProjectType;

    // --- Referencias a Otros Managers ---
    private GameManager gameManager;

    private void Awake()
    {
        // O usa un patrón Singleton para GameManager
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void StartWork()
    {
        Debug.Log($"[Workstation] Recibida llamada a StartWork");
        if (isWorking) return;
        isWorking = true;
        workDuration = 0f;
        Debug.Log($"Iniciando trabajo en {workstationType}!");
        gameManager.PauseTime();
        // Lógica para mostrar la UI de progreso
    }

    // Se llama al soltar el botón de trabajo
    public void EndWork(float heldTime)
    {
        Debug.Log("[Workstation] Recibida llamada a EndWork.");
        if (!isWorking) return;
        isWorking = false;

        Debug.Log($"Terminando trabajo en {workstationType}. Duración: {workDuration}s");

        // 1. Aquí se calcula el progreso (por ahora, basado en el tiempo mantenido)
        float progressAmount = workDuration * 10; // Ejemplo: 10 de progreso por segundo

        // 2. Notificar al GameManager del progreso
        gameManager.ProgressProject(workstationType, currentProjectType, progressAmount);

        // 3. Notificar al GameManager del tiempo dilatado
        // (Lógica de "Time Dilation" que vimos antes)
        float dilatedTime = Random.Range(heldTime * 0.8f, heldTime * 1.2f);
        gameManager.AddDilatedTime(dilatedTime);

        // 4. Reactivar la escala de tiempo y deshabilitar UI
        gameManager.ResumeTime();
        // Lógica para ocultar la UI de progreso
    }

    void Update()
    {
        if (isWorking)
        {
            workDuration += Time.deltaTime;
            // Aquí puedes actualizar una barra de progreso de la UI
        }
    }

    // Métodos para el cálculo de "Time Dilation" y el tiempo dilatado
    private float CalculateDilatedTime(float progressAmount)
    {
        // ... (Tu lógica de cálculo aquí)
        return 0; // Valor de ejemplo
    }
}

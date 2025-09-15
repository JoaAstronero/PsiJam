using UnityEngine;


public class Workstation : Station, IWorkstation
{
    private bool isWorking = false;
    private float workDuration = 0f;
    private GameEnums.ProjectType currentProjectType;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public override void Interact()
    {
        // Aquí podrías abrir una UI de selección de proyecto, mostrar info, etc.
        Debug.Log("Interactuando con una Workstation.");
    }

    public void StartWork(GameEnums.ProjectType projectType)
    {
        Debug.Log($"[Workstation] Recibida llamada a StartWork");
        if (isWorking) return;
        isWorking = true;
        workDuration = 0f;
        currentProjectType = projectType;
        Debug.Log($"Iniciando trabajo en {workstationType} y de tipo {currentProjectType}!");
        gameManager.PauseTime();
        // Lógica para mostrar la UI de progreso
    }

    public void EndWork(float heldTime)
    {
        if (!isWorking) return;
        isWorking = false;

        float progressAmount = heldTime / 4f * gameManager.GetDefaultModuleDuration();
        // si heldTime = 4h → completas un módulo entero

        gameManager.ProgressProject(workstationType, currentProjectType, progressAmount);

        float dilatedTime = Random.Range(heldTime * 0.8f, heldTime * 1.2f);
        gameManager.AddDilatedTime(dilatedTime);
        gameManager.ResumeTime();
    }
    void Update()
    {
        if (isWorking)
        {
            workDuration += Time.deltaTime;
            // Aquí puedes actualizar una barra de progreso de la UI
        }
    }
}



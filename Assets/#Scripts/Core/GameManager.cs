using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        if (timeManager != null)
            TimeManager.OnTimeChanged += OnTimeTick;
    }

    private void OnDisable()
    {
        if (timeManager != null)
            TimeManager.OnTimeChanged -= OnTimeTick;
    }

    private void OnTimeTick(int day, int hour, int minute)
    {
        // Aquí puedes agregar lógica para actualizar la UI, buffs, etc.
        // Ya no necesitas chequear deadlines aquí.
    }
    // Instancia singleton para acceso global
    public static GameManager Instance;

    // Referencias a managers especializados
    public TimeManager timeManager;
    public ProjectManager projectManager;
    public BuffManager buffManager;

    private void Awake()
    {
        Instance = this;
        // Puedes asignar los managers por inspector o buscarlos en la escena
        if (timeManager == null) timeManager = Object.FindFirstObjectByType<TimeManager>();
        if (projectManager == null) projectManager = Object.FindFirstObjectByType<ProjectManager>();
        if (buffManager == null) buffManager = Object.FindFirstObjectByType<BuffManager>();
        // Inicializar proyectos si corresponde
        if (projectManager != null)
        {
            projectManager.LoadAllProjectsFromJSON();
            projectManager.InitializeActiveProjects();
        }
    }

    // API pública para mantener compatibilidad
    public float GetDefaultModuleDuration() => projectManager?.DefaultModuleDuration ?? 240f;
    public void SetDefaultModuleDuration(float duration)
    {
        if (projectManager != null)
            projectManager.DefaultModuleDuration = duration;
    }
    public void PauseTime() => timeManager?.PauseTime();
    public void ResumeTime() => timeManager?.ResumeTime();
    public void AdvanceDay() => timeManager?.AdvanceToNextDay();
    public void AddDilatedTime(float seconds) => timeManager?.AddDilatedTime(seconds);
    public void ProgressProject(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType, float amount)
        => projectManager?.ProgressProject(workstationType, projectType, amount);
    // Puedes agregar métodos de delegación similares para buffs, etc.
}

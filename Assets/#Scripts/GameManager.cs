using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Instancia singleton para acceso global
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        LoadAllProjectsFromJSON();
        InitializeActiveProjects();
        GameEvents.OnDayChanged += OnDayPassed;
    }

    // Modular: Progresar proyecto (sin parámetros de estación)
    public void ProgressProject(float progress)
    {
        Debug.Log($"[GameManager] Progreso del proyecto (modular): {progress}");
        // Implementa la lógica real de progreso aquí si es necesario
    }

    // Modular: Pausar el tiempo
    public void PauseTime()
    {
        Debug.Log("[GameManager] Tiempo pausado (modular).");
        // Implementa la lógica real de pausa aquí si es necesario
    }

    // Modular: Reanudar el tiempo
    public void ResumeTime()
    {
        Debug.Log("[GameManager] Tiempo reanudado (modular).");
        // Implementa la lógica real de reanudación aquí si es necesario
    }

    // Modular: Añadir tiempo dilatado
    public void AddDilatedTime(float seconds)
    {
        Debug.Log($"[GameManager] Tiempo dilatado añadido: {seconds} segundos (modular).");
        // Implementa la lógica real de suma de tiempo aquí si es necesario
    }

    //================================TIME MANAGMENT================================
    // Eventos para comunicar los cambios en el juego
    public delegate void TimeChangeAction(int newDay, float newTime);
    public static event TimeChangeAction OnTimeChanged;

    public delegate void StatChangeAction(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType, float newProgress);
    public static event StatChangeAction OnProjectProgressed;

    // Variables para la gestión del tiempo
    public float timeScale = 0.05f; // Multiplicador de velocidad del tiempo
    public float dayDurationInSeconds = 120f;
    private float currentTime = 0f;
    private int currentDay = 1;


    public TextAsset projectsJSON;
    // Listas para gestionar los proyectos
    public List<Project> allProjectsInMemory = new List<Project>(); // Todos los proyectos cargados del JSON
    public List<Project> pendingProjects = new List<Project>();     // Proyectos que no están asignados
    // Diccionarios de proyectos activos
    public Dictionary<GameEnums.WorkstationType, Project> activePersonalProjects = new Dictionary<GameEnums.WorkstationType, Project>();
    public Dictionary<GameEnums.WorkstationType, Project> activeFreelanceProjects = new Dictionary<GameEnums.WorkstationType, Project>();

    // (Eliminado Awake duplicado)

    private void InitializeActiveProjects()
    {
        foreach (GameEnums.WorkstationType type in System.Enum.GetValues(typeof(GameEnums.WorkstationType)))
        {
            if (type == GameEnums.WorkstationType.None) continue;

            // Asigna un proyecto personal si está disponible
            var personalProject = allProjectsInMemory.Find(p => p.type == GameEnums.ProjectType.Personal && p.assignedWorkstation == type);
            if (personalProject != null)
            {
                activePersonalProjects[type] = personalProject;
                allProjectsInMemory.Remove(personalProject);
            }

            // Asigna un proyecto freelance si está disponible
            var freelanceProject = allProjectsInMemory.Find(p => p.type == GameEnums.ProjectType.Freelance && p.assignedWorkstation == type);
            if (freelanceProject != null)
            {
                activeFreelanceProjects[type] = freelanceProject;
                allProjectsInMemory.Remove(freelanceProject);
            }
        }
    }

    private void LoadAllProjectsFromJSON()
    {
        ProjectList projectList = JsonUtility.FromJson<ProjectList>(projectsJSON.text);
        foreach (var data in projectList.projects)
        {
            Project newProject = new Project
            {
                projectName = data.projectName,
                totalProgress = data.totalProgress,
                type = data.projectType,
                assignedWorkstation = data.workstationType,
                currentProgress = 0,
                isCompleted = false,
                deadlineDaysLeft = 10
            };
            allProjectsInMemory.Add(newProject);
        }
        // Los proyectos restantes se añaden a la lista de pendientes
        pendingProjects.AddRange(allProjectsInMemory);
        allProjectsInMemory.Clear(); // Limpiamos la lista temporal
    }

    // Método para asignar un nuevo proyecto (a ser llamado en el futuro)
    public void AssignNewProjectToStation(GameEnums.WorkstationType stationType, GameEnums.ProjectType projectType)
    {
        //TODO:
        // Lógica para encontrar un proyecto pendiente y asignarlo
        // a una de las estaciones si su slot está vacío.
    }


    void Update()
    {
        TimeManager();
    }

    void TimeManager()
    {

    }

    private void OnDayPassed(int newDay)
    {
        Debug.Log($"Ha iniciado el Día {newDay}");
        DeadlinesManager();

    }

    private void DeadlinesManager()
    {
        // Iterar sobre los proyectos freelance y gestionar los deadlines
        foreach (var entry in activeFreelanceProjects)
        {
            var project = entry.Value;
            if (!project.isCompleted && project.deadlineDaysLeft > 0)
            {
                project.deadlineDaysLeft--;

                if (project.deadlineDaysLeft <= 0)
                {
                    // El deadline ha vencido, aplica el debuff
                    Debug.Log($"El proyecto '{project.projectName}' ha vencido. ¡Debuff activado!");
                    // Aquí se aplicaría la lógica para el debuff acumulativo
                }
            }
        }
    }

    public void ProgressProject(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType, float amount)
    {
        // Lógica para progresar en los proyectos
        Project projectToUpdate = null;

        if (projectType == GameEnums.ProjectType.Personal && activePersonalProjects.ContainsKey(workstationType))
        {
            projectToUpdate = activePersonalProjects[workstationType];
        }
        else if (projectType == GameEnums.ProjectType.Freelance && activeFreelanceProjects.ContainsKey(workstationType))
        {
            projectToUpdate = activeFreelanceProjects[workstationType];
        }

        if (projectToUpdate != null)
        {
            projectToUpdate.currentProgress += amount;
            // Lanzar evento para que la UI se actualice
            OnProjectProgressed?.Invoke(workstationType, projectType, projectToUpdate.currentProgress);

            if (projectToUpdate.currentProgress >= projectToUpdate.totalProgress)
            {
                projectToUpdate.isCompleted = true;
                // Lógica de recompensa, etc.
            }
        }
    }

}

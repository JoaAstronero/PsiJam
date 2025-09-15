using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    // Ahora solo hay una lista de proyectos pendientes, no buffer temporal
    public List<Project> pendingProjects = new List<Project>();
    public Dictionary<GameEnums.WorkstationType, Project> activePersonalProjects = new Dictionary<GameEnums.WorkstationType, Project>();
    public Dictionary<GameEnums.WorkstationType, Project> activeFreelanceProjects = new Dictionary<GameEnums.WorkstationType, Project>();

    // Eventos para desacoplar lógica
    public static event Action<Project> OnProjectAssigned;
    public static event Action<Project> OnProjectCompleted;
    public static event Action<Project> OnDeadlineExpired;
    public static event Action<Project, int> OnModuleCompleted;
    public delegate void StatChangeAction(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType, float newProgress);
    public static event StatChangeAction OnProjectProgressed;

    public TextAsset projectsJSON;

    [SerializeField] private float defaultModuleDuration = 240f; // en segundos o minutos, lo que decidas

    public float DefaultModuleDuration
    {
        get => defaultModuleDuration;
        set => defaultModuleDuration = value;
    }
    private void OnEnable()
    {
        TimeManager.OnDayPassed += HandleDeadlines;
    }

    private void OnDisable()
    {
        TimeManager.OnDayPassed -= HandleDeadlines;
    }

    // Ahora HandleDeadlines solo necesita el día
    private void HandleDeadlines(int newDay)
    {
        foreach (var entry in activeFreelanceProjects)
        {
            var project = entry.Value;
            if (!project.isCompleted && project.deadlineDaysLeft > 0)
            {
                project.deadlineDaysLeft--;
                if (project.deadlineDaysLeft <= 0)
                {
                    Debug.Log($"El proyecto '{project.projectName}' ha vencido. ¡Debuff activado!");
                    OnDeadlineExpired?.Invoke(project);
                    // Aquí se aplicaría la lógica para el debuff acumulativo
                }
            }
        }
    }
    public void LoadAllProjectsFromJSON()
    {
        ProjectList projectList = JsonUtility.FromJson<ProjectList>(projectsJSON.text);
        Debug.Log($"[ProjectManager] Cargando {projectList.projects.Count} proyectos desde JSON");

        foreach (var data in projectList.projects)
        {
            // Parseo tipos
            GameEnums.ProjectType parsedType = GameEnums.ProjectType.Personal;
            GameEnums.WorkstationType parsedStation = GameEnums.WorkstationType.Alchemy;
            bool typeOk = true, stationOk = true;

            try
            {
                parsedType = (GameEnums.ProjectType)System.Enum.Parse(typeof(GameEnums.ProjectType), data.projectType.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProjectManager] ERROR al parsear ProjectType '{data.projectType}' para '{data.projectName}': {e.Message}");
                typeOk = false;
            }
            try
            {
                parsedStation = (GameEnums.WorkstationType)System.Enum.Parse(typeof(GameEnums.WorkstationType), data.workstationType.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProjectManager] ERROR al parsear WorkstationType '{data.workstationType}' para '{data.projectName}': {e.Message}");
                stationOk = false;
            }
            if (!typeOk || !stationOk) continue;

            // Duración por módulo: si no está definida en JSON, usamos default
            float moduleDuration = data.moduleDuration > 0 ? data.moduleDuration : defaultModuleDuration;

            Project newProject = new Project
            {
                projectName = data.projectName,
                totalModules = data.totalModules > 0 ? data.totalModules : 4, // default mínimo si no lo ponen en JSON

                modulesCompleted = 0,
                currentProgress = 0,
                type = parsedType,
                assignedWorkstation = parsedStation,
                isCompleted = false,
                deadlineDaysLeft = 10
            };

            Debug.Log($"[ProjectManager] Parse result: '{newProject.projectName}' | Tipo: {parsedType} | Estación: {parsedStation} | Módulos: {newProject.totalModules} | Duración módulo: {newProject.totalModules * defaultModuleDuration}s");

            pendingProjects.Add(newProject);
        }
    }


    public void InitializeActiveProjects()
    {
        foreach (GameEnums.WorkstationType type in Enum.GetValues(typeof(GameEnums.WorkstationType)))
        {
            if (type == GameEnums.WorkstationType.None) continue;

            // Asigna un proyecto personal si está disponible
            var personalProject = pendingProjects.Find(p => p.type == GameEnums.ProjectType.Personal && p.assignedWorkstation == type);
            if (personalProject != null)
            {
                activePersonalProjects[type] = personalProject;
                pendingProjects.Remove(personalProject);
                OnProjectAssigned?.Invoke(personalProject);
            }

            // Asigna un proyecto freelance si está disponible
            var freelanceProject = pendingProjects.Find(p => p.type == GameEnums.ProjectType.Freelance && p.assignedWorkstation == type);
            if (freelanceProject != null)
            {
                activeFreelanceProjects[type] = freelanceProject;
                pendingProjects.Remove(freelanceProject);
                OnProjectAssigned?.Invoke(freelanceProject);
            }
        }
    }

    public void ProgressProject(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType, float amount)
    {
        Project projectToUpdate = GetProject(workstationType, projectType);
        if (projectToUpdate == null || projectToUpdate.isCompleted) return;

        float oldProgress = projectToUpdate.currentProgress;
        projectToUpdate.currentProgress += amount;

        // Chequear si completó módulos
        int oldModules = projectToUpdate.modulesCompleted;
        int newModules = Mathf.FloorToInt(projectToUpdate.currentProgress / defaultModuleDuration);
        for (int i = oldModules; i < newModules; i++)
        {
            projectToUpdate.modulesCompleted++;
            OnModuleCompleted?.Invoke(projectToUpdate, projectToUpdate.modulesCompleted);
        }

        // Chequear si completó proyecto
        if (projectToUpdate.currentProgress >= projectToUpdate.totalModules * defaultModuleDuration)
        {
            projectToUpdate.isCompleted = true;
            OnProjectCompleted?.Invoke(projectToUpdate);
        }

        OnProjectProgressed?.Invoke(workstationType, projectType, projectToUpdate.currentProgress);
    }

    public Project GetProject(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType)
    {
        Project projectToReturn = null;
        if (projectType == GameEnums.ProjectType.Personal && activePersonalProjects.ContainsKey(workstationType))
        {
            projectToReturn = activePersonalProjects[workstationType];
        }
        else if (projectType == GameEnums.ProjectType.Freelance && activeFreelanceProjects.ContainsKey(workstationType))
        {
            projectToReturn = activeFreelanceProjects[workstationType];
        }
        return projectToReturn;
    }

    public void AssignNewProjectToStation(GameEnums.WorkstationType stationType, GameEnums.ProjectType projectType)
    {
        // Lógica para encontrar un proyecto pendiente y asignarlo a la estación si su slot está vacío.
        var project = pendingProjects.Find(p => p.type == projectType && p.assignedWorkstation == stationType);
        if (project != null)
        {
            if (projectType == GameEnums.ProjectType.Personal)
                activePersonalProjects[stationType] = project;
            else
                activeFreelanceProjects[stationType] = project;

            pendingProjects.Remove(project);
            OnProjectAssigned?.Invoke(project);
        }
    }

    public List<Project> GetPendingProjects(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType)
    {
        return pendingProjects.FindAll(p => p.assignedWorkstation == workstationType && p.type == projectType);
    }

    public bool HasFreeSlot(GameEnums.WorkstationType workstationType, GameEnums.ProjectType projectType)
    {
        if (projectType == GameEnums.ProjectType.Personal)
            return !activePersonalProjects.ContainsKey(workstationType) || activePersonalProjects[workstationType]?.isCompleted == true;
        else
            return !activeFreelanceProjects.ContainsKey(workstationType) || activeFreelanceProjects[workstationType]?.isCompleted == true;
    }
}

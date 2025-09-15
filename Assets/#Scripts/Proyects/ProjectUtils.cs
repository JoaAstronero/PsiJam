using System.Collections.Generic;

[System.Serializable]
public class ProjectData
{
    public string projectName;
    public string projectType;
    public string workstationType;
    public float moduleDuration;
    public int totalModules;
}

[System.Serializable]
public class ProjectList
{
    public List<ProjectData> projects;
}

[System.Serializable]
public class Project
{
    public string projectName;
    public float currentProgress;
    public int totalModules;
    public int modulesCompleted; // cuántos módulos se han completado
    public GameEnums.ProjectType type;
    public GameEnums.WorkstationType assignedWorkstation;
    public bool isCompleted;
    public int deadlineDaysLeft;

}

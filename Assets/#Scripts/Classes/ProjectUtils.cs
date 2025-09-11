using System.Collections.Generic;

[System.Serializable]
public class ProjectData
{
    public string projectName;
    public GameEnums.ProjectType projectType;
    public GameEnums.WorkstationType workstationType;
    public float totalProgress;
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
    public float totalProgress;
    public GameEnums.ProjectType type;
    public GameEnums.WorkstationType assignedWorkstation;
    public bool isCompleted;
    public int deadlineDaysLeft;
}
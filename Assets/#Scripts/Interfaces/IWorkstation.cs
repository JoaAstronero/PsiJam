public interface IWorkstation
{
    void StartWork(GameEnums.ProjectType projectType);
    void EndWork(float heldTime);
}
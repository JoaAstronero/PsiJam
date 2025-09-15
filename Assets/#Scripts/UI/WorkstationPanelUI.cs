using UnityEngine;

public class WorkstationPanelUI : MonoBehaviour
{
    public GameObject workstationUIPrefab; // Asigna el prefab en el inspector
    public Transform contentParent; // Asigna el panel (con Vertical/Horizontal Layout Group)
    private ProjectManager projectManager;

    public void Setup(ProjectManager pm)
    {
        projectManager = pm;
        // Limpia hijos previos si recargas
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Instancia un UI por cada WorkstationType (excepto None)
        foreach (GameEnums.WorkstationType type in System.Enum.GetValues(typeof(GameEnums.WorkstationType)))
        {
            if (type == GameEnums.WorkstationType.None) continue;
            Debug.Log($"[WorkstationPanelUI] Instanciando UI para {type}");
            var go = Instantiate(workstationUIPrefab, contentParent);
            var wsUI = go.GetComponent<WorkstationUI>();
            if (wsUI == null)
            {
                Debug.LogError($"[WorkstationPanelUI] El prefab no tiene WorkstationUI para {type}");
            }
            else
            {
                wsUI.Setup(type, projectManager);
            }
        }
    }

    void OnEnable()
    {
        // Actualiza cada vez que se abre el panel
        if (projectManager != null)
        {
            Debug.Log("[WorkstationPanelUI] OnEnable: Refrescando panel con ProjectManager asignado");
            Setup(projectManager);
        }
        else
        {
            Debug.LogWarning("[WorkstationPanelUI] OnEnable: ProjectManager es null");
        }
    }
}
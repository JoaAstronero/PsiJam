using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WorkstationUI : MonoBehaviour
{
    public TextMeshProUGUI workstationNameText;
    public TMP_Dropdown personalDropdown;
    public TMP_Dropdown freelanceDropdown;
    public Button assignPersonalButton;
    public Button assignFreelanceButton;
    public TextMeshProUGUI personalProjectText;
    public TextMeshProUGUI freelanceProjectText;

    private GameEnums.WorkstationType workstationType;
    private ProjectManager projectManager;

    public void Setup(GameEnums.WorkstationType type, ProjectManager pm)
    {
        workstationType = type;
        projectManager = pm;
        workstationNameText.text = type.ToString();

        // Personal
        Debug.Log($"[WorkstationUI] Setup para {type} (Personal)");
        if (projectManager.HasFreeSlot(type, GameEnums.ProjectType.Personal))
        {
            var options = projectManager.GetPendingProjects(type, GameEnums.ProjectType.Personal);
            Debug.Log($"[WorkstationUI] {type} Personal: {options.Count} proyectos pendientes");
            foreach (var p in options)
                Debug.Log($"[WorkstationUI] Proyecto pendiente: {p.projectName}");
            personalDropdown.ClearOptions();
            personalDropdown.AddOptions(options.ConvertAll(p => p.projectName));
            personalDropdown.gameObject.SetActive(true);
            assignPersonalButton.gameObject.SetActive(true);
            personalProjectText.gameObject.SetActive(false);
        }
        else
        {
            personalDropdown.gameObject.SetActive(false);
            assignPersonalButton.gameObject.SetActive(false);
            if (projectManager.activePersonalProjects.ContainsKey(type))
            {
                var project = projectManager.activePersonalProjects[type];
                Debug.Log($"[WorkstationUI] {type} Personal ocupado por: {project.projectName}");
                personalProjectText.text = project.projectName;
            }
            else
            {
                Debug.Log($"[WorkstationUI] {type} Personal sin proyecto activo");
                personalProjectText.text = "(Sin proyecto)";
            }
            personalProjectText.gameObject.SetActive(true);
        }

        // Freelance (igual lógica)
        Debug.Log($"[WorkstationUI] Setup para {type} (Freelance)");
        if (projectManager.HasFreeSlot(type, GameEnums.ProjectType.Freelance))
        {
            var options = projectManager.GetPendingProjects(type, GameEnums.ProjectType.Freelance);
            Debug.Log($"[WorkstationUI] {type} Freelance: {options.Count} proyectos pendientes");
            foreach (var p in options)
                Debug.Log($"[WorkstationUI] Proyecto pendiente: {p.projectName}");
            freelanceDropdown.ClearOptions();
            freelanceDropdown.AddOptions(options.ConvertAll(p => p.projectName));
            freelanceDropdown.gameObject.SetActive(true);
            assignFreelanceButton.gameObject.SetActive(true);
            freelanceProjectText.gameObject.SetActive(false);
        }
        else
        {
            freelanceDropdown.gameObject.SetActive(false);
            assignFreelanceButton.gameObject.SetActive(false);
            if (projectManager.activeFreelanceProjects.ContainsKey(type))
            {
                var project = projectManager.activeFreelanceProjects[type];
                Debug.Log($"[WorkstationUI] {type} Freelance ocupado por: {project.projectName}");
                freelanceProjectText.text = project.projectName;
            }
            else
            {
                Debug.Log($"[WorkstationUI] {type} Freelance sin proyecto activo");
                freelanceProjectText.text = "(Sin proyecto)";
            }
            freelanceProjectText.gameObject.SetActive(true);
        }
    }

    // Llama esto desde el botón de asignar
    public void OnAssignPersonal()
    {
        var options = projectManager.GetPendingProjects(workstationType, GameEnums.ProjectType.Personal);
        Debug.Log($"[WorkstationUI] OnAssignPersonal: {options.Count} opciones para {workstationType}");
        if (options.Count > 0)
        {
            projectManager.AssignNewProjectToStation(workstationType, GameEnums.ProjectType.Personal);
            Setup(workstationType, projectManager); // Refresca la UI
        }
    }

    public void OnAssignFreelance()
    {
        var options = projectManager.GetPendingProjects(workstationType, GameEnums.ProjectType.Freelance);
        Debug.Log($"[WorkstationUI] OnAssignFreelance: {options.Count} opciones para {workstationType}");
        if (options.Count > 0)
        {
            projectManager.AssignNewProjectToStation(workstationType, GameEnums.ProjectType.Freelance);
            Setup(workstationType, projectManager); // Refresca la UI
        }
    }
}
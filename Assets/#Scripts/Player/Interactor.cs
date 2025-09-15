using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    // ====> VARIABLES <====

    // --- Referencias de Componentes ---
    private PlayerInput playerInput;
    private Camera playerCamera;

    // --- Referencias de Input Actions ---
    private InputAction workPersonalAction;
    private InputAction workFreelanceAction;

    [Header("Raycast")]
    public float interactDistance = 3f;

    // --- Variables Internas ---
    private IWorkstation currentTarget;
    private float workStartTime;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerCamera = Camera.main;

        // Obtiene las acciones del mapa de Interaction
        workPersonalAction = playerInput.actions["Interaction/WorkPersonal"];
        workFreelanceAction = playerInput.actions["Interaction/WorkFreelance"];
    }

    private void OnEnable()
    {
        // Suscribe los métodos a los eventos de las acciones
        workPersonalAction.started += OnPersonalWorkStarted;
        workPersonalAction.canceled += OnPersonalWorkCanceled;
        workFreelanceAction.started += OnFreelanceWorkStarted;
        workFreelanceAction.canceled += OnFreelanceWorkCanceled;
    }

    private void OnDisable()
    {
        // Desuscribe los métodos cuando el script se deshabilita
        workPersonalAction.started -= OnPersonalWorkStarted;
        workPersonalAction.canceled -= OnPersonalWorkCanceled;
        workFreelanceAction.started -= OnFreelanceWorkStarted;
        workFreelanceAction.canceled -= OnFreelanceWorkCanceled;
    }

    // --- NUEVO: Lógica unificada para interactuar con cualquier estación ---
    private void OnPersonalWorkStarted(InputAction.CallbackContext context)
    {
        //Debug.log("Evento OnPersonalWorkStarted disparado.");
        var interactable = CheckForInteractable();
        if (interactable is IWorkstation workstation)
        {
            //Debug.log("Estación de trabajo detectada. Iniciando trabajo personal.");
            playerInput.actions.FindActionMap("Player").Disable();
            currentTarget = workstation;
            workStartTime = Time.time;
            currentTarget.StartWork(GameEnums.ProjectType.Personal);
        }
        else if (interactable != null)
        {
            //Debug.log("Estación especial detectada. Ejecutando acción instantánea.");
            interactable.Interact();
        }
        else
        {
            //Debug.log("No se detectó ninguna estación. El jugador sigue moviéndose.");
        }
    }

    private void OnPersonalWorkCanceled(InputAction.CallbackContext context)
    {
        //Debug.log("Evento OnPersonalWorkCanceled disparado.");
        if (currentTarget != null)
        {
            playerInput.actions.FindActionMap("Player").Enable();
            float heldTime = Time.time - workStartTime;
            currentTarget.EndWork(heldTime);
            currentTarget = null;
            //Debug.log("Trabajo personal terminado y movimiento del jugador reanudado.");
        }
    }

    private void OnFreelanceWorkStarted(InputAction.CallbackContext context)
    {
        //Debug.log("Evento OnFreelanceWorkStarted disparado.");
        var interactable = CheckForInteractable();
        if (interactable is IWorkstation workstation)
        {
            //Debug.log("Estación de trabajo detectada. Iniciando trabajo freelance.");
            playerInput.actions.FindActionMap("Player").Disable();
            currentTarget = workstation;
            workStartTime = Time.time;
            currentTarget.StartWork(GameEnums.ProjectType.Freelance);
        }
        else if (interactable != null)
        {
            //Debug.log("Estación especial detectada. Ejecutando acción instantánea.");
            interactable.Interact();
        }
        else
        {
            //Debug.log("No se detectó ninguna estación. El jugador sigue moviéndose.");
        }
    }

    private void OnFreelanceWorkCanceled(InputAction.CallbackContext context)
    {
        //Debug.log("Evento OnFreelanceWorkCanceled disparado.");
        if (currentTarget != null)
        {
            playerInput.actions.FindActionMap("Player").Enable();
            float heldTime = Time.time - workStartTime;
            currentTarget.EndWork(heldTime);
            currentTarget = null;
            //Debug.log("Trabajo freelance terminado y movimiento del jugador reanudado.");
        }
    }

    // --- NUEVO: Raycast genérico para cualquier IInteractable ---
    private IInteractable CheckForInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Busca cualquier componente que implemente IInteractable en el objeto o sus padres
            return hit.collider.GetComponentInParent<IInteractable>();
        }
        return null;
    }
}

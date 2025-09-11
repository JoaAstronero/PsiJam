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

    private void OnPersonalWorkStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Evento OnPersonalWorkStarted disparado.");
        var hitStation = CheckForStation();
        if (hitStation != null)
        {
            Debug.Log("Estación detectada. Iniciando trabajo personal.");
            playerInput.actions.FindActionMap("Player").Disable();
            currentTarget = hitStation;
            workStartTime = Time.time;
            currentTarget.StartWork();
        }
        else
        {
            Debug.Log("No se detectó ninguna estación. El jugador sigue moviéndose.");
        }
    }

    private void OnPersonalWorkCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Evento OnPersonalWorkCanceled disparado.");
        if (currentTarget != null)
        {
            playerInput.actions.FindActionMap("Player").Enable();
            float heldTime = Time.time - workStartTime;
            currentTarget.EndWork(heldTime);
            currentTarget = null;
            Debug.Log("Trabajo personal terminado y movimiento del jugador reanudado.");
        }
    }

    private void OnFreelanceWorkStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Evento OnFreelanceWorkStarted disparado.");
        var hitStation = CheckForStation();
        if (hitStation != null)
        {
            Debug.Log("Estación detectada. Iniciando trabajo freelance.");
            playerInput.actions.FindActionMap("Player").Disable();
            currentTarget = hitStation;
            workStartTime = Time.time;
            currentTarget.StartWork();
        }
        else
        {
            Debug.Log("No se detectó ninguna estación. El jugador sigue moviéndose.");
        }
    }

    private void OnFreelanceWorkCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Evento OnFreelanceWorkCanceled disparado.");
        if (currentTarget != null)
        {
            playerInput.actions.FindActionMap("Player").Enable();
            float heldTime = Time.time - workStartTime;
            currentTarget.EndWork(heldTime);
            currentTarget = null;
            Debug.Log("Trabajo freelance terminado y movimiento del jugador reanudado.");
        }
    }

    private IWorkstation CheckForStation()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Busca cualquier componente que implemente IWorkstation en el objeto o sus padres
            return hit.collider.GetComponentInParent<IWorkstation>();
        }
        return null;
    }
}

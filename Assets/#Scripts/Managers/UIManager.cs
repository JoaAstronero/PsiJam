using TMPro;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    [Header("Tiempo")]
    public TextMeshProUGUI calendarText;
    public TextMeshProUGUI clockText;
    public ClockHand hourHand;
    public ClockHand minuteHand;
    [Header("Bars")]
    public StatBar bar1;
    [Header("Gestión de Proyectos")]
    public WorkstationPanelUI workstationPanelUI; // Asigna el panel del menú en el inspector
    private bool gestionMenuActivo = false;
    public PlayerController playerController; // Asigna en el inspector o busca en Awake

    void Start()
    {
        // Solo una vez al inicio, o cuando recargues proyectos
        workstationPanelUI.Setup(GameManager.Instance.projectManager);
    }

    void OnEnable()
    {
        GameEvents.OnStat1Changed += bar1.SetValue;
        TimeManager.OnTimeChanged += OnTimeChanged;
    }

    void OnDisable()
    {
        GameEvents.OnStat1Changed -= bar1.SetValue;
        TimeManager.OnTimeChanged -= OnTimeChanged;
    }
    public bool IsGestionMenuActive() => gestionMenuActivo;

    private void OnTimeChanged(int day, int hour, int minute)
    {
        if (clockText != null)
            clockText.text = $"{hour:D2}:{minute:D2}";
        if (calendarText != null)
            calendarText.text = $"Day {day}";
        if (hourHand != null)
            hourHand.SetTime(hour, minute, 0);
        if (minuteHand != null)
            minuteHand.SetTime(hour, minute, 0);
    }

    public void ToggleGestionMenu()
    {
        gestionMenuActivo = !gestionMenuActivo;
        workstationPanelUI.gameObject.SetActive(gestionMenuActivo);

        if (gestionMenuActivo)
            playerController.ActivarUIMode();
        else
            playerController.ActivarPlayerMode();
    }
}

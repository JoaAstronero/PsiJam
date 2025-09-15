using UnityEngine;

public class TimeManager : MonoBehaviour
{
    /// <summary>
    /// Devuelve el día, la hora y el minuto actuales del juego.
    /// </summary>
    public (int day, int hour, int minute) GetTimeOfDay()
    {
        int totalMinutes = Mathf.FloorToInt((currentTime / dayDurationInSeconds) * 1440f);
        int hour = totalMinutes / 60;
        int minute = totalMinutes % 60;
        return (currentDay, hour, minute);
    }
    [Header("Configuración de tiempo")]
    public float timeScale = 0.05f;
    public float dayDurationInSeconds = 120f;
    public int tickMinutes = 1; // Cada cuántos minutos lanzar el evento (1 = cada minuto, 10 = cada 10 minutos)
    public float dayStartHour = 8f; // Hora a la que empieza el día (0-23)

    private float currentTime = 0f;
    private int currentDay = 1;
    private int lastTickMinute = 0;
    private bool paused = false;

    public delegate void TimeChangeAction(int day, int hour, int minute);
    public static event TimeChangeAction OnTimeChanged;

    public delegate void DayPassedAction(int newDay);
    public static event DayPassedAction OnDayPassed;

    public void AdvanceToNextDay()
    {
        if (paused) return;
        currentTime = dayStartHour * (dayDurationInSeconds / 24f);
        currentDay++;
        Debug.Log($"[TimeManager] Avanzando al siguiente día: {currentDay}");
        OnDayPassed?.Invoke(currentDay);
    }

    public void PauseTime()
    {
        paused = true;
        Debug.Log("[TimeManager] Tiempo pausado.");
    }

    public void ResumeTime()
    {
        paused = false;
        Debug.Log("[TimeManager] Tiempo reanudado.");
    }

    public void AddDilatedTime(float seconds)
    {
        currentTime += seconds;
        Debug.Log($"[TimeManager] Tiempo dilatado añadido: {seconds} segundos.");
    }

    void Update()
    {
        if (paused) return;
        int prevDay = currentDay;
        currentTime += Time.deltaTime * timeScale;
        if (currentTime >= dayDurationInSeconds)
        {
            currentTime -= dayDurationInSeconds;
            currentDay++;
            Debug.Log($"[TimeManager] Nuevo día: {currentDay}");
            OnDayPassed?.Invoke(currentDay);
        }
        int totalMinutes = Mathf.FloorToInt((currentTime / dayDurationInSeconds) * 1440f);
        int hour = totalMinutes / 60;
        int minute = totalMinutes % 60;
        if (totalMinutes / tickMinutes != lastTickMinute / tickMinutes)
        {
            lastTickMinute = totalMinutes;
            OnTimeChanged?.Invoke(currentDay, hour, minute);
            //Debug.Log($"[TimeManager] Tick de tiempo: Día {currentDay}, Hora {hour}, Minuto {minute}");
        }
    }
}

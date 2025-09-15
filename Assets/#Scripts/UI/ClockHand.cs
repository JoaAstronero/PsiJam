using UnityEngine;

public class ClockHand : MonoBehaviour
{
    // Easing tipo "back" (rebote al final)
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
    public enum HandType { Hour, Minute, Second }
    public HandType handType = HandType.Hour;
    public bool invertRotation = false;
    public float animationDuration = 0.5f;

    private int lastValue = 0;
    private int max = 1;

    private float animStartAngle;
    private float animEndAngle;
    private float animTimer = 0f;
    private bool animating = false;

    public void SetTime(int hour, int minute, int second)
    {
        int value = 0;
        switch (handType)
        {
            case HandType.Hour:
                value = hour;
                max = 24;
                break;
            case HandType.Minute:
                value = minute;
                max = 60;
                break;
            case HandType.Second:
                value = second;
                max = 60;
                break;
        }

        // Calcular el ángulo objetivo normalizado
        float angleStep = 360f / max;
        float targetAngle = value * angleStep;
        if (invertRotation) targetAngle *= -1f;

        // Ángulo actual normalizado
        float currentAngle = transform.localEulerAngles.z;
        // Si la animación sería hacia atrás, sumamos 360 para forzar siempre hacia adelante
        float delta = Mathf.DeltaAngle(currentAngle, targetAngle);
        if (delta < 0) targetAngle += 360f;

        animStartAngle = currentAngle;
        animEndAngle = targetAngle;
        animTimer = 0f;
        animating = true;
        lastValue = value;

        if (animationDuration <= 0f)
        {
            transform.localEulerAngles = new Vector3(0, 0, targetAngle % 360f);
            animating = false;
        }
    }

    void Update()
    {
        if (animating)
        {
            animTimer += Time.deltaTime;
            float t = Mathf.Clamp01(animTimer / animationDuration);
            float easedT = EaseOutBack(t);
            float newZ = Mathf.Lerp(animStartAngle, animEndAngle, easedT);
            transform.localEulerAngles = new Vector3(0, 0, newZ % 360f);
            if (t >= 1f)
            {
                animating = false;
                // Normaliza la rotación final
                transform.localEulerAngles = new Vector3(0, 0, animEndAngle % 360f);
            }
        }
    }
}

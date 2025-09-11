using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Bars")]
    public RectTransform bar1;
    public RectTransform bar2;
    public RectTransform bar3;
    [Header("Settings")]
    public float maxBarHeight = 300f;

    void OnEnable()
    {
        GameEvents.OnStat1Changed += UpdateBar;
    }

    void OnDisable()
    {

    }

    private void UpdateBar(float current, float max)
    {
        float ratio = current / max;
        SetBarHeight(bar1, ratio);
    }

    private void SetBarHeight(RectTransform bar, float ratio)
    {
        var size = bar.sizeDelta;
        size.y = ratio * maxBarHeight;
        bar.sizeDelta = size;
    }

}

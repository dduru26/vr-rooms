using UnityEngine;
using UnityEngine.UI;

public class TurnSettingsToggles : MonoBehaviour
{
    public Toggle snapToggle;
    public Toggle smoothToggle;

    void Awake()
    {
        if (snapToggle) snapToggle.onValueChanged.AddListener(on => { if (on && TurnSettings.Smooth) TurnSettings.Smooth = false; });
        if (smoothToggle) smoothToggle.onValueChanged.AddListener(on => { if (on && !TurnSettings.Smooth) TurnSettings.Smooth = true; });
    }

    void OnEnable()
    {
        bool smooth = TurnSettings.Smooth;
        if (snapToggle) snapToggle.SetIsOnWithoutNotify(!smooth);
        if (smoothToggle) smoothToggle.SetIsOnWithoutNotify(smooth);
    }
}

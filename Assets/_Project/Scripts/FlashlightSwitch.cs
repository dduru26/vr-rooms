using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlashlightSwitch : MonoBehaviour
{
    public Light[] lights;
    public bool startOn;

    public bool IsOn { get; private set; }

    void Awake()
    {
        if (lights == null || lights.Length == 0) lights = GetComponentsInChildren<Light>(true);
        Set(startOn);
        GetComponent<XRGrabInteractable>().activated.AddListener(_ => Set(!IsOn));
    }

    void Set(bool on)
    {
        IsOn = on;
        foreach (var l in lights) if (l) l.enabled = on;
    }
}

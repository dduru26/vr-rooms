using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    [Tooltip("Analog grip value (0-1), e.g. XRI Left Interaction/Select Value.")]
    public InputActionProperty gripAction;

    [Tooltip("Analog trigger value (0-1), e.g. XRI Left Interaction/Activate Value.")]
    public InputActionProperty triggerAction;

    [Tooltip("How quickly the fingers follow the buttons (higher = snappier).")]
    public float smoothing = 18f;

    static readonly int GripId = Animator.StringToHash("Grip");
    static readonly int TriggerId = Animator.StringToHash("Trigger");

    Animator animator;
    float grip, trigger;

    void Awake() => animator = GetComponent<Animator>();

    void OnEnable()
    {
        if (gripAction.reference == null) gripAction.action?.Enable();
        if (triggerAction.reference == null) triggerAction.action?.Enable();
    }

    void Update()
    {
        float targetGrip = gripAction.action != null ? gripAction.action.ReadValue<float>() : 0f;
        float targetTrigger = triggerAction.action != null ? triggerAction.action.ReadValue<float>() : 0f;
        float k = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        grip = Mathf.Lerp(grip, targetGrip, k);
        trigger = Mathf.Lerp(trigger, targetTrigger, k);
        animator.SetFloat(GripId, grip);
        animator.SetFloat(TriggerId, trigger);
    }
}

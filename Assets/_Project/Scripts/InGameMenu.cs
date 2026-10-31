using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [Tooltip("The world-space canvas that is shown and hidden.")]
    public GameObject menuRoot;

    [Tooltip("Optional label that shows which room the player is in.")]
    public TMP_Text currentRoomLabel;

    [Tooltip("How far in front of the headset the menu appears, in metres.")]
    public float distance = 0.9f;

    [Tooltip("Vertical offset from eye height, in metres (negative = below the eyes).")]
    public float heightOffset = -0.1f;

    public static event System.Action Opened;
    public static event System.Action Closed;

    InputAction toggleAction;

    void Awake()
    {
        toggleAction = new InputAction("Toggle Menu", InputActionType.Button);
        toggleAction.AddBinding("<XRController>{LeftHand}/{MenuButton}");
        toggleAction.AddBinding("<XRController>{LeftHand}/{SecondaryButton}");
        toggleAction.AddBinding("<Keyboard>/m");
        toggleAction.performed += _ => Toggle();
        if (menuRoot) menuRoot.SetActive(false);
        if (currentRoomLabel) currentRoomLabel.text = "LOCATION: " + FriendlyName(SceneManager.GetActiveScene().name);
    }

    void OnEnable() => toggleAction?.Enable();

    void OnDisable() => toggleAction?.Disable();

    void OnDestroy() => toggleAction?.Dispose();

    public void Toggle()
    {
        if (menuRoot.activeSelf) Close(); else Open();
    }

    public void Open()
    {
        var cam = Camera.main;
        if (cam != null)
        {
            var forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude < 0.01f) forward = cam.transform.up;
            menuRoot.transform.position = cam.transform.position + forward * distance + Vector3.up * heightOffset;
            menuRoot.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
        menuRoot.SetActive(true);
        Opened?.Invoke();
    }

    public void Close()
    {
        bool wasOpen = menuRoot.activeSelf;
        menuRoot.SetActive(false);
        if (wasOpen) Closed?.Invoke();
    }

    static string FriendlyName(string scene) => scene switch
    {
        SceneNavigator.CourseScene => "Create with VR Room",
        SceneNavigator.VRRoomScene => "VR Room",
        _ => scene
    };
}

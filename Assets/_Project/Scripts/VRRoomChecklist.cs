using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class VRRoomChecklist : MonoBehaviour
{
    public static VRRoomChecklist Instance { get; private set; }

    public enum Task { Walk, Turn, Teleport, Menu, PickUp, Flashlight, MountFlashlight, Door, MainRoom, Clock, Chess, Mug }

    public static readonly string[] Labels =
    {
        "Walk with the left stick",
        "Turn with the right stick",
        "Teleport: hold X/A, aim, squeeze Grip",
        "Open the menu with Y",
        "Pick up any object (Grip)",
        "Turn on the flashlight (back-left crate)",
        "Hang the flashlight back on its mount",
        "Open the glass door by its handle",
        "Go through to the main room",
        "Check the time on the wall clock",
        "Put a chess piece on the board",
        "Return the mug to its coaster",
    };

    [Header("Panel")]
    [Tooltip("The world-space canvas, pinned to the head (pivot at its bottom-left corner).")] public RectTransform panel;
    [Tooltip("Everything below the header; hidden while minimized.")] public GameObject body;
    public TMP_Text title;
    public TMP_Text counter;
    public TMP_Text minimizeLabel;
    [Tooltip("Fill bar; its right anchor shows progress.")] public RectTransform progressFill;
    public TMP_Text[] rowLabels;
    public GameObject[] rowChecks;
    [Tooltip("Panel height in canvas pixels when expanded / minimized.")] public float fullHeight = 492f, minHeight = 74f;
    [Tooltip("Distance from the eyes in metres.")] public float distance = 0.55f;
    [Tooltip("Open panel: degrees below (negative) / above straight ahead.")] public float openPitch = -4f;
    [Tooltip("Minimized tab: the full panel's frame centre, degrees left (negative) / right of straight ahead.")] public float yaw = -20f;
    [Tooltip("Minimized tab: the full panel's frame centre, degrees below (negative) / above straight ahead.")] public float pitch = -16f;
    [Tooltip("Metres per canvas pixel for the minimized tab.")] public float pixelScale = 0.0005f;
    [Tooltip("Metres per canvas pixel while open in the centre (bigger, easier to read).")] public float openScale = 0.0007f;
    public Color doneColor = new Color(0.5f, 0.66f, 0.6f);
    public Color flashColor = new Color(0.71f, 1f, 0.23f);

    [Header("Rig")]
    public Transform head;
    public Transform origin;
    public ContinuousMoveProvider move;
    public SnapTurnProvider snapTurn;
    public ContinuousTurnProvider smoothTurn;
    public TeleportationProvider teleport;

    [Header("Room")]
    public XRGrabInteractable flashlight;
    public XRGrabInteractable mug;
    public XRGrabInteractable doorPanel;
    public XRSocketInteractor flashlightMount;
    public XRSocketInteractor mugCoaster;
    public XRSocketInteractor[] chessSquares;
    public HingeJoint doorHinge;
    public Transform clock;
    [Tooltip("The main room starts past this world Z.")] public float mainRoomZ = 14.5f;
    [Tooltip("Metres to walk with the stick for the walk task.")] public float walkDistance = 2f;

    readonly bool[] done = new bool[Labels.Length];
    bool visible = true, menuOpen, minimized, flashlightTaken, mugTaken;
    InputAction minimizeAction;
    float walked, clockLook;
    Vector3 lastOrigin;
    Color bodyColor;

    public int DoneCount { get { int n = 0; foreach (var d in done) if (d) n++; return n; } }
    public bool IsDone(Task t) => done[(int)t];

    public bool Visible
    {
        get => visible;
        set { visible = value; UpdateVisibility(); }
    }

    public bool Minimized
    {
        get => minimized;
        set { minimized = value; Layout(); }
    }

    public void ToggleMinimized() => Minimized = !minimized;

    void Awake()
    {
        Instance = this;
        minimizeAction = new InputAction("Minimize Checklist", InputActionType.Button);
        minimizeAction.AddBinding("<XRController>{RightHand}/{Primary2DAxisClick}");
        minimizeAction.AddBinding("<Keyboard>/t");
        minimizeAction.performed += _ => ToggleMinimized();
    }
    void OnEnable() => minimizeAction?.Enable();
    void OnDisable() => minimizeAction?.Disable();
    void OnDestroy() { if (Instance == this) Instance = null; InGameMenu.Opened -= OnMenuOpened; InGameMenu.Closed -= OnMenuClosed; minimizeAction?.Dispose(); }

    void Start()
    {
        if (!head && Camera.main) head = Camera.main.transform;
        bodyColor = rowLabels.Length > 0 ? rowLabels[0].color : Color.white;
        for (int i = 0; i < rowLabels.Length && i < Labels.Length; i++) rowLabels[i].text = Labels[i];

        if (snapTurn) snapTurn.locomotionEnded += _ => Complete(Task.Turn);
        if (smoothTurn) smoothTurn.locomotionEnded += _ => Complete(Task.Turn);
        if (teleport) teleport.locomotionEnded += _ => Complete(Task.Teleport);
        InGameMenu.Opened += OnMenuOpened;
        InGameMenu.Closed += OnMenuClosed;

        foreach (var g in FindObjectsByType<XRGrabInteractable>())
            if (g != doorPanel) g.selectEntered.AddListener(OnGrab);
        if (flashlight) flashlight.activated.AddListener(_ => Complete(Task.Flashlight));
        if (flashlightMount) flashlightMount.selectEntered.AddListener(a => { if (flashlightTaken && Is(a, flashlight)) Complete(Task.MountFlashlight); });
        if (mugCoaster) mugCoaster.selectEntered.AddListener(a => { if (mugTaken && Is(a, mug)) Complete(Task.Mug); });
        if (chessSquares != null)
            foreach (var s in chessSquares) if (s) s.selectEntered.AddListener(_ => { if (Time.timeSinceLevelLoad > 1f) Complete(Task.Chess); });

        if (origin) lastOrigin = origin.position;
        if (head && panel)
        {
            panel.SetParent(head, false);
            var canvas = panel.GetComponent<Canvas>(); if (canvas && head.TryGetComponent<Camera>(out var cam)) canvas.worldCamera = cam;
        }
        Layout();
        Refresh();
    }

    static bool Is(SelectEnterEventArgs a, XRGrabInteractable g) => g && a.interactableObject != null && a.interactableObject.transform == g.transform;

    void OnGrab(SelectEnterEventArgs a)
    {
        if (a.interactorObject is XRSocketInteractor) return;
        Complete(Task.PickUp);
        if (Is(a, flashlight)) flashlightTaken = true;
        if (Is(a, mug)) mugTaken = true;
    }

    void OnMenuOpened() { menuOpen = true; Complete(Task.Menu); UpdateVisibility(); }
    void OnMenuClosed() { menuOpen = false; UpdateVisibility(); }

    void Update()
    {
        if (!head) return;
        if (origin)
        {
            var step = Vector3.ProjectOnPlane(origin.position - lastOrigin, Vector3.up).magnitude;
            lastOrigin = origin.position;
            bool stick = move && move.isActiveAndEnabled && move.leftHandMoveInput.ReadValue().sqrMagnitude > 0.04f;
            if (stick && step < 0.5f) { walked += step; if (walked >= walkDistance) Complete(Task.Walk); }
        }
        if (doorHinge && Mathf.Abs(doorHinge.angle) > 40f) Complete(Task.Door);
        if (head.position.z > mainRoomZ) Complete(Task.MainRoom);
        if (clock)
        {
            var to = clock.position - head.position;
            if (to.magnitude < 7f && Vector3.Dot(head.forward, to.normalized) > 0.94f) { clockLook += Time.deltaTime; if (clockLook > 0.6f) Complete(Task.Clock); }
        }
    }

    void Layout()
    {
        if (!panel) return;
        if (body) body.SetActive(!minimized);
        if (minimizeLabel) minimizeLabel.text = minimized ? "+" : "\u2013";
        panel.localScale = Vector3.one * (minimized ? pixelScale : openScale);
        float w = panel.sizeDelta.x * pixelScale;
        if (!minimized)
        {
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, fullHeight);
            var rot = Quaternion.Euler(-openPitch, 0f, 0f);
            panel.localRotation = rot;
            panel.localPosition = rot * Vector3.forward * distance;
        }
        else
        {
            panel.pivot = Vector2.zero;
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, minHeight);
            var rot = Quaternion.Euler(-pitch, yaw, 0f);
            float h = fullHeight * pixelScale;
            panel.localRotation = rot;
            panel.localPosition = rot * Vector3.forward * distance - rot * Vector3.right * (w * 0.5f) - rot * Vector3.up * (h * 0.5f);
        }
    }

    void UpdateVisibility() { if (panel) panel.gameObject.SetActive(visible && !menuOpen); }

    public void Complete(Task t)
    {
        int i = (int)t;
        if (done[i]) return;
        done[i] = true;
        Refresh();
        if (i < rowLabels.Length && rowLabels[i] && isActiveAndEnabled) StartCoroutine(Flash(rowLabels[i]));
        foreach (var h in FindObjectsByType<HapticImpulsePlayer>()) h.SendHapticImpulse(DoneCount == done.Length ? 0.7f : 0.35f, DoneCount == done.Length ? 0.4f : 0.08f);
    }

    void Refresh()
    {
        int n = DoneCount, total = done.Length;
        if (counter) counter.text = $"{n} / {total}";
        if (title) title.text = n == total ? "Room complete" : "Tasks";
        if (progressFill) progressFill.anchorMax = new Vector2((float)n / total, 1f);
        for (int i = 0; i < rowLabels.Length && i < done.Length; i++)
        {
            if (rowChecks != null && i < rowChecks.Length && rowChecks[i]) rowChecks[i].SetActive(done[i]);
            if (!rowLabels[i]) continue;
            rowLabels[i].color = done[i] ? doneColor : bodyColor;
            rowLabels[i].fontStyle = done[i] ? (rowLabels[i].fontStyle | FontStyles.Strikethrough) : (rowLabels[i].fontStyle & ~FontStyles.Strikethrough);
        }
    }

    IEnumerator Flash(TMP_Text label)
    {
        for (float t = 0f; t < 1.2f; t += Time.deltaTime)
        {
            label.color = Color.Lerp(flashColor, doneColor, t / 1.2f);
            yield return null;
        }
        label.color = doneColor;
    }
}

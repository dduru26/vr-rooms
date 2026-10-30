using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrainingProgress : MonoBehaviour
{
    [Tooltip("Text that receives the progress readout")]
    public TextMeshProUGUI output = null;

    [Tooltip("Collider of the platform boxes are stacked on")]
    public Collider platform = null;

    [Tooltip("Magnet mover whose speed is displayed")]
    public MoveWithVelocity magnet = null;

    [Tooltip("Start the timer automatically instead of waiting for Begin()")]
    public bool startOnAwake = false;

    [Tooltip("Seconds between readout refreshes")]
    public float refreshInterval = 0.25f;

    private bool running = false;
    private float startTime = 0f;
    private float nextRefresh = 0f;
    private int best = 0;

    private void Start()
    {
        if (startOnAwake) Begin();
        Refresh();
    }

    public void Begin()
    {
        running = true;
        startTime = Time.time;
        best = 0;
    }

    private void Update()
    {
        if (Time.time < nextRefresh) return;
        nextRefresh = Time.time + refreshInterval;
        Refresh();
    }

    public int CountBoxesOnPlatform()
    {
        if (platform == null) return 0;
        Bounds area = platform.bounds;
        int count = 0;
        foreach (var grab in FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Exclude))
        {
            if (!grab.name.StartsWith("Box") || grab.isSelected) continue;
            Vector3 p = grab.transform.position;
            bool insideXZ = p.x > area.min.x && p.x < area.max.x && p.z > area.min.z && p.z < area.max.z;
            if (insideXZ && p.y > area.max.y - 0.05f) count++;
        }
        return count;
    }

    private void Refresh()
    {
        if (output == null) return;
        int boxes = CountBoxesOnPlatform();
        if (boxes > best) best = boxes;
        float elapsed = running ? Time.time - startTime : 0f;
        int minutes = (int)(elapsed / 60f);
        int seconds = (int)(elapsed % 60f);
        string speed = magnet != null ? magnet.speed.ToString("0.0") : "-";
        output.text = $"Boxes on platform: {boxes}  (best {best})\nTime: {minutes:00}:{seconds:00}\nMagnet speed: {speed}";
    }
}

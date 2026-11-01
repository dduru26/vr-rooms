using TMPro;
using UnityEngine;

public class MenuOptions : MonoBehaviour
{
    public TMP_Text turnLabel;
    public GameObject tasksButton;

    public TMP_Text tasksLabel;

    void OnEnable() { TurnSettings.Changed += Refresh; Refresh(); }
    void OnDisable() => TurnSettings.Changed -= Refresh;

    public void ToggleTurn() => TurnSettings.Smooth = !TurnSettings.Smooth;

    public void ToggleTasks()
    {
        var list = VRRoomChecklist.Instance;
        if (list) list.Visible = !list.Visible;
        Refresh();
    }

    void Refresh()
    {
        if (turnLabel) turnLabel.text = "Turning: " + (TurnSettings.Smooth ? "Smooth" : "Snap");
        var list = VRRoomChecklist.Instance;
        if (tasksButton) tasksButton.SetActive(list != null);
        if (tasksLabel && list) tasksLabel.text = "Tasks: " + (list.Visible ? "Shown" : "Hidden");
    }
}

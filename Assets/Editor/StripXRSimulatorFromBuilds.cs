using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StripXRSimulatorFromBuilds : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report == null) return;
        int removed = 0;
        foreach (var root in scene.GetRootGameObjects())
            foreach (var c in root.GetComponentsInChildren<Component>(true))
            {
                if (c == null) continue;
                var n = c.GetType().Name;
                if (n == "XRInteractionSimulator" || n == "XRDeviceSimulator") { Object.DestroyImmediate(c.gameObject); removed++; }
            }
        if (removed > 0) Debug.Log($"[Build] Removed {removed} XR simulator object(s) from {scene.name}");
    }
}

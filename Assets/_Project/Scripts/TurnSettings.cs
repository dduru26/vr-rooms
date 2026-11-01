using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public static class TurnSettings
{
    const string Key = "vrrooms.turn.smooth";

    public static event System.Action Changed;

         public static bool Smooth
    {
        get => PlayerPrefs.GetInt(Key, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(Key, value ? 1 : 0);
            PlayerPrefs.Save();
            Apply();
            Changed?.Invoke();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() => SceneManager.sceneLoaded += (_, __) => Apply();

    public static void Apply()
    {
        bool smooth = Smooth;
#pragma warning disable CS0618
        foreach (var p in Object.FindObjectsByType<SnapTurnProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None)) p.gameObject.SetActive(!smooth);
        foreach (var p in Object.FindObjectsByType<ContinuousTurnProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None)) p.gameObject.SetActive(smooth);
#pragma warning restore CS0618
    }
}

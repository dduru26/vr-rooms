using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    public const string StartScene = "Start Menu";

    public const string CourseScene = "Donald's Room";
    public const string VRRoomScene = "VR Room";

    [Tooltip("Seconds the screen takes to fade to or from black.")]
    public float fadeDuration = 0.4f;

    CanvasGroup fade;
    bool busy;

        void Start()
    {
        CreateFadeOverlay();
        StartCoroutine(FadeTo(0f));
    }

    public void LoadStart() => Load(StartScene);

    public void LoadCourseRoom() => Load(CourseScene);

    public void LoadVRRoom() => Load(VRRoomScene);

    public void ReloadCurrent() => Load(SceneManager.GetActiveScene().name);

    public void Quit() => Application.Quit();

    public void Load(string sceneName)
    {
        if (busy) return;
        StartCoroutine(LoadRoutine(sceneName));
    }

        IEnumerator LoadRoutine(string sceneName)
    {
        busy = true;
        yield return FadeTo(1f);
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!op.isDone) yield return null;
    }

    IEnumerator FadeTo(float target)
    {
        if (fade == null) yield break;
        float start = fade.alpha, t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fade.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        fade.alpha = target;
    }

        void CreateFadeOverlay()
    {
        var cam = Camera.main;
        if (cam == null) return;
        var go = new GameObject("Fade Overlay");
        go.transform.SetParent(transform, false);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = cam.nearClipPlane + 0.02f;
        canvas.sortingOrder = 1000;
        fade = go.AddComponent<CanvasGroup>();
        fade.alpha = 1f;
        fade.blocksRaycasts = false;
        fade.interactable = false;
        var img = new GameObject("Black").AddComponent<Image>();
        img.transform.SetParent(go.transform, false);
        img.color = Color.black;
        img.raycastTarget = false;
        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PaintTurntable : MonoBehaviour
{
    [Tooltip("Where miniature prints are placed")]
    public Transform printSpot = null;

    [Tooltip("Size of a print relative to the original")]
    public float printScale = 0.25f;

    [Tooltip("Only strokes finished within this distance of the turntable are collected")]
    public float collectRadius = 2f;

    [Tooltip("Seconds between checks for newly finished strokes")]
    public float collectInterval = 0.2f;

    private GameObject lastPrint = null;
    private float nextCollect = 0f;

    private void Update()
    {
        if (Time.time < nextCollect) return;
        nextCollect = Time.time + collectInterval;
        CollectFinishedStrokes();
    }

    public void CollectFinishedStrokes()
    {
        foreach (var trail in FindObjectsByType<TrailRenderer>(FindObjectsInactive.Exclude))
        {
            if (trail.transform.parent != null) continue;
            if (Vector3.Distance(trail.transform.position, transform.position) > collectRadius) continue;
            if (trail.positionCount < 2) { Discard(trail.gameObject); continue; }

            var points = new Vector3[trail.positionCount];
            trail.GetPositions(points);
            var stroke = new GameObject("Stroke");
            stroke.tag = trail.gameObject.tag;
            stroke.transform.SetParent(transform, false);
            var line = stroke.AddComponent<LineRenderer>();
            CopyStyle(trail, line, 1f);
            line.useWorldSpace = false;
            line.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++) line.SetPosition(i, transform.InverseTransformPoint(points[i]));
            Discard(trail.gameObject);
        }
    }

    private static void Discard(GameObject go)
    {
        go.SetActive(false);
        Destroy(go);
    }

    public void SetRotation(float normalized)
    {
        transform.localRotation = Quaternion.Euler(0f, Mathf.Clamp01(normalized) * 360f, 0f);
    }

    public void Print()
    {
        if (printSpot == null) return;
        CollectFinishedStrokes();
        if (lastPrint) Destroy(lastPrint);

        lastPrint = new GameObject("Print");
        lastPrint.transform.SetParent(printSpot, false);
        lastPrint.transform.localScale = Vector3.one * printScale;

        foreach (var mr in GetComponentsInChildren<MeshRenderer>())
        {
            var mf = mr.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;
            var copy = new GameObject(mr.name);
            copy.transform.SetParent(lastPrint.transform, false);
            copy.transform.localPosition = transform.InverseTransformPoint(mr.transform.position);
            copy.transform.localRotation = Quaternion.Inverse(transform.rotation) * mr.transform.rotation;
            copy.transform.localScale = mr.transform.lossyScale;
            copy.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
            copy.AddComponent<MeshRenderer>().sharedMaterials = mr.sharedMaterials;
        }

        foreach (var line in GetComponentsInChildren<LineRenderer>())
        {
            var copy = new GameObject("Stroke");
            copy.transform.SetParent(lastPrint.transform, false);
            var mini = copy.AddComponent<LineRenderer>();
            mini.sharedMaterials = line.sharedMaterials;
            mini.colorGradient = line.colorGradient;
            mini.widthCurve = line.widthCurve;
            mini.widthMultiplier = line.widthMultiplier * printScale;
            mini.numCapVertices = line.numCapVertices;
            mini.numCornerVertices = line.numCornerVertices;
            mini.useWorldSpace = false;
            var pts = new List<Vector3>();
            for (int i = 0; i < line.positionCount; i++) pts.Add(transform.InverseTransformPoint(line.transform.TransformPoint(line.GetPosition(i))));
            mini.positionCount = pts.Count;
            mini.SetPositions(pts.ToArray());
        }
    }

    private static void CopyStyle(TrailRenderer from, LineRenderer to, float scale)
    {
        to.sharedMaterials = from.sharedMaterials;
        to.colorGradient = from.colorGradient;
        to.widthCurve = from.widthCurve;
        to.widthMultiplier = from.widthMultiplier * scale;
        to.numCapVertices = from.numCapVertices;
        to.numCornerVertices = from.numCornerVertices;
        to.alignment = from.alignment;
        to.textureMode = from.textureMode;
        to.shadowCastingMode = from.shadowCastingMode;
    }
}

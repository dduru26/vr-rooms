using System;
using TMPro;
using UnityEngine;

public class WallClock : MonoBehaviour
{
    [Tooltip("Hour hand transform (pivot on the clock centre, pointing at 12 when unrotated).")]
    public Transform hourHand;

    [Tooltip("Minute hand transform.")]
    public Transform minuteHand;

    [Tooltip("Second hand transform.")]
    public Transform secondHand;

    [Tooltip("Local axis the hands spin around (the clock face's normal).")]
    public Vector3 rotationAxis = Vector3.right;

    [Tooltip("When on, the second hand sweeps smoothly; when off it ticks once per second.")]
    public bool smoothSeconds = false;

    [Tooltip("Optional digital display for the time (HH:mm:ss).")]
    public TMP_Text digitalTime;

    [Tooltip("Optional digital display for the date.")]
    public TMP_Text digitalDate;

    Quaternion hour0, minute0, second0;

    void Awake()
    {
        if (hourHand) hour0 = hourHand.localRotation;
        if (minuteHand) minute0 = minuteHand.localRotation;
        if (secondHand) second0 = secondHand.localRotation;
    }

    void Update()
    {
        DateTime now = DateTime.Now;
        float seconds = now.Second + (smoothSeconds ? now.Millisecond / 1000f : 0f);
        float minutes = now.Minute + seconds / 60f;
        float hours = (now.Hour % 12) + minutes / 60f;

        if (secondHand) secondHand.localRotation = second0 * Quaternion.AngleAxis(seconds * 6f, rotationAxis);
        if (minuteHand) minuteHand.localRotation = minute0 * Quaternion.AngleAxis(minutes * 6f, rotationAxis);
        if (hourHand) hourHand.localRotation = hour0 * Quaternion.AngleAxis(hours * 30f, rotationAxis);

        if (digitalTime) digitalTime.text = now.ToString("HH:mm:ss");
        if (digitalDate) digitalDate.text = now.ToString("dddd d MMMM yyyy");
    }
}

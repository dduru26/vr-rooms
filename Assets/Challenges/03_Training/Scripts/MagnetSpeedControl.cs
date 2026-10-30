using UnityEngine;

public class MagnetSpeedControl : MonoBehaviour
{
    [Tooltip("The magnet mover to adjust")]
    public MoveWithVelocity magnet = null;

    [Tooltip("How much each press changes the speed")]
    public float step = 1f;

    [Tooltip("Slowest allowed speed")]
    public float minSpeed = 1f;

    [Tooltip("Fastest allowed speed")]
    public float maxSpeed = 10f;

    public void Increase() => SetSpeed(magnet.speed + step);

    public void Decrease() => SetSpeed(magnet.speed - step);

    public void SetSpeed(float value)
    {
        if (magnet != null) magnet.speed = Mathf.Clamp(value, minSpeed, maxSpeed);
    }
}

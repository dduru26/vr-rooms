using UnityEngine;

public class IgnoreCollisionsWith : MonoBehaviour
{
    [Tooltip("Colliders this object should pass through (for example the door frame).")]
    public Collider[] others;

    void Awake()
    {
        var mine = GetComponentsInChildren<Collider>();
        foreach (var a in mine)
            foreach (var b in others)
                if (a != null && b != null) Physics.IgnoreCollision(a, b, true);
    }
}

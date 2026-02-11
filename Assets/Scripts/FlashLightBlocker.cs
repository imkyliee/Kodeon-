using UnityEngine;

public class FlashLightBlocker : MonoBehaviour
{
    [Tooltip("The actual Light component (usually the Spot light on the flashlight)")]
    public Light flashlight;

    public float maxRange = 20f;      // Normal flashlight range
    public float smoothSpeed = 15f;   // How fast it adjusts to walls

    [Tooltip("Layers that block the light. Set in inspector to ignore player layer")]
    public LayerMask occlusionMask = ~0;

    [Tooltip("Small radius for the SphereCast to detect obstacles across the cone")]
    public float sphereRadius = 0.06f;

    [Tooltip("How far back from the hit point to stop the light to avoid visible clipping")]
    public float stopEpsilon = 0.02f;

    void Update()
    {
        if (flashlight == null)
            return;

        if (!flashlight.enabled)
            return; // Don't block light when flashlight is off

        RaycastHit hit;
        float targetRange = maxRange;

        // Use the light's transform as origin (handles offset on the prefab)
        Vector3 origin = flashlight.transform.position;
        Vector3 dir = flashlight.transform.forward;

        // Use a small SphereCast to better detect walls across the cone. Ignore triggers.
        if (Physics.SphereCast(origin, sphereRadius, dir, out hit, maxRange, occlusionMask, QueryTriggerInteraction.Ignore))
        {
            // Subtract a tiny epsilon so the light stops just before the wall and doesn't visually clip
            targetRange = Mathf.Max(0.05f, hit.distance - stopEpsilon);
        }

        // Move towards the target. Move faster when shrinking so light doesn't leak through walls.
        float speed = (flashlight.range > targetRange) ? smoothSpeed * 8f : smoothSpeed;
        flashlight.range = Mathf.MoveTowards(flashlight.range, targetRange, Time.deltaTime * speed);
    }

    // Debug helper: draw the spherecast in the scene view
    void OnDrawGizmosSelected()
    {
        if (flashlight == null)
            return;

        Gizmos.color = Color.yellow;
        Vector3 origin = flashlight.transform.position;
        Gizmos.DrawWireSphere(origin + flashlight.transform.forward * 0.1f, sphereRadius);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float playerRadius = 0.4f;
    public LayerMask obstacleLayer;     // Walls/crates

    [Header("Freeze Pulse (press E)")]
    public float pulseRadius = 6f;
    public float pulseFreezeTime = 3f;
    public float pulseCooldown = 8f;

    private float nextPulseTime;

    public float PulseCooldownLeft => Mathf.Max(0f, nextPulseTime - Time.time);

    private void Update()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Movement (WASD / arrow keys)
        float x = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1 : 0) -
                  (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1 : 0);
        float z = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1 : 0) -
                  (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1 : 0);

        Vector3 dir = new Vector3(x, 0, z);
        if (dir.sqrMagnitude > 0.01f)
        {
            dir.Normalize();
            Vector3 step = dir * moveSpeed * Time.deltaTime;

            // Try full move, then slide along each axis so walls don't feel sticky
            if (CanMove(step)) transform.position += step;
            else if (CanMove(new Vector3(step.x, 0, 0))) transform.position += new Vector3(step.x, 0, 0);
            else if (CanMove(new Vector3(0, 0, step.z))) transform.position += new Vector3(0, 0, step.z);
        }

        // Freeze Pulse
        if (kb.eKey.wasPressedThisFrame && Time.time >= nextPulseTime)
        {
            FreezePulse();
        }
    }

    private bool CanMove(Vector3 step)
    {
        if (step.sqrMagnitude < 0.000001f) return false;
        return !Physics.SphereCast(transform.position, playerRadius, step.normalized,
                                   out _, step.magnitude + 0.02f, obstacleLayer);
    }

    private void FreezePulse()
    {
        nextPulseTime = Time.time + pulseCooldown;

        foreach (var guard in GameManager.Instance.guards)
        {
            if (guard == null) continue;
            Vector3 d = guard.transform.position - transform.position;
            d.y = 0;
            if (d.magnitude <= pulseRadius) guard.Freeze(pulseFreezeTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pulseRadius);
    }
}
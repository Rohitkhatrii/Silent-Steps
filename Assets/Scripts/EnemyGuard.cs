using UnityEngine;

public class EnemyGuard : MonoBehaviour
{
    [Header("Detection")]
    public float detectRadius = 5f;
    public float hitRadius = 1.5f;
    public float stopDistance = 0.8f;
    public LayerMask obstacleLayer;

    [Header("Combat")]
    public float moveSpeed = 5f;
    public int attackDamage = 10;
    public float hitInterval = 0.5f;
    public float ignoreTime = 4f;
    public float armDelay = 0.4f;   // Seconds you must stand still before moving counts as escaping

    private enum State { Idle, Chasing, Ignoring, Frozen }
    private State state = State.Idle;

    private float ignoreEndTime;
    private float frozenEndTime;
    private float nextHitTime;
    private float stillTimer;
    private bool armed;
    private Vector3 lastPlayerPos;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        SetState(State.Idle);
    }

    // Called by the player's Freeze Pulse
    public void Freeze(float seconds)
    {
        frozenEndTime = Time.time + seconds;
        SetState(State.Frozen);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.player == null) return;
        if (gm.currentState != GameManager.GameState.Playing) return;

        Vector3 playerPos = gm.player.transform.position;
        bool playerMoved = (playerPos - lastPlayerPos).sqrMagnitude > 0.0001f;
        lastPlayerPos = playerPos;

        switch (state)
        {
            case State.Idle:
                if (CanSeePlayer(playerPos, detectRadius))
                {
                    armed = false;
                    stillTimer = 0f;
                    nextHitTime = Time.time;
                    SetState(State.Chasing);
                }
                break;

            case State.Chasing:
                // Lost sight (hid behind a wall or ran far away)
                if (!CanSeePlayer(playerPos, detectRadius * 1.5f))
                {
                    SetState(State.Idle);
                    break;
                }

                // Player moved after standing still long enough -> ignore for 4 seconds
                if (playerMoved)
                {
                    if (armed)
                    {
                        ignoreEndTime = Time.time + ignoreTime;
                        SetState(State.Ignoring);
                        break;
                    }
                    stillTimer = 0f;
                }
                else
                {
                    stillTimer += Time.deltaTime;
                    if (stillTimer >= armDelay) armed = true;
                }

                // Keep sliding in until very close to the player
                if (FlatDistance(playerPos) > stopDistance)
                {
                    Vector3 target = new Vector3(playerPos.x, transform.position.y, playerPos.z);
                    transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                }

                // Hit as soon as in range
                if (FlatDistance(playerPos) <= hitRadius && Time.time >= nextHitTime)
                {
                    gm.TakeDamage(attackDamage);
                    nextHitTime = Time.time + hitInterval;
                }
                break;

            case State.Ignoring:
                if (Time.time >= ignoreEndTime) SetState(State.Idle);
                break;

            case State.Frozen:
                if (Time.time >= frozenEndTime) SetState(State.Idle);
                break;
        }
    }

    private float FlatDistance(Vector3 p)
    {
        Vector3 d = p - transform.position;
        d.y = 0;
        return d.magnitude;
    }

    private bool CanSeePlayer(Vector3 playerPos, float radius)
    {
        Vector3 to = playerPos - transform.position;
        to.y = 0;
        float dist = to.magnitude;
        if (dist > radius) return false;
        if (dist < 0.01f) return true;
        return !Physics.Raycast(transform.position, to.normalized, dist, obstacleLayer);
    }

    private void SetState(State s)
    {
        state = s;
        if (rend == null) return;
        switch (s)
        {
            case State.Chasing:  rend.material.color = Color.red; break;
            case State.Ignoring: rend.material.color = Color.gray; break;
            case State.Frozen:   rend.material.color = Color.cyan; break;
            default:             rend.material.color = Color.white; break;
        }
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState { Playing, GameOver, Victory }
    public GameState currentState = GameState.Playing;

    [Header("UI References")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI endScreenText;
    public Slider healthBar;
    public TextMeshProUGUI healthText;   // optional
    public TextMeshProUGUI pulseText;    // optional: shows pulse cooldown

    [Header("Player Tracking")]
    public PlayerMovement player;
    public EnemyGuard[] guards;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Audio")]
    public AudioClip hitSound;
    [Range(0f, 1f)] public float hitVolume = 1f;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
    }

    private void Start()
    {
        guards = FindObjectsOfType<EnemyGuard>();
        currentHealth = maxHealth;
        currentState = GameState.Playing;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (pulseText == null || player == null) return;

        // Hide the pulse/flashbang text once the game has ended
        if (currentState != GameState.Playing)
        {
            if (pulseText.gameObject.activeSelf) pulseText.gameObject.SetActive(false);
            return;
        }

        float cd = player.PulseCooldownLeft;
        pulseText.text = cd <= 0f ? "Flashbang: Ready (E)" : $"Use Again After: {cd:0.0}s";
    }

    public void TakeDamage(int amount)
    {
        if (currentState != GameState.Playing) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHealthUI();

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound, hitVolume);

        if (currentHealth <= 0) TriggerGameOver();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        if (healthText != null)
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
    }

    public void TriggerGameOver()
    {
        currentState = GameState.GameOver;
        endScreenPanel.SetActive(true);
        endScreenText.text = "DETECTED!\nMission Failed.";
    }

    public void TriggerVictory()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.Victory;
        endScreenPanel.SetActive(true);
        endScreenText.text = "TERMINAL Reached!\nMission Success.";
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
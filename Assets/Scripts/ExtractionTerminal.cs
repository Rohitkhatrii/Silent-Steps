using UnityEngine;

public class ExtractionTerminal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.TriggerVictory();
        }
    }
}
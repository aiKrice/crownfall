using UnityEngine;

public class TestConsole : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🧪 TEST CONSOLE - Si tu vois ce message, la console fonctionne !");
        Debug.LogWarning("⚠️ TEST WARNING");
        Debug.LogError("❌ TEST ERROR (c'est normal, c'est un test)");
    }
}
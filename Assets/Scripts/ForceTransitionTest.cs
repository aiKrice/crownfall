using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ForceTransitionTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🚀 FORCE TRANSITION TEST - Starting...");
        StartCoroutine(TestTransition());
    }
    
    IEnumerator TestTransition()
    {
        Debug.Log("⏰ Waiting 3 seconds...");
        yield return new WaitForSeconds(3f);
        
        Debug.Log("🔄 Loading MainMenuScene directly...");
        try
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to load scene: {e.Message}");
        }
    }
}
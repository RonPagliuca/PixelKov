using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        // Check if the ESC key was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    private void QuitGame()
    {
        // Log a message to the console (useful for debugging)
        Debug.Log("Quit game!");

        // Quit the application
        // This will not stop the editor, so it's wrapped in UNITY_EDITOR
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

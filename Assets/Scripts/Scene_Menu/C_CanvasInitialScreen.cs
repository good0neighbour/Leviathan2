using UnityEngine;
using UnityEngine.SceneManagement;

public class C_CanvasInitialScreen : MonoBehaviour
{
    public void ButtonStart()
    {
        SceneManager.LoadScene("Scene_Play");
    }
}

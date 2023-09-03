using UnityEngine;
using UnityEngine.SceneManagement;

public class C_CanvasMenu : MonoBehaviour
{
    public void ButtonMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scene_Menu");
    }


    public void ButtonBack()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }


    public void ButtonQuit()
    {
        Application.Quit();
    }


    private void Update()
    {
        // 단축키 동작, 윈도우에서 ESC, 안드로이드에서 뒤로가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonBack();
        }
    }
}

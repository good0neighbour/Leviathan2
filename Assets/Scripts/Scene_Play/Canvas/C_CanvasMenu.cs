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
        // ����Ű ����, �����쿡�� ESC, �ȵ���̵忡�� �ڷΰ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonBack();
        }
    }
}

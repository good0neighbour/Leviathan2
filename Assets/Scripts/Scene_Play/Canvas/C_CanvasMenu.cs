using UnityEngine;
using UnityEngine.SceneManagement;

public class C_CanvasMenu : MonoBehaviour
{
    public void ButtonMainMenu()
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);
        Time.timeScale = 1;
        SceneManager.LoadScene("Scene_Menu");
    }


    public void ButtonBack()
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }


    public void ButtonQuit()
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);
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

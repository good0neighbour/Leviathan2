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
        // ����Ű ����, �����쿡�� ESC, �ȵ���̵忡�� �ڷΰ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonBack();
        }
    }
}

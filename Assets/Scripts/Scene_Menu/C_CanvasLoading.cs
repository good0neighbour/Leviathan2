using UnityEngine;
using UnityEngine.SceneManagement;

public class C_CanvasLoading : MonoBehaviour
{
    private bool m_load = true;

    // ������ ���Ŀ� ȣ��
    private void LateUpdate()
    {
        if (m_load)
        {
            m_load = false;
            SceneManager.LoadScene("Scene_Play");
        }
    }
}

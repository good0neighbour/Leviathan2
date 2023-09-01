using UnityEngine;
using UnityEngine.SceneManagement;

public class C_CanvasLoading : MonoBehaviour
{
    private bool m_load = true;

    // 렌더링 이후에 호출
    private void LateUpdate()
    {
        if (m_load)
        {
            m_load = false;
            SceneManager.LoadScene("Scene_Play");
        }
    }
}

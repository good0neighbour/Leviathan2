using UnityEngine;

public class C_CanvasTutorial : MonoBehaviour
{
    public void ButtonAnywhere()
    {
        Destroy(gameObject);
    }


    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Destroy(gameObject);
        }
    }
}

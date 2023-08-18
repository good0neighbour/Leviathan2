using UnityEngine;

public class C_Actor : MonoBehaviour, I_State<E_PlayState>
{
    /* ========== Fields ========== */

    private Animator mp_animator = null;
    private Transform mp_cameraTransform = null;
    private Vector3 m_accelerator = Vector3.zero;
    private Vector3 m_velocity = Vector3.zero;



    /* ========== Public Methods ========== */

    public void ChangeState(E_PlayState t_state)
    {
        
    }


    public void Execute()
    {
        
    }


    public void StateFixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += Quaternion.Euler(0.0f, mp_cameraTransform.localRotation.eulerAngles.y, 0.0f) * new Vector3(-1.0f * Time.deltaTime, 0.0f, 0.0f);
            transform.localRotation = Quaternion.Euler(0.0f, mp_cameraTransform.localRotation.eulerAngles.y, 0.0f) * Quaternion.Euler(0.0f, -90.0f, 0.0f);
            mp_animator.SetFloat("MovingSpeed", 1.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += Quaternion.Euler(0.0f, mp_cameraTransform.localRotation.eulerAngles.y, 0.0f) * new Vector3(1.0f * Time.deltaTime, 0.0f, 0.0f);
            transform.localRotation = Quaternion.Euler(0.0f, mp_cameraTransform.localRotation.eulerAngles.y, 0.0f) * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            mp_animator.SetFloat("MovingSpeed", 1.0f);
        }
        else
        {

            mp_animator.SetFloat("MovingSpeed", 0.0f);
        }
    }


    public void StateUpdate()
    {
        
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        mp_animator = GetComponent<Animator>();
    }


    private void Start()
    {
        mp_cameraTransform = C_CameraMove.instance.transform;
    }
}

using UnityEngine;

public class C_MinimapCameraMove : MonoBehaviour
{
    /* ========== Fields ========== */

    private Transform mp_mainCameraTransform = null;
    private float m_yPosition = 500.0f;

    public static C_MinimapCameraMove instance
    {
        get;
        private set;
    }

    public Transform target
    {
        get;
        set;
    }



    /* ========== Private Methods ========== */

    private void LateFixedUpdate()
    {
        transform.localPosition = new Vector3(target.localPosition.x, m_yPosition, target.localPosition.z);
        transform.localRotation = Quaternion.Euler(90.0f, mp_mainCameraTransform.localEulerAngles.y, 0.0f);
    }


    private void Awake()
    {
        //����Ƽ�� �̱�������
        instance = this;

        // ī�޶� ���� ����
        m_yPosition = transform.localPosition.y;
    }


    private void Start()
    {
        // ����
        mp_mainCameraTransform = C_CameraMove.instance.transform;

        // �븮�� ���
        C_PlayManager.instance.lateFixedUpdate += LateFixedUpdate;
    }
}

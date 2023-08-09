using UnityEngine;
using TMPro;

public abstract class C_AirPlaneStateBase : C_StateBase
{
    /* ========== Fields ========== */

    protected C_AirPlane m_machine = null;
    protected Transform m_transform = null;
    protected Vector3 m_airResist = Vector3.zero;
    protected float m_liftPower = 0.0f;
    protected float m_HUDUpDownMoveAmount = 0.0f;
    private RectTransform m_HUDUpDown = null;
    private RectTransform m_directionImage = null;
    private TMP_Text m_velocityText = null;



    /* ========== Properties ========== */

    public float power
    {
        protected get; set;
    }


    public Vector3 velocity
    {
        get; set;
    }



    /* ========== Public Methods ========== */

    public C_AirPlaneStateBase(C_AirPlane t_machine, C_AirplaneSettings t_settings)
    {
        m_machine = t_machine;
        m_transform = t_machine.transform;
        t_machine.GetHUDs(out m_HUDUpDown, out m_velocityText, out m_directionImage);

        m_airResist = t_settings.m_airResist;
        m_liftPower = t_settings.m_liftPower;
        m_HUDUpDownMoveAmount = t_settings.m_HUDUpDownMoveAmount;
    }



    /* ========== Protected Methods ========== */

    /// <summary>
    /// �ӷ� ����
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration)
    {
        // ���
        //t_acceleration += new Vector3(0.0f, -velocity.y * Mathf.Abs(velocity.z) * m_liftPower, 0.0f);

        // ��Į ��ǥ�� ���� �ӷ�
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * velocity
        
            // ����
            + t_acceleration * Time.fixedDeltaTime;

        // ��ü ����
        velocity = m_transform.localRotation

            // ��������
            * new Vector3(
                (float)(t_velocity.x - m_airResist.x * Mathf.Abs(t_velocity.x) * t_velocity.x),
                (float)(t_velocity.y - m_airResist.y * Mathf.Abs(t_velocity.y) * t_velocity.y),
                (float)(t_velocity.z - m_airResist.z * Mathf.Abs(t_velocity.z) * t_velocity.z)
            )

            // �߷°��ӵ�
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;

        // ��ȯ
        return velocity;
    }


    /// <summary>
    /// �ӷ� ���� �� ��Į ��ǥ�� ���� �ӷ�
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration, out float t_localVelocityZ)
    {
        // ���
        //t_acceleration += new Vector3(0.0f, -velocity.y * Mathf.Abs(velocity.z) * m_liftPower, 0.0f);

        // ��Į ��ǥ�� ���� �ӷ�
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * velocity

            // ����
            + t_acceleration * Time.fixedDeltaTime;

        // ��Į ��ǥ�� ���� ���� ���� �ӷ�
        t_localVelocityZ = t_velocity.z;

        // ��ü ����
        velocity = m_transform.localRotation

            // ��������
            * new Vector3(
                (float)(t_velocity.x - m_airResist.x * Mathf.Abs(t_velocity.x) * t_velocity.x),
                (float)(t_velocity.y - m_airResist.y * Mathf.Abs(t_velocity.y) * t_velocity.y),
                (float)(t_velocity.z - m_airResist.z * Mathf.Abs(t_velocity.z) * t_velocity.z)
            )

            // �߷°��ӵ�
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;

        // ��ȯ
        return velocity;
    }


    /// <summary>
    /// �������÷��� ������Ʈ
    /// </summary>
    protected void HUDUpdate()
    {
        // ��ü ����
        Vector3 t_rotation = m_transform.localRotation.eulerAngles;
        if (180.0f < t_rotation.x)
        {
            t_rotation.x -= 360.0f;
        }
        if (180.0f < t_rotation.y)
        {
            t_rotation.y -= 360.0f;
        }
        if (180.0f < t_rotation.z)
        {
            t_rotation.z -= 360.0f;
        }

        // ��ü �̵� ����
        float t_velocity = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);
        Vector3 t_direction = Quaternion.Inverse(m_transform.localRotation) * velocity / t_velocity;

        // �ӷ� ǥ��
        m_velocityText.text = Mathf.RoundToInt(t_velocity).ToString();

        // ��귮 ����
        float t_r2d = 180.0f / Mathf.PI;
        float t_radianZ = t_rotation.z * Mathf.PI / 180.0f;
        float t_cosZ = Mathf.Cos(t_radianZ);
        float t_sinZ = Mathf.Sin(t_radianZ);

        // ��, �Ʒ� ���� HUD ��ġ
        m_HUDUpDown.localPosition = new Vector3(
            (t_rotation.x * t_sinZ + Mathf.Atan(t_direction.x) * t_cosZ * t_r2d) * m_HUDUpDownMoveAmount,
            (t_rotation.x * t_cosZ + Mathf.Atan(t_direction.y) * Mathf.Abs(t_sinZ) * t_r2d) * m_HUDUpDownMoveAmount,
            0.0f
        );

        // ��, �Ʒ� ���� HUD ȸ��
        m_HUDUpDown.localRotation = Quaternion.Euler(0.0f, 0.0f, -t_rotation.z);

        // ���� ǥ�� �̹��� ��ġ
        m_directionImage.localPosition = new Vector3(
            Mathf.Atan(t_direction.x) * t_r2d * m_HUDUpDownMoveAmount,
            Mathf.Atan(t_direction.y) * t_r2d * m_HUDUpDownMoveAmount,
            0.0f);
    }
}

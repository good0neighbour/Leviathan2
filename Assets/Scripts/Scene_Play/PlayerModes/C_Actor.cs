using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class C_Actor : MonoBehaviour, I_State<E_PlayState>, I_Actor
{
    /* ========== Fields ========== */

    private enum E_ActorState
    {
        ENABLING,
        PLAYING,
        DISABLING
    }



    /* ========== Fields ========== */

    private List<Material> mp_materials = new List<Material>();
    private SkinnedMeshRenderer[] mp_renderers = null;
    private Animator mp_animator = null;
    private Transform mp_cameraTransform = null;
    private Vector3 m_velocity = Vector3.zero;
    private E_ActorState m_currentState = E_ActorState.PLAYING;
    private float m_velocityScalar = 0.0f;
    private float m_maxWalkSpeed = 0.0f;
    private float m_maxRunSpeed = 0.0f;
    private float m_currentMaxMovingSpeed = 0.0f;
    private float m_accelerator = 0.0f;
    private float m_cameraRotateSpeed = 0.0f;
    private float m_dissolveAmount = 1.0f;
    private short m_hitPoint = 0;
    private byte m_isMoving = 0;
    private bool m_isRunning = false;
    private bool m_aniChange = false;



    /* ========== Public Methods ========== */

    public void ChangeState(E_PlayState t_state)
    {
        gameObject.SetActive(false);
        C_PlayManager.instance.SetState(t_state);
    }


    public void Execute()
    {
        gameObject.SetActive(true);
        m_currentState = E_ActorState.ENABLING;
    }


    public void StateFixedUpdate()
    {
        // �ִ� �ӷ�
        if (m_isRunning)
        {
            if (m_maxRunSpeed > m_currentMaxMovingSpeed)
            {
                m_currentMaxMovingSpeed += m_accelerator * Time.deltaTime;
                if (m_maxRunSpeed < m_currentMaxMovingSpeed)
                {
                    m_currentMaxMovingSpeed = m_maxRunSpeed;
                }
            }
        }
        else
        {
            if (m_maxWalkSpeed < m_currentMaxMovingSpeed)
            {
                m_currentMaxMovingSpeed -= m_accelerator * Time.deltaTime;
                if (m_maxWalkSpeed > m_currentMaxMovingSpeed)
                {
                    m_currentMaxMovingSpeed = m_maxWalkSpeed;
                }
            }
        }

        // ����
        if (0 < m_isMoving && m_currentMaxMovingSpeed >= m_velocityScalar)
        {
            // Animator�� �� ����
            m_aniChange = true;

            #region �����¿� ������
            // ����
            if (0 < (m_isMoving & C_Constants.ACTOR_FORWARD))
            {
                if (m_currentMaxMovingSpeed > m_velocity.z)
                {
                    m_velocity.z += m_accelerator * Time.fixedDeltaTime;
                    if (m_currentMaxMovingSpeed < m_velocity.z)
                    {
                        m_velocity.z = m_currentMaxMovingSpeed;
                    }
                }
            }
            else if (0.0f < m_velocity.z)
            {
                m_velocity.z -= m_accelerator * Time.fixedDeltaTime;
                if (0.0f > m_velocity.z)
                {
                    m_velocity.z = 0.0f;
                }
            }

            // �Ĺ�
            if (0 < (m_isMoving & C_Constants.ACTOR_BACKWARD))
            {
                if (-m_currentMaxMovingSpeed < m_velocity.z)
                {
                    m_velocity.z -= m_accelerator * Time.fixedDeltaTime;
                    if (-m_currentMaxMovingSpeed > m_velocity.z)
                    {
                        m_velocity.z = -m_currentMaxMovingSpeed;
                    }
                }
            }
            else if (0.0f > m_velocity.z)
            {
                m_velocity.z += m_accelerator * Time.fixedDeltaTime;
                if (0.0f < m_velocity.z)
                {
                    m_velocity.z = 0.0f;
                }
            }

            // �¹�
            if (0 < (m_isMoving & C_Constants.ACTOR_LEFT))
            {
                if (-m_currentMaxMovingSpeed < m_velocity.x)
                {
                    m_velocity.x -= m_accelerator * Time.fixedDeltaTime;
                    if (-m_currentMaxMovingSpeed > m_velocity.x)
                    {
                        m_velocity.x = -m_currentMaxMovingSpeed;
                    }
                }
            }
            else if (0.0f > m_velocity.x)
            {
                m_velocity.x += m_accelerator * Time.fixedDeltaTime;
                if (0.0f < m_velocity.x)
                {
                    m_velocity.x = 0.0f;
                }
            }

            // ���
            if (0 < (m_isMoving & C_Constants.ACTOR_RIGHT))
            {
                if (m_currentMaxMovingSpeed > m_velocity.x)
                {
                    m_velocity.x += m_accelerator * Time.fixedDeltaTime;
                    if (m_currentMaxMovingSpeed < m_velocity.x)
                    {
                        m_velocity.x = m_currentMaxMovingSpeed;
                    }
                }
            }
            else if (0.0f < m_velocity.x)
            {
                m_velocity.x -= m_accelerator * Time.fixedDeltaTime;
                if (0.0f > m_velocity.x)
                {
                    m_velocity.x = 0.0f;
                }
            }
            #endregion

            // �밢�� �ӷ� ����
            float t_velocity = Mathf.Sqrt(m_velocity.x * m_velocity.x + m_velocity.z * m_velocity.z);
            if (m_currentMaxMovingSpeed < t_velocity)
            {
                // �ӷ� ����
                float t_divide = m_currentMaxMovingSpeed / t_velocity;
                m_velocity.x *= t_divide;
                m_velocity.z *= t_divide;

                // �� �ӷ� ����ȭ
                m_velocityScalar = m_currentMaxMovingSpeed;
            }
            else
            {
                // �� �ӷ� ����ȭ
                m_velocityScalar = t_velocity;
            }
        }
        // ����
        else if (0.0f < m_velocityScalar)
        {
            // �ӷ�
            float t_velocity = Mathf.Sqrt(m_velocity.x * m_velocity.x + m_velocity.z * m_velocity.z);

            // �ӷ� ����
            float t_reducement = t_velocity - m_accelerator * Time.fixedDeltaTime;
            if (0.0f < t_reducement)
            {
                float t_multiply = t_reducement / t_velocity;
                m_velocity.x *= t_multiply;
                m_velocity.z *= t_multiply;
            }
            else
            {
                m_velocity.x = 0.0f;
                m_velocity.z = 0.0f;
            }

            // �� �ӷ� ����ȭ
            m_velocityScalar = t_reducement;
        }

        // �ӷ��� 0 �̻��̸�
        if (0.0f < m_velocityScalar)
        {
            // ī�޶� ����
            Quaternion t_camRot = Quaternion.Euler(
                0.0f,
                mp_cameraTransform.localRotation.eulerAngles.y,
                0.0f
            );

            // �̵�
            transform.localPosition += t_camRot * (m_velocity * Time.deltaTime);

            // ȸ��
            float t_angle = Mathf.Atan(m_velocity.x / m_velocity.z) * Mathf.Rad2Deg;
            if (0.0f > m_velocity.z)
            {
                t_angle += 180f;
            }
            transform.localRotation = t_camRot * Quaternion.Euler(0.0f, t_angle, 0.0f);

            // ī�޶� ȸ��
            mp_cameraTransform.localRotation *= Quaternion.Euler(
                0.0f,
                m_velocity.x * m_cameraRotateSpeed * Time.fixedDeltaTime,
                0.0f
            );
        }
    }


    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN

        #region ����
        // ���� �̵�
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_isMoving |= C_Constants.ACTOR_FORWARD;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            m_isMoving ^= C_Constants.ACTOR_FORWARD;
        }

        // �Ĺ� �̵�
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_isMoving |= C_Constants.ACTOR_BACKWARD;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            m_isMoving ^= C_Constants.ACTOR_BACKWARD;
        }

        // �¹� �̵�
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_isMoving |= C_Constants.ACTOR_LEFT;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            m_isMoving ^= C_Constants.ACTOR_LEFT;
        }

        // ��� �̵�
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_isMoving |= C_Constants.ACTOR_RIGHT;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            m_isMoving ^= C_Constants.ACTOR_RIGHT;
        }

        // �޸���
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_isRunning = false;
        }
        #endregion
        #region ���� ����
        if (Input.GetKeyDown(KeyCode.B) && E_ActorState.PLAYING == m_currentState)
        {
            m_currentState = E_ActorState.DISABLING;
        }
        #endregion
#endif
        // �̵� �ÿ��� Animator�� �� ����
        if (m_aniChange)
        {
            mp_animator.SetFloat("MovingSpeed", m_velocityScalar);
            switch (m_velocityScalar)
            {
                case 0.0f:
                    m_aniChange = false;
                    break;
            }
        }

        // Dissolve
        switch (m_currentState)
        {
            case E_ActorState.ENABLING:
                m_dissolveAmount -= Time.deltaTime;
                if (0.0f >= m_dissolveAmount)
                {
                    m_dissolveAmount = 0.0f;
                    m_currentState = E_ActorState.PLAYING;
                }
                DissolveMaterials(m_dissolveAmount);
                return;

            case E_ActorState.DISABLING:
                m_dissolveAmount += Time.deltaTime;
                if (1.0f <= m_dissolveAmount)
                {
                    m_dissolveAmount = 1.0f;
                    ChangeState(E_PlayState.GUIDEDMISSLE);
                }
                DissolveMaterials(m_dissolveAmount);
                return;
        }
    }


    public void Hit(byte t_damage)
    {
        switch (m_currentState)
        {
            case E_ActorState.PLAYING:
                m_hitPoint -= t_damage;
                if (0 >= m_hitPoint)
                {
                    Die();
                }
                return;

            default:
                // �ٸ� ���¿����� �����̴�.
                return;
        }
    }


    public void Die()
    {
        ChangeState(E_PlayState.AIRPLANE);
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// ��Ÿ������ DissolveAmount �� ����
    /// </summary>
    private void DissolveMaterials(float t_amount)
    {
        foreach (Material t_mtl in mp_materials)
        {
            t_mtl.SetFloat("_DissolveAmount", t_amount);
        }
    }


    private void Awake()
    {
        // ���� �����´�.
        C_ActorSettings t_settings = Resources.Load< C_ActorSettings>("ActorSettings");
        m_maxWalkSpeed = t_settings.m_maxWalkSpeed;
        m_maxRunSpeed = t_settings.m_maxRunSpeed;
        m_accelerator = t_settings.m_accelerator;
        m_cameraRotateSpeed = t_settings.m_cameraRotateSpeed;
        m_hitPoint = t_settings.m_hitPoint;

        // ��Ÿ���� ����
        List<string> tp_nameList = new List<string>();
        mp_renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer t_ren in mp_renderers)
        {
            // ��Ÿ���� �迭 ����
            byte t_length = (byte)t_ren.materials.Length;
            Material[] tp_materials = new Material[t_length];

            // MeshRenderer�� ������ �ִ� ��Ÿ���� ���� ��ȸ
            for (byte t_i = 0; t_i < t_length; ++t_i)
            {
                // �̹� ������ ��Ÿ�������� �˻�
                byte t_j;
                byte t_nameLength = (byte)tp_nameList.Count;
                for (t_j = 0; t_j < t_nameLength; ++t_j)
                {
                    if (t_ren.materials[t_i].name.Equals(tp_nameList[t_j]))
                    {
                        tp_materials[t_i] = mp_materials[t_j];
                        break;
                    }
                }

                // ������ ��Ÿ������ ���� ���
                if (t_nameLength == t_j)
                {
                    Material t_mtl = new Material(t_ren.materials[t_i]);
                    tp_nameList.Add(t_ren.materials[t_i].name);
                    mp_materials.Add(t_mtl);
                    tp_materials[t_i] = t_mtl;
                }
            }

            // ��Ÿ���� �迭 ����
            t_ren.materials = tp_materials;
        }

        // �ȱ� ����
        m_currentMaxMovingSpeed = m_maxWalkSpeed;

        // ����
        mp_animator = GetComponent<Animator>();

        // ó������ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }


    private void Start()
    {
        mp_cameraTransform = C_CameraMove.instance.transform;
    }
}

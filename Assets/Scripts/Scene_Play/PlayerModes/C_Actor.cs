using System.Collections.Generic;
using UnityEngine;

public class C_Actor : MonoBehaviour, I_State<E_PlayStates>, I_Actor
{
    /* ========== Fields ========== */

    private List<Material> mp_materials = new List<Material>();
    private SkinnedMeshRenderer[] mp_renderers = null;
    private Animator mp_animator = null;
    private Transform mp_cameraTransform = null;
    private C_EnemyBase mp_enemyBase = null;
    private Vector3 m_velocity = Vector3.zero;
    private E_ActorStates m_currentState = E_ActorStates.STANDBY;
    private float m_velocityScalar = 0.0f;
    private float m_maxWalkSpeed = 0.0f;
    private float m_maxRunSpeed = 0.0f;
    private float m_currentMaxMovingSpeed = 0.0f;
    private float m_accelerator = 0.0f;
    private float m_cameraRotateSpeed = 0.0f;
    private float m_dissolveAmount = 1.0f;
    private float m_interactRange = 1.0f;
    private float m_maxHitPointMult = 0.0f;
    private float m_conquestSpeed = 0.0f;
    private float m_conquesting = 0.0f;
    private short m_currentHitPoint = 0;
    private byte m_isMoving = 0;
    private byte m_conquestingPhase = 0;
    private bool m_isRunning = false;
    private bool m_aniChange = false;



    /* ========== Public Methods ========== */

    public void ChangeState(E_PlayStates t_state)
    {
        gameObject.SetActive(false);
        C_CanvasActorHUD.instance.CanvasEnable(false);
        C_PlayManager.instance.SetState(t_state);
    }


    public void Execute()
    {
        m_isMoving = 0;
        m_velocity = Vector3.zero;
        m_velocityScalar = 0.0f;
        m_currentMaxMovingSpeed = m_maxWalkSpeed;
        m_conquestingPhase = 0;
        m_isRunning = false;
        m_aniChange = false;
        C_CanvasActorHUD.instance.actor = this;
        C_CanvasActorHUD.instance.SetHitPointBar(m_currentHitPoint * m_maxHitPointMult);
        C_CanvasActorHUD.instance.ConquestDisplayEnable(false);
        gameObject.SetActive(true);
        C_CanvasActorHUD.instance.CanvasEnable(true);
        m_currentState = E_ActorStates.ENABLING;
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
        // �Ϲ� ����
        GeneralControl();

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

        // ���¿� ���� ����
        UpdateByActorState();
    }


    public void Hit(byte t_damage)
    {
        switch (m_currentState)
        {
            case E_ActorStates.STANDBY:
                m_currentHitPoint -= t_damage;
                C_CanvasActorHUD.instance.SetHitPointBar(m_currentHitPoint * m_maxHitPointMult);
                if (0 >= m_currentHitPoint)
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
        ChangeState(E_PlayStates.AIRPLANE);
    }


    /// <summary>
    /// Actor �ʱ�ȭ
    /// </summary>
    public void ActorInitialize(C_ActorSettings tp_settings)
    {
        // ���� ����
        m_maxWalkSpeed = tp_settings.m_maxWalkSpeed;
        m_maxRunSpeed = tp_settings.m_maxRunSpeed;
        m_accelerator = tp_settings.m_accelerator;
        m_cameraRotateSpeed = tp_settings.m_cameraRotateSpeed;
        m_interactRange = tp_settings.m_interactRange;
        m_currentHitPoint = tp_settings.m_hitPoint;
        m_conquestSpeed = tp_settings.m_conquestSpeed;
        m_maxHitPointMult = 1.0f / m_currentHitPoint;

        // �ȱ� ����
        m_currentMaxMovingSpeed = m_maxWalkSpeed;
    }    


    /// <summary>
    /// UI ��ư���� ������ �� ����
    /// </summary>
    public void ButtonConquest(bool t_active)
    {
        if (t_active)
        {
            m_conquestingPhase = 1;
        }
        else
        {
            m_conquestingPhase = 3;
        }
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// ���¿� ���� ����
    /// </summary>
    private void UpdateByActorState()
    {
        switch (m_currentState)
        {
            // ���ܳ���.
            case E_ActorStates.ENABLING:
                m_dissolveAmount -= Time.deltaTime;
                if (0.0f >= m_dissolveAmount)
                {
                    m_dissolveAmount = 0.0f;
                    m_currentState = E_ActorStates.STANDBY;
                }
                DissolveMaterials(m_dissolveAmount);
                return;

            // ��ȣ�ۿ� ����
            case E_ActorStates.STANDBY:
                foreach (Collider t_col in Physics.OverlapSphere(transform.localPosition, m_interactRange))
                {
                    if (t_col.tag.Equals("tag_enemyDevice"))
                    {
                        m_currentState = E_ActorStates.NEARDEVICE;
                        mp_enemyBase = t_col.GetComponentInParent<C_EnemyBase>();
                        C_CanvasActorHUD.instance.ConquestButtonEnable(true);
                        return;
                    }
                }
                return;

            //��ȣ�ۿ�
            case E_ActorStates.NEARDEVICE:
#if PLATFORM_STANDALONE_WIN
                if (Input.GetKeyDown(KeyCode.E))
                {
                    m_conquestingPhase = 1;
                }
                else if (Input.GetKeyUp(KeyCode.E))
                {
                    m_conquestingPhase = 3;
                }
#endif
                // �� ���� ���� ����
                switch (m_conquestingPhase)
                {
                    // ���
                    case 0:
                        break;

                    // ���� ����
                    case 1:
                        C_CanvasActorHUD.instance.ConquestDisplayEnable(true);
                        m_conquesting = 0.0f;
                        m_conquestingPhase = 2;
                        break;

                    // ���� ��
                    case 2:
                        m_conquesting += m_conquestSpeed * Time.deltaTime;
                        if (1.0f <= m_conquesting)
                        {
                            m_conquesting = 1.0f;
                            mp_enemyBase.BaseConquested();
                            C_CanvasActorHUD.instance.ConquestDisplayEnable(false);
                            C_CanvasActorHUD.instance.ConquestButtonEnable(false);
                            m_currentState = E_ActorStates.STANDBY;
                            m_conquestingPhase = 0;
                            return;
                        }
                        C_CanvasActorHUD.instance.SetConquestBar(m_conquesting);
                        break;

                    // ���� ���
                    case 3:
                        C_CanvasActorHUD.instance.ConquestDisplayEnable(false);
                        m_conquestingPhase = 0;
                        break;
                }
                // ������ ������ �� ���� ��ġ�� �ִ��� Ȯ��
                foreach (Collider t_col in Physics.OverlapSphere(transform.localPosition, m_interactRange))
                {
                    if (t_col.tag.Equals("tag_enemyDevice"))
                    {
                        return;
                    }
                }
                m_currentState = E_ActorStates.STANDBY;
                C_CanvasActorHUD.instance.ConquestButtonEnable(false);
                return;

            // �������.
            case E_ActorStates.DISABLING:
                m_dissolveAmount += Time.deltaTime;
                if (1.0f <= m_dissolveAmount)
                {
                    m_dissolveAmount = 1.0f;
                    ChangeState(E_PlayStates.GUIDEDMISSLE);
                }
                DissolveMaterials(m_dissolveAmount);
                return;
        }
    }


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


    /// <summary>
    /// �Ϲ� ����
    /// </summary>
    private void GeneralControl()
    {
#if PLATFORM_STANDALONE_WIN
        // ���� �̵�
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_isMoving += C_Constants.ACTOR_FORWARD;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            m_isMoving ^=  C_Constants.ACTOR_FORWARD;
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
        // ���� ����
        if (Input.GetKeyDown(KeyCode.B) && E_ActorStates.STANDBY == m_currentState)
        {
            m_currentState = E_ActorStates.DISABLING;
        }
#endif
    }


    private void Awake()
    {
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

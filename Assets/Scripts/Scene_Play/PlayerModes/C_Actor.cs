using System.Collections.Generic;
using UnityEngine;

public class C_Actor : MonoBehaviour, I_State<E_PlayStates>, I_Hitable
{
    /* ========== Fields ========== */

    [SerializeField] private float m_waterDetectRadius = 0.3f;
    [SerializeField] private float m_swimOffsetY = 1.0f;
    [SerializeField] private float m_groundDetectRadius = 0.3f;
    [SerializeField] private float m_groundDetectY = 0.0f;
    [Header("�Ҹ�")]
    [SerializeField] private AudioClip mp_footStep = null;
    [SerializeField] private AudioClip mp_swimWater = null;
    private List<Material> mp_materials = new List<Material>();
    private SkinnedMeshRenderer[] mp_renderers = null;
    private C_ActorInfomation.S_Info mp_actorInfo = null;
    private Animator mp_animator = null;
    private Rigidbody mp_rigidbody = null;
    private AudioSource mp_audioSource = null;
    private Transform mp_cameraTransform = null;
    private Transform mp_waterTransform = null;
    private C_EnemyBase mp_enemyBase = null;
    private C_Joystick mp_joystick = null;
    private E_ActorStates m_currentState = E_ActorStates.STANDBY;
    private float m_maxSpeed = 0.0f;
    private float m_cameraRotateSpeed = 0.0f;
    private float m_dissolveAmount = 1.0f;
    private float m_interactRange = 1.0f;
    private float m_maxHitPointMult = 0.0f;
    private float m_conquestSpeed = 0.0f;
    private float m_conquesting = 0.0f;
    private float m_joystickScalar = 0.0f;
    private float m_currentHitPoint = 0.0f;
    private byte m_conquestingPhase = 0;
    private byte m_actorIndex = 0;
    private bool m_waterDetect = false;
#if PLATFORM_STANDALONE_WIN
    private float m_accelerator = 0.0f;
#endif



    /* ========== Public Methods ========== */

    public void ChangeState(E_PlayStates t_state)
    {
        gameObject.SetActive(false);
        C_CanvasActorHUD.instance.CanvasEnable(false);
        C_PlayManager.instance.SetState(t_state);
    }


    public void Execute()
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.ACTOR_SUMMON);
        m_conquestingPhase = 0;
        mp_rigidbody.useGravity = true;
        m_waterDetect = false;
        mp_animator.SetBool("WaterDetect", false);
        C_CanvasActorHUD.instance.SetActor(
            this,
            m_currentHitPoint * m_maxHitPointMult,
            mp_actorInfo.mp_actorPortrait
        );
        gameObject.SetActive(true);
        m_currentState = E_ActorStates.ENABLING;
    }


    public void StateFixedUpdate()
    {
        // UI ���̽�ƽ ��Į�� ��
        m_joystickScalar = mp_joystick.value.magnitude;

        // �̵� �ӵ��� 0���� Ŭ ����
        if (0.0f < m_joystickScalar)
        {
            float t_velocityZ = mp_joystick.value.y * m_maxSpeed;
            float t_velocityX = mp_joystick.value.x * m_maxSpeed;

            // ī�޶� ����
            Quaternion t_camRot = Quaternion.Euler(
                0.0f,
                mp_cameraTransform.localRotation.eulerAngles.y,
                0.0f
            );

            // �̵�
            transform.localPosition += t_camRot * new Vector3(
                t_velocityX * Time.deltaTime,
                0.0f,
                t_velocityZ * Time.deltaTime
            );

            // ȸ��
            float t_angle = Mathf.Atan(t_velocityX / t_velocityZ) * Mathf.Rad2Deg;
            if (0.0f > t_velocityZ)
            {
                t_angle += 180f;
            }
            transform.localRotation = t_camRot * Quaternion.Euler(0.0f, t_angle, 0.0f);

            // ī�޶� ȸ��
            mp_cameraTransform.localRotation *= Quaternion.Euler(
                0.0f,
                t_velocityX * m_cameraRotateSpeed * Time.fixedDeltaTime,
                0.0f
            );
        }

        if (m_waterDetect)
        {
            // ���� ����
            foreach (Collider t_col in Physics.OverlapSphere(new Vector3(
                transform.localPosition.x,
                transform.localPosition.y + m_groundDetectY,
                transform.localPosition.z
            ), m_groundDetectRadius))
            {
                if (0 < (LayerMask.GetMask("layer_ground") & (1 << t_col.gameObject.layer)))
                {
                    if (m_waterDetect)
                    {
                        // �ȱ� ���·� ��ȯ
                        m_waterDetect = false;
                        mp_animator.SetBool("WaterDetect", false);
                        mp_rigidbody.useGravity = true;
                        mp_audioSource.clip = mp_footStep;
                    }
                    return;
                }
            }
        }

        // �� ����
        foreach (Collider t_col in Physics.OverlapSphere(new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + m_swimOffsetY,
            transform.localPosition.z
        ), m_waterDetectRadius))
        {
            if (0 < (LayerMask.GetMask("layer_water") & (1 << t_col.gameObject.layer)))
            {
                if (!m_waterDetect)
                {
                    // ���� ���·� ��ȯ
                    mp_waterTransform = t_col.transform;
                    m_waterDetect = true;
                    mp_animator.SetBool("WaterDetect", true);
                    mp_rigidbody.useGravity = false;
                    mp_audioSource.clip = mp_swimWater;
                    C_AudioManager.instance.PlayAuido(E_AudioType.DIVE);
                }

                // �������� ��ġ ����
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    mp_waterTransform.localPosition.y - m_swimOffsetY,
                    transform.localPosition.z
                );
                return;
            }
        }
    }


    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN
        // �̵�
        if (Input.GetKey(KeyCode.W))
        {
            mp_joystick.value.y += m_accelerator * Time.deltaTime;
            if (1.0f < mp_joystick.value.y)
            {
                mp_joystick.value.y = 1.0f;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            mp_joystick.value.y -= m_accelerator * Time.deltaTime;
            if (-1.0f > mp_joystick.value.y)
            {
                mp_joystick.value.y = -1.0f;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            mp_joystick.value.x -= m_accelerator * Time.deltaTime;
            if (-1.0f > mp_joystick.value.x)
            {
                mp_joystick.value.x = -1.0f;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            mp_joystick.value.x += m_accelerator * Time.deltaTime;
            if (1.0f < mp_joystick.value.x)
            {
                mp_joystick.value.x = 1.0f;
            }
        }

        // ���� ����
        if (Input.GetKeyDown(KeyCode.B))
        {
            ButtonAeroplane();
        }
#endif
        // Animator�� �� ����
        mp_animator.SetFloat("MovingSpeed", m_joystickScalar);

        // ���¿� ���� ����
        UpdateByActorState();
    }


    public void Hit(byte t_damage)
    {
        switch (m_currentState)
        {
            case E_ActorStates.STANDBY:
            case E_ActorStates.NEARDEVICE:
                m_currentHitPoint -= t_damage;
                C_CanvasActorHUD.instance.SetHitPointBar(m_currentHitPoint * m_maxHitPointMult);
                if (0.0f >= m_currentHitPoint)
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
        C_GuidedMissle.instance.SetActorDead(m_actorIndex);
        ChangeState(E_PlayStates.AIRPLANE);
    }


    /// <summary>
    /// Actor �ʱ�ȭ
    /// </summary>
    public void ActorInitialize(C_ActorSettings tp_settings, C_ActorInfomation.S_Info tp_info, byte t_slotIndex, byte t_actorLevel)
    {
        // Actor ����
        mp_actorInfo = tp_info;

        // ���� ����
        m_cameraRotateSpeed = tp_settings.m_cameraRotateSpeed;
        m_maxSpeed = tp_info.m_maxSpeed + tp_info.m_maxSpeedUp * t_actorLevel;
        m_interactRange = tp_info.m_interactRange + tp_info.m_interactRangeUp * t_actorLevel;
        m_currentHitPoint = tp_info.m_hitPoint + tp_info.m_hitPointUp * t_actorLevel;
        m_conquestSpeed = tp_info.m_conquestSpeed + tp_info.m_conquestSpeedUp * t_actorLevel;
        m_maxHitPointMult = 1.0f / m_currentHitPoint;

        // Actor �ε���
        m_actorIndex = t_slotIndex;

#if PLATFORM_STANDALONE_WIN
        m_accelerator = tp_settings.m_accelerator;
#endif
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


    public void ButtonAeroplane()
    {
        switch (m_currentState)
        {
            case E_ActorStates.STANDBY:
            case E_ActorStates.NEARDEVICE:
                C_AudioManager.instance.PlayAuido(E_AudioType.ACTOR_SUMMON);
                m_currentState = E_ActorStates.DISABLING;
                return;

            default:
                return;
        }
    }


    public void PlayAudioForAnimationEvent()
    {
        mp_audioSource.pitch = m_joystickScalar / m_maxSpeed * 0.7f + 0.3f;
        mp_audioSource.Play();
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
                    ChangeState(E_PlayStates.AIRPLANE);
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
        mp_rigidbody = GetComponent<Rigidbody>();
        mp_audioSource = GetComponent<AudioSource>();
        mp_audioSource.clip = mp_footStep;

        // ó������ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }


    private void Start()
    {
        mp_cameraTransform = C_CameraMove.instance.transform;
        mp_joystick = C_CanvasActorHUD.instance.GetUIJoystick();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + m_swimOffsetY,
            transform.localPosition.z
        ), m_waterDetectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(
            transform.localPosition.x,
            transform.localPosition.y + m_groundDetectY,
            transform.localPosition.z
        ), m_groundDetectRadius);
    }
#endif
}

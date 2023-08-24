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
        // 최대 속력
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

        // 가속
        if (0 < m_isMoving && m_currentMaxMovingSpeed >= m_velocityScalar)
        {
            // Animator에 값 전달
            m_aniChange = true;

            #region 전후좌우 움직임
            // 전방
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

            // 후방
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

            // 좌방
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

            // 우방
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

            // 대각선 속력 제한
            float t_velocity = Mathf.Sqrt(m_velocity.x * m_velocity.x + m_velocity.z * m_velocity.z);
            if (m_currentMaxMovingSpeed < t_velocity)
            {
                // 속력 제한
                float t_divide = m_currentMaxMovingSpeed / t_velocity;
                m_velocity.x *= t_divide;
                m_velocity.z *= t_divide;

                // 총 속력 정규화
                m_velocityScalar = m_currentMaxMovingSpeed;
            }
            else
            {
                // 총 속력 정규화
                m_velocityScalar = t_velocity;
            }
        }
        // 감속
        else if (0.0f < m_velocityScalar)
        {
            // 속력
            float t_velocity = Mathf.Sqrt(m_velocity.x * m_velocity.x + m_velocity.z * m_velocity.z);

            // 속력 감소
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

            // 총 속력 정규화
            m_velocityScalar = t_reducement;
        }

        // 속력이 0 이상이면
        if (0.0f < m_velocityScalar)
        {
            // 카메라 방향
            Quaternion t_camRot = Quaternion.Euler(
                0.0f,
                mp_cameraTransform.localRotation.eulerAngles.y,
                0.0f
            );

            // 이동
            transform.localPosition += t_camRot * (m_velocity * Time.deltaTime);

            // 회전
            float t_angle = Mathf.Atan(m_velocity.x / m_velocity.z) * Mathf.Rad2Deg;
            if (0.0f > m_velocity.z)
            {
                t_angle += 180f;
            }
            transform.localRotation = t_camRot * Quaternion.Euler(0.0f, t_angle, 0.0f);

            // 카메라 회전
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

        #region 조작
        // 전방 이동
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_isMoving |= C_Constants.ACTOR_FORWARD;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            m_isMoving ^= C_Constants.ACTOR_FORWARD;
        }

        // 후방 이동
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_isMoving |= C_Constants.ACTOR_BACKWARD;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            m_isMoving ^= C_Constants.ACTOR_BACKWARD;
        }

        // 좌방 이동
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_isMoving |= C_Constants.ACTOR_LEFT;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            m_isMoving ^= C_Constants.ACTOR_LEFT;
        }

        // 우방 이동
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_isMoving |= C_Constants.ACTOR_RIGHT;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            m_isMoving ^= C_Constants.ACTOR_RIGHT;
        }

        // 달리기
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_isRunning = false;
        }
        #endregion
        #region 상태 변경
        if (Input.GetKeyDown(KeyCode.B) && E_ActorState.PLAYING == m_currentState)
        {
            m_currentState = E_ActorState.DISABLING;
        }
        #endregion
#endif
        // 이동 시에만 Animator에 값 전달
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
                // 다른 상태에서는 무적이다.
                return;
        }
    }


    public void Die()
    {
        ChangeState(E_PlayState.AIRPLANE);
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// 메타리얼의 DissolveAmount 값 전달
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
        // 설정 가져온다.
        C_ActorSettings t_settings = Resources.Load< C_ActorSettings>("ActorSettings");
        m_maxWalkSpeed = t_settings.m_maxWalkSpeed;
        m_maxRunSpeed = t_settings.m_maxRunSpeed;
        m_accelerator = t_settings.m_accelerator;
        m_cameraRotateSpeed = t_settings.m_cameraRotateSpeed;
        m_hitPoint = t_settings.m_hitPoint;

        // 메타리얼 복사
        List<string> tp_nameList = new List<string>();
        mp_renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (Renderer t_ren in mp_renderers)
        {
            // 메타리얼 배열 생성
            byte t_length = (byte)t_ren.materials.Length;
            Material[] tp_materials = new Material[t_length];

            // MeshRenderer가 가지고 있는 메타리얼 전부 순회
            for (byte t_i = 0; t_i < t_length; ++t_i)
            {
                // 이미 복사한 메타리얼인지 검사
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

                // 복사한 메타리얼이 없을 경우
                if (t_nameLength == t_j)
                {
                    Material t_mtl = new Material(t_ren.materials[t_i]);
                    tp_nameList.Add(t_ren.materials[t_i].name);
                    mp_materials.Add(t_mtl);
                    tp_materials[t_i] = t_mtl;
                }
            }

            // 메타리얼 배열 전달
            t_ren.materials = tp_materials;
        }

        // 걷기 상태
        m_currentMaxMovingSpeed = m_maxWalkSpeed;

        // 참조
        mp_animator = GetComponent<Animator>();

        // 처음에는 비활성화
        gameObject.SetActive(false);
    }


    private void Start()
    {
        mp_cameraTransform = C_CameraMove.instance.transform;
    }
}

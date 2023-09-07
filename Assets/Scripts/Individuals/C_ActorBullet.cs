using UnityEngine;

public class C_ActorBullet : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Vector3 m_startPosition = Vector3.zero;
    [SerializeField] private Vector3 m_goalPosition = Vector3.zero;
    [SerializeField][Range(-1.0f, 0.0f)] private float m_angle = -0.5f;
    private float m_distanceX = 0.0f;
    private float m_distanceZ = 0.0f;
    private float m_distanceXZ = 0.0f;
    private float t_a = 0.0f;
    private float t_b = 0.0f;
    private float t_current = 0.0f;



    /* ========== Private Methods ========== */

    private void OnEnable()
    {
        // XZ 평면 상 거리
        m_distanceX = m_goalPosition.x - m_startPosition.x;
        m_distanceZ = m_goalPosition.z - m_startPosition.z;
        m_distanceXZ = Mathf.Sqrt(m_distanceX * m_distanceX + m_distanceZ * m_distanceZ);

        // 이차함수의 상수 값
        float t_temp = m_distanceXZ * 0.5f - (m_goalPosition.y - m_startPosition.y) * 0.5f * m_angle / m_distanceXZ;
        t_b = t_temp * t_temp / -m_angle;
        t_a = -Mathf.Sqrt(-t_b * m_angle);

        // 처음위치
        t_current = 0.0f;

        Debug.Log($"y = {1.0f / m_angle} * (x + {t_a})^2 + {t_b}");
    }


    private void Update()
    {
        t_current += Time.deltaTime;
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            m_angle * (t_current + t_a) * (t_current + t_a) + t_b,
            transform.localPosition.z
        );
        if (0.0f > transform.localPosition.y)
        {
            gameObject.SetActive(false);
        }
    }
}

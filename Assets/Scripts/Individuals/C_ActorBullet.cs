using UnityEngine;

public class C_ActorBullet : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private float m_angle = -0.1f;
    [SerializeField] private float m_velocityMult = -4.9f;
    [SerializeField] private float m_disableHeight = -2.0f;
    [SerializeField] private byte m_damage = 1;
    private float m_a = 0.0f;
    private float m_b = 0.0f;
    private float m_velocityXZ = 0.0f;
    private float m_sin = 0.0f;
    private float m_cos = 0.0f;
    private float m_current = 0.0f;
    private uint m_targetLayer = 0;

    [SerializeField] public Vector3 startPosition;
    //{
    //    get;
    //    set;
    //}

    [SerializeField] public Vector3 goalPosition;
    //{
    //    get;
    //    set;
    //}



    /* ========== Private Methods ========== */

    private void DisableThis()
    {
        gameObject.SetActive(false);
        //C_ObjectPool.instance.ReturnObject(gameObject, E_ObjectPool.ACTORBULLET);
        //Destroy(gameObject);
    }


    private void Awake()
    {
        // �浹 ���̾� ����
        m_targetLayer = (uint)(LayerMask.GetMask("layer_ground")
            + LayerMask.GetMask("layer_stencilWall")
            + LayerMask.GetMask("layer_water"));
    }


    private void OnEnable()
    {
        // XZ ��� �� �Ÿ�
        float t_distanceX = goalPosition.x - startPosition.x;
        float t_distanceZ = goalPosition.z - startPosition.z;
        float t_distanceXZ = Mathf.Sqrt(t_distanceX * t_distanceX + t_distanceZ * t_distanceZ);

        // ��� ��
        float t_temp = t_distanceXZ + (startPosition.y - goalPosition.y) / (m_angle * t_distanceXZ);
        m_b = startPosition.y - m_angle * 0.25f * t_temp * t_temp;
        m_a = -Mathf.Sqrt((startPosition.y - m_b) / m_angle);

        // ��� ����
        float m_radian = Mathf.Atan(t_distanceX / t_distanceZ);
        if (0.0f > t_distanceZ)
        {
            m_radian += Mathf.PI;
        }
        m_sin = Mathf.Sin(m_radian);
        m_cos = Mathf.Cos(m_radian);
        m_velocityXZ = m_velocityMult / m_angle;

        // ó����ġ
        m_current = 0.0f;
    }


    private void FixedUpdate()
    {
        // �����Լ� �׷����� ���� �̵�
        float t_temp = m_current + m_a;
        transform.localPosition = new Vector3(
            startPosition.x + m_sin * m_current,
            m_angle * t_temp * t_temp + m_b,
            startPosition.z + m_cos * m_current
        );

        // �浹 ����
        foreach (Collider tp_col in Physics.OverlapSphere(transform.localPosition, 0.5f))
        {
            if (tp_col.tag.Equals("tag_landForce") || tp_col.tag.Equals("tag_oceanForce"))
            {
                tp_col.GetComponent<I_Hitable>().Hit(m_damage);
                DisableThis();
                return;
            }
            else if (0 < ((1 << tp_col.gameObject.layer) & m_targetLayer))
            {
                DisableThis();
                return;
            }
        }

        // �ּ� ���̿� ����� ��
        if (m_disableHeight > transform.localPosition.y)
        {
            DisableThis();
            return;
        }

        // �ð� ��ȭ
        m_current += Time.deltaTime * m_velocityXZ;
    }
}

using UnityEngine;

public class C_ParticleEnabled : MonoBehaviour
{
    private ParticleSystem mp_particle = null;
    private float m_duration = 0.0f;
    private float m_timer = 0.0f;

    private void Awake()
    {
        mp_particle = GetComponent<ParticleSystem>();
        m_duration = mp_particle.main.duration;
    }

    private void OnEnable()
    {
        m_timer = 0.0f;
        mp_particle.Play();
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_duration < m_timer)
        {
            // 비활성화
            gameObject.SetActive(false);

            // ObjectPool에 되돌린다.
            C_ObjectPool.instance.ReturnObject(gameObject, E_ObjectPool.EXPLOSION);
        }
    }
}

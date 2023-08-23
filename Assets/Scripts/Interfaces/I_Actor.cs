public interface I_Actor
{
    /// <summary>
    /// 피격 시
    /// </summary>
    public void Hit(byte t_damage);

    /// <summary>
    /// 사망 시
    /// </summary>
    public void Die();
}

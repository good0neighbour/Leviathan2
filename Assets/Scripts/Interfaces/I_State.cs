public interface I_State<T>
{
    /// <summary>
    /// ���� ���� �� �� �� ����
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// ���� ����
    /// </summary>
    public abstract void ChangeState(T t_state);

    /// <summary>
    /// Update �Լ��� ȣ��
    /// </summary>
    public abstract void StateUpdate();

    /// <summary>
    /// FixedUpdate �Լ��� ȣ��
    /// </summary>
    public abstract void StateFixedUpdate();
}

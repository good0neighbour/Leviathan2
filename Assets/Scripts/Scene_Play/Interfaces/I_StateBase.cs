public interface I_StateBase
{
    /// <summary>
    /// ���� ���� �� �� �� ����
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// ���� ����
    /// </summary>
    public abstract void ChangeState();

    /// <summary>
    /// Update �Լ��� ȣ��
    /// </summary>
    public abstract void StateUpdate();

    /// <summary>
    /// FixedUpdate �Լ��� ȣ��
    /// </summary>
    public abstract void StateFixedUpdate();
}

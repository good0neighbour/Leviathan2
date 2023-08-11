public abstract class C_StateBase
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
    public abstract void Update();

    /// <summary>
    /// FixedUpdate �Լ��� ȣ��
    /// </summary>
    public abstract void FixedUpdate();
}

public abstract class C_StateBase
{
    /// <summary>
    /// 상태 변경 후 한 번 실행
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// 상태 변경
    /// </summary>
    public abstract void ChangeState();

    /// <summary>
    /// Update 함수에 호출
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// FixedUpdate 함수에 호출
    /// </summary>
    public abstract void FixedUpdate();
}

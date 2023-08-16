public interface I_State<T>
{
    /// <summary>
    /// 상태 변경 후 한 번 실행
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// 상태 변경
    /// </summary>
    public abstract void ChangeState(T t_state);

    /// <summary>
    /// Update 함수에 호출
    /// </summary>
    public abstract void StateUpdate();

    /// <summary>
    /// FixedUpdate 함수에 호출
    /// </summary>
    public abstract void StateFixedUpdate();
}

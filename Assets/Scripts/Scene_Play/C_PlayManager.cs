using UnityEngine;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayStates>
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_actor = null;
    [SerializeField] private C_MinionSettings mp_minionSettings = null;
    private I_State<E_PlayStates>[] mp_states = new I_State<E_PlayStates>[(int)E_PlayStates.END];

    public static C_PlayManager instance
    {
        get;
        set;
    }

    public D_PlayDelegate earlyFixedUpdate
    {
        get;
        set;
    }

    public D_PlayDelegate lateFixedUpdate
    {
        get;
        set;
    }

    public Vector3 playerBasePosition
    {
        get;
        private set;
    }

    public Vector3 landForceBasePosition
    {
        get;
        private set;
    }

    public E_PlayStates currentState
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public void SetState(E_PlayStates t_state)
    {
        // ���� ����
        currentState = t_state;
        C_CameraMove.instance.SetState(currentState);

        // ���� ����
        mp_states[(int)currentState].Execute();
    }


    public C_MinionSettings GetEnemySettings()
    {
        return mp_minionSettings;
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // ����Ƽ�� �̱�������
        instance = this;

        // �÷��̾� ���� ��ġ
        playerBasePosition = transform.Find("PlayerBase").localPosition;

        // ������� ���� ��ġ
        landForceBasePosition = transform.Find("LandForceBase").localPosition;
    }


    private void Start()
    {
        // Actor ���� �����´�.
        C_ActorSettings tp_actSet = Resources.Load<C_ActorSettings>("ActorSettings");

        // ���� ����
        I_State<E_PlayStates> tp_state = FindFirstObjectByType<C_AirPlane>(FindObjectsInactive.Include);
        mp_states[(int)E_PlayStates.AIRPLANE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.AIRPLANE, ((C_AirPlane)tp_state).transform);

        tp_state = FindFirstObjectByType<C_GuidedMissle>(FindObjectsInactive.Include);
        mp_states[(int)E_PlayStates.GUIDEDMISSLE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.GUIDEDMISSLE, ((C_GuidedMissle)tp_state).transform);

        C_Actor tp_actor = Instantiate(mp_actor).GetComponent<C_Actor>();
        tp_actor.ActorInitialize(tp_actSet);
        C_GuidedMissle.instance.SetActor(tp_actor.transform);
        mp_states[(int)E_PlayStates.ACTOR] = tp_actor;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.ACTOR, tp_actor.transform);

        // ó�� ����
        currentState = E_PlayStates.AIRPLANE;

        // ���� ����
        SetState(currentState);
    }


    private void Update()
    {
        mp_states[(int)currentState].StateUpdate();
    }


    private void FixedUpdate()
    {
        // ���� FixedUpdate
        earlyFixedUpdate?.Invoke();

        // ������
        mp_states[(int)currentState].StateFixedUpdate();

        // ���� FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}

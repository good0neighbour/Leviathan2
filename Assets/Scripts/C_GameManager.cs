public class C_GameManager
{
    /* ========== Fields ========== */

    public static C_GameManager mp_instance = null;
    private C_ActorInfomation.S_Info[] mp_actorList = new C_ActorInfomation.S_Info[C_Constants.NUM_OF_ACTOR_LIMIT];

    public static C_GameManager instance
    {
        get
        {
            switch (mp_instance)
            {
                case null:
                    mp_instance = new C_GameManager();
                    return mp_instance;

                default:
                    return mp_instance;
            }
        }
    }

    public bool gameWin
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void SetActorList(C_ActorInfomation.S_Info[] tp_actorList)
    {
        mp_actorList = tp_actorList;
    }


    public C_ActorInfomation.S_Info[] GetActorList()
    {
        return mp_actorList;
    }
}

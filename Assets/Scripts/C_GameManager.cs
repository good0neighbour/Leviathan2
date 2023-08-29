public class C_GameManager
{
    /* ========== Fields ========== */

    public static C_GameManager mp_instance = null;

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
}

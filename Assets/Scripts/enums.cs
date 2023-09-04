public enum E_NodeStatuss
{
    SUCCESS,
    FAILURE,
    RUNNING
}

public enum E_PlayStates
{
    AIRPLANE,
    GUIDEDMISSILE,
    ACTOR,
    END
}

public enum E_FlightStates
{
    HOVER,
    FLIGHT,
    END
}

public enum E_GuidedMissleStates
{
    BROWSING,
    LAUNCHING,
    END
}

public enum E_ActorStates
{
    ENABLING,
    STANDBY,
    NEARDEVICE,
    DISABLING,
    END
}

public enum E_ObjectPool
{
    ATTACKENEMY_LANDFORCE,
    ATTACKENEMY_OCEANFORCE,
    ALLYMINION,
    EXPLOSION,
    END
}

public enum E_LanguageType
{
    KOREAN,
    ENGLISH,
    END
}

public enum E_AudioType
{
    TOUCH,
    SELECT,
    SWITCH_HOVER,
    SWITCH_FLIGHT,
    GUIDEDMISSILE_TOUCH,
    FOOT_STEP,
    SWIM_WATER,
}
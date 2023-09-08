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
    ACTORBULLET,
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
    ALERT,
    SWITCH_HOVER,
    SWITCH_FLIGHT,
    GUIDEDMISSILE_TOUCH,
    DIVE,
    STEALTH,
    ACTOR_SUMMON,
}
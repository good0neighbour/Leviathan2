public enum E_NodeStatuss
{
    SUCCESS,
    FAILURE,
    RUNNING
}

public enum E_PlayStates
{
    AIRPLANE,
    GUIDEDMISSLE,
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
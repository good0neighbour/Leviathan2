using UnityEngine;

public static class C_Constants
{
    // ����
    public const float DOUBLE_PI = Mathf.PI * 2.0f;

    // C_Actor ����, Actor �̵� ����
    public const byte ACTOR_FORWARD = 0b00001;
    public const byte ACTOR_BACKWARD = 0b00010;
    public const byte ACTOR_LEFT = 0b00100;
    public const byte ACTOR_RIGHT = 0b01000;

    // C_Actor ����, ���� ����
    public const byte CONQUEST_STANDBY = 0;
    public const byte CONQUEST_START = 1;
    public const byte CONQUEST_PROGRESSING = 2;
    public const byte CONQUEST_CANCEL = 3;

    // C_Airplane ����, ���� Ȱ��ȭ
    public const byte STEALTH_ENABLE = 0b01;
    public const byte STEALTH_ANIMATION = 0b10;

    // C_StateHover ����, ��� ��ȯ ����
    public const byte HOVER_STANDBY = 0;
    public const byte HOVER_UNAVAILABLE = 1;
    public const byte HOVER_ACTIVATE = 2;

    // C_Enemy ����, �ൿƮ�� ����
    public const byte ENEMY_BASICACTION = 0;
    public const byte ENEMY_HEAD_TO_ENEMY = 1;
    public const byte ENEMY_ATTACK = 2;

    // C_PlayerBase ����, �޼��� ����
    public const float MESSAGE_TIME = 10.0f;
    public const byte PLAYER_HALFHITPOINT = 0b01;
    public const byte PLAYER_LOWHITPOINT = 0b10;

    // C_ObjectPool ����, ���� ����
    public const byte LANDFORCELIMIT = 16;
    public const byte OCEANFORCELIMIT = 16;
    public const byte ALLYLIMIT = 16;

    // C_Message ����, �޼��� ����
    public const float MESSAGEBOX_DELAY = 2.0f;
    public const float MESSAGEBOX_APPEARING_TIME = 0.5f;
    public const float MESSAGEBOX_SCALEMULT_Y = 1.0f / MESSAGEBOX_APPEARING_TIME;

    // C_Message ����, �޼��� ����
    public const byte MESSAGE_STANDBY = 0;
    public const byte MESSAGE_SHOWING = 1;
    public const byte MESSAGE_DONE = 2;

    // �Ϲ�
    public const float DISTANCE_FADE = 500.0f;
    public const float CAMERA_ANGLE = 45.0f;
    public const float ALLY_SPAWN_TIME = 25.0f;
    public const byte NUM_OF_ACTOR_LIMIT = 3;
}

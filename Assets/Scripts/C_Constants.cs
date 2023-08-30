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

    // �Ϲ�
    public const float DISTANCE_FADE = 500.0f;
}

using UnityEngine;

public static class C_Constants
{
    // 수학
    public const float DOUBLE_PI = Mathf.PI * 2.0f;

    // C_Actor 전용, Actor 이동 방향
    public const byte ACTOR_FORWARD = 0b00001;
    public const byte ACTOR_BACKWARD = 0b00010;
    public const byte ACTOR_LEFT = 0b00100;
    public const byte ACTOR_RIGHT = 0b01000;

    // C_Airplane 전용, 은폐 활성화
    public const byte STEALTH_ENABLE = 0b01;
    public const byte STEALTH_ANIMATION = 0b10;

    // C_StateHover 전용, 모드 전환 상태
    public const byte HOVER_STANDBY = 0;
    public const byte HOVER_UNAVAILABLE = 1;
    public const byte HOVER_ACTIVATE = 2;

    // C_Enemy 전용, 행동트리 상태
    public const byte ENEMY_BASICACTION = 0;
    public const byte ENEMY_HEAD_TO_ENEMY = 1;
    public const byte ENEMY_ATTACK = 2;

    // 일반
    public const float DISTANCE_FADE = 500.0f;
}

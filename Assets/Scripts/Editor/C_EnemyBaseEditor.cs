using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(C_EnemyBase))]
public class C_EnemyBaseEditor : Editor
{
    private void OnSceneGUI()
    {
        C_EnemyBase tp_base = (C_EnemyBase)target;

        Vector2 tCurPos = tp_base.attackEnemySpawnPosint;
        Vector3 t_handlePos = Handles.PositionHandle(
            new Vector3(tCurPos.x, 0.0f, tCurPos.y),
            Quaternion.identity
        );
        tp_base.attackEnemySpawnPosint = new Vector2(t_handlePos.x, t_handlePos.z);
    }

    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset EnemySpawnPoint"))
        {
            C_EnemyBase tp_base = (C_EnemyBase)target;
            tp_base.attackEnemySpawnPosint = new Vector2(
                tp_base.transform.localPosition.x + 10.0f,
                tp_base.transform.localPosition.z
            );
        }
    }
}

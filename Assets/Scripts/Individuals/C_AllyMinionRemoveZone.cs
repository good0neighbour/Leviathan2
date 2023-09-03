using UnityEngine;

public class C_AllyMinionRemoveZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("tag_player"))
        {
            C_AllyMinion t_ally = other.GetComponent<C_AllyMinion>();
            switch (t_ally)
            {
                case null:
                    return;

                default:
                    t_ally.Die();
                    return;
            }
        }
    }
}

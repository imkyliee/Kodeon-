using UnityEngine;

public class GroundCheckerr : MonoBehaviour
{
    public Movement movement;
    public LayerMask groundLayer;

    private int groundContacts;

    private void OnTriggerEnter(Collider other)
    {
        if (IsGround(other))
        {
            Debug.Log("ENTER GROUND");
            groundContacts++;
            movement.SetGrounded(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsGround(other))
        {
            Debug.Log("EXIT GROUND");
            groundContacts--;

            if (groundContacts <= 0)
            {
                groundContacts = 0;
                movement.SetGrounded(false);
            }
        }
    }

    bool IsGround(Collider other)
    {
        return ((1 << other.gameObject.layer) & groundLayer) != 0;
    }
}
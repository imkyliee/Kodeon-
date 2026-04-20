using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    public float interactDistance;
    public Animator doorAnimator;
    public GameObject UI;

    private bool isLookingAtDoor = false;

    void Start()
    {
        UI.SetActive(false);
    }
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        bool hitDoor = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("DungeonDoor"))
            {
                hitDoor = true;
            }
        }

        if (hitDoor && !isLookingAtDoor)
        {
            doorAnimator.SetBool("IsOpen", true);
            UI.SetActive(true);
            isLookingAtDoor = true;
        }
        else if (!hitDoor && isLookingAtDoor)
        {
            doorAnimator.SetBool("IsOpen", false);
            UI.SetActive(false);
            isLookingAtDoor = false;
        }
    }
}
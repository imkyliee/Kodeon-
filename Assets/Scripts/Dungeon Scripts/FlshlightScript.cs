using UnityEngine;

public class FlshlightScript : MonoBehaviour
{
    [SerializeField] private GameObject flashlightObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            flashlightObject?.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            flashlightObject?.SetActive(false);
        }
    }
}

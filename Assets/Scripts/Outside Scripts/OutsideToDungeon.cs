using UnityEngine;

public class OutsideToDungeon : MonoBehaviour
{
    public string sceneToLoad;
    public SceneTransition transition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (transition != null)
            {
                transition.OnButtonPressed(sceneToLoad);
            }
        }
    }
}
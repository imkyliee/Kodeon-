using UnityEngine;

public class MainMenuProxy : MonoBehaviour
{
    public MainMenu mainMenu; 

    public void EnableButtonsEvent()
    {
        mainMenu.EnableButtons();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public Animator cameraAnimator;
    public Animator doorAnimator;
    private Button[] menuButtons;

 void Awake()
    {
        // Get all buttons in the main menu
        menuButtons = mainMenuUI.GetComponentsInChildren<Button>();
    }
    public void PlayGame()
{
    // Disable all buttons so they can't be clicked again
    foreach (var btn in menuButtons)
    {
        btn.interactable = false;
    }

    // Trigger animations
    cameraAnimator.SetTrigger("Play");

    if (doorAnimator != null)
    {
        doorAnimator.SetTrigger("Open");
    }

}

    public void Option()
    {
        cameraAnimator.SetTrigger("Option");
    }

    public void YesOrNo()
    {
        cameraAnimator.SetTrigger("Quit");
    }

    public void Back()
    {
        cameraAnimator.SetTrigger("Back");
        cameraAnimator.SetTrigger("Idle");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
    
}
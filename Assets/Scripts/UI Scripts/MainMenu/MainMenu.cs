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
        // Get all buttons
        menuButtons = mainMenuUI.GetComponentsInChildren<Button>();
    }

    void SetButtonsInteractable(bool state)
    {
        foreach (var btn in menuButtons)
        {
            btn.interactable = state;
        }
    }
     public void EnableButtons()
    {
        SetButtonsInteractable(true);
    }
    public void PlayGame()
    {
    // Disable all buttons
     SetButtonsInteractable(false);

    // Trigger animations
    cameraAnimator.SetTrigger("Play");

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }

}
    public void Option()
    {
        SetButtonsInteractable(false);
        cameraAnimator.SetTrigger("Option");
    }

    public void YesOrNo()
    {
        SetButtonsInteractable(false);
        cameraAnimator.SetTrigger("Quit");
    }

    public void Back()
    {
        SetButtonsInteractable(false);
        cameraAnimator.SetTrigger("Back");
        cameraAnimator.SetTrigger("Idle");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
    
}
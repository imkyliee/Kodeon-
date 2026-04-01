using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnimator;
    public float transitionTime;

    private bool hasTransitioned = false;

    public void OnButtonPressed()
    {
        if (!hasTransitioned)
        {
            hasTransitioned = true;
            Transition();
        }
    }

    public void Transition()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);

        Time.timeScale = 1f; // Ensure time scale is reset before loading the scene

        SceneManager.LoadScene("Main Menu");
    }
}
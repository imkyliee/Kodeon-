using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnimator;
    public float transitionTime;

    public void OnButtonPressed(string scene)
    {
        StartCoroutine(LoadScene(scene));
    }

    IEnumerator LoadScene(string scene)
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);

        SceneManager.LoadScene(scene);
    }
}
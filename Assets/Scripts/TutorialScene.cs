using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         Time.timeScale = 1f;
    }

      public void Tutorial()
    {
     SceneManager.LoadScene("Tutorial");
    }
}

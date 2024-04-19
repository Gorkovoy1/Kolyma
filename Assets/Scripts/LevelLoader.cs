using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public bool nextLevel = true;

    void Update()
    {
        if(nextLevel)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        //or (SceneManager.LoadScene(""))
        //to give time for animation to play, ie delay code, use coroutine
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play animation
        transition.SetTrigger("Start");
        

        //Wait
        yield return new WaitForSeconds(transitionTime);
        //pauses coroutine for x amount of seconds
        
        //Load scene
        SceneManager.LoadScene(levelIndex);

        
    }
        
}


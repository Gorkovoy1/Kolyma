using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMenu()
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(0);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        StartCoroutine(SceneLoader.instance.LoadNextScene(sceneName));
    }
}

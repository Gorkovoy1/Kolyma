using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public bool triggerLoad = false;
    public static SceneLoader instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(triggerLoad)
        {
            triggerLoad = false;
            //fade out music first
            StartCoroutine(LoadNextScene(sceneName));
        }

    }

    IEnumerator LoadNextScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName); 
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //progressBar.value = progress;
            //loadingText.text = $"Loading...{Mathf.Floor(progress * 100)}%";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    /*
    IEnumerator FadeOutMusic(float duration)
    {
        float t = 0;

        while (t < duration)
        {
            float v = Mathf.Lerp(100, 0, t / duration);
            AKSoundEngine.SetRTPCValue("MusicVolume", v);

            t += Time.deltaTime;
            yield return null;
        }

        AKSoundEngine.SetRTPCValue("MusicVolume", 0);
    }
    */
}

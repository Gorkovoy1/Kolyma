using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{

    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float loadDelay = 2f;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNextScene(sceneName));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        yield return new WaitForSeconds(loadDelay);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName); //replace with scene number
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            loadingText.text = $"Loading...{Mathf.Floor(progress * 100)}%";

            if(operation.progress >= 0.9f)
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

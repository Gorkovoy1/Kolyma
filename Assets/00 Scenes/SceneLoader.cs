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
    public Image blocker;
    public bool isLoading = false;
    [SerializeField] private float fadeDuration = 0.75f;

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

    public IEnumerator LoadNextScene(string sceneName)
    {
        sceneName = sceneName;

        StartCoroutine(RTPCFader.instance.FadeOut());
        StartCoroutine(SceneEndLoad());

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName); 
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f && RTPCFader.instance.done)
            {
                operation.allowSceneActivation = true;
                StartCoroutine(RTPCFader.instance.FadeIn());
                StartCoroutine(SceneStartLoad());
            }

            yield return null;
        }
    }

    public IEnumerator SceneEndLoad()
    {
        isLoading = true;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        float timer = 0f;
        Color color = blocker.color;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            blocker.color = color;
            yield return null;
        }

        color.a = 1f;
        blocker.color = color;
    }

    public IEnumerator SceneStartLoad()
    {
        float timer = 0f;
        Color color = blocker.color;
        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            blocker.color = color;

            yield return null;
        }

        color.a = 0f;
        blocker.color = color;

        isLoading = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    
}

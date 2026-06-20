using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class HighlightObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    Material normal;
    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;

    public Animator transition;
    public float transitionTime = 1f;

    public int levelIndex;

    public bool isTableScene;

    
    public bool isFlashing = false;

    public Material mat;
    public string outlinePropertyName = "_ImageOutline";
    public float elapsedTime = 0f;
    public float fadeDuration = 1f;

    public bool onGlow = false;


    void OnEnable()
    {
        isFlashing = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        normal = GetComponent<Image>().material;
        mat = Resources.Load<Material>("MapGlow");

    }

    void Update()
    {
        if (!isFlashing && !onGlow)
        {
            //do nothing
            GetComponent<Image>().material = normal;
        }
        else if (isFlashing && !onGlow)
        {
            GetComponent<Image>().material = mat;
        }
        else if (onGlow)
        {
            GetComponent<Image>().material = Resources.Load<Material>("Glow");
        }

        if (GetComponent<Image>().material == mat)
        {
            // Increases over time
            elapsedTime += Time.deltaTime;

            // PingPong goes 0→1→0 smoothly
            float t = Mathf.PingPong(elapsedTime / fadeDuration, 1f);

            SetAlpha(t);
        }

    }

    public void SetAlpha(float alpha)
    {
        Color c = mat.GetColor(outlinePropertyName);

        c.a = alpha;

        mat.SetColor(outlinePropertyName, c);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        onGlow = true;
        GetComponent<Image>().material = Resources.Load<Material>("GlowYellow");
    }
   

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tempRaycaster);
        Destroy(tempCanvas);
        onGlow = false;

        GetComponent<Image>().material = normal;

    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        isFlashing = false;

        if (!isTableScene)
        {
            Debug.Log("go to scene");

            StartCoroutine(LoadLevel(levelIndex));
        }
        else
        {
            //show scene with betting window n stuff
            //SceneManager.LoadScene("4 InvBet", LoadSceneMode.Additive);

            //when using inv persistent, show inv canvas and set mode to bet mode
            UIPanelManager.instance.SetState(UIState.Bet);
        }
        
        
    }


    IEnumerator LoadLevel(int levelIndex)
    {
        /*
        //Play animation
        transition.SetTrigger("Start");


        //Wait
        yield return new WaitForSeconds(transitionTime);
        //pauses coroutine for x amount of seconds

        AkSoundEngine.StopAll();
        //Load scene
        SceneManager.LoadScene(levelIndex);
        */

        string scenePath = SceneUtility.GetScenePathByBuildIndex(levelIndex);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        StartCoroutine(SceneLoader.instance.LoadNextScene(sceneName));

        yield return null;

    }

}


    
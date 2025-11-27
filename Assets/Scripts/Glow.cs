using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Glow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    Material normal;
    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;

    public Animator transition;
    public float transitionTime = 1f;

    public string buildingName = " ";
    public int buildingNumber;

    public bool isFlashing = false;

    [SerializeField] private ConfirmationWindow myConfirmationWindow;

    public Material mat;
    public string outlinePropertyName = "_ImageOutline";
    public float elapsedTime = 0f;
    public float fadeDuration = 1f;

    public bool onGlow = false;

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
        else if(isFlashing && !onGlow)
        {
            GetComponent<Image>().material = mat;
        }
        else if(onGlow)
        {
            GetComponent<Image>().material = Resources.Load<Material>("Glow");
        }

        if(GetComponent<Image>().material == mat)
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
        Debug.Log("glow");
        onGlow = true;

        //tempCanvas = gameObject.AddComponent<Canvas>();
        //tempCanvas.overrideSorting = false;
        //tempCanvas.sortingOrder = 1;
        //tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        //GetComponent<Image>().material = Resources.Load<Material>("Glow");

    }
   

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tempRaycaster);
        Destroy(tempCanvas);
        onGlow = false;
        GetComponent<Image>().material = normal;
        Debug.Log("stop glow");
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("go to scene");
        
        //spawn GUI "go to _____?"
        //on no, do nothing
        //on yes, load scene
        OpenConfirmationWindow("Travel to " + buildingName + "?");

        
    }

    public void OpenConfirmationWindow(string message)
    {
        myConfirmationWindow.gameObject.SetActive(true);
        myConfirmationWindow.yesButton.onClick.AddListener(YesClicked);
        myConfirmationWindow.noButton.onClick.AddListener(NoClicked);
        myConfirmationWindow.elsewhereButton.onClick.AddListener(NoClicked);
        myConfirmationWindow.messageText.text = message;
    }

    private void YesClicked()
    {
        Debug.Log("Yes");
        myConfirmationWindow.gameObject.SetActive(false);
        StartCoroutine(LoadLevel(buildingNumber));
    }

    private void NoClicked()
    {
        Debug.Log("No");
        myConfirmationWindow.gameObject.SetActive(false);

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

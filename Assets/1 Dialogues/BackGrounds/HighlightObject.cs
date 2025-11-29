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


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        normal = GetComponent<Image>().material;
        
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().material = Resources.Load<Material>("GlowYellow");
    }
   

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tempRaycaster);
        Destroy(tempCanvas);

        GetComponent<Image>().material = normal;
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("go to scene");
        
        StartCoroutine(LoadLevel(levelIndex));
        
    }


    IEnumerator LoadLevel(int levelIndex)
    {
        //Play animation
        transition.SetTrigger("Start");


        //Wait
        yield return new WaitForSeconds(transitionTime);
        //pauses coroutine for x amount of seconds

        AkSoundEngine.StopAll();
        //Load scene
        SceneManager.LoadScene(levelIndex);

    }
    
}

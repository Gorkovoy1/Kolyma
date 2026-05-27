using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class BattleEndController : MonoBehaviour
{
    public Sprite victory;
    public Sprite defeat;

    public bool startBattleEnd = false;

    public Image image;
    public float fadeTime;
    private float elapsed = 0f;
    public bool isFading = false;
    public GameObject sfxObj;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startBattleEnd)
        {
            startBattleEnd = false;
            StartFade();
        }

        if (isFading)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeTime);
            Color color = image.color;
            color.a = alpha;
            image.color = color;

            if (alpha >= 1f)
            {
                isFading = false; // Fade finished
                StartCoroutine(DelayToNextScene());
            }
        }

    }

    IEnumerator DelayToNextScene()
    {
        yield return new WaitForSeconds(1.5f);

        if(image.sprite == victory)
        {
            UIPanelManager.instance.SetState(UIState.Winnings);
            InventoryManager.instance.showWinnings = true;

            if(InventoryManager.instance != null)
            {
                yield return new WaitUntil(() => InventoryManager.instance.winningsPanel.childCount == 0);
            }
            else
            {
                string path = SceneUtility.GetScenePathByBuildIndex(0);

                string sceneName = Path.GetFileNameWithoutExtension(path);

                SceneLoader.instance.sceneName = sceneName;
                SceneLoader.instance.triggerLoad = true;
            }
                
        }
        

        if (SceneManager.GetActiveScene().buildIndex == 12 || SceneManager.GetActiveScene().buildIndex == 0)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(0);

            string sceneName = Path.GetFileNameWithoutExtension(path);

            SceneLoader.instance.sceneName = sceneName;
            SceneLoader.instance.triggerLoad = true;
        }
        else
        {
            string path = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);

            string sceneName = Path.GetFileNameWithoutExtension(path);

            SceneLoader.instance.sceneName = sceneName;
            SceneLoader.instance.triggerLoad = true;
            UIPanelManager.instance.SetState(UIState.Minimized);
        }
            
    }

    public void StartFade()
    {
        //figure out if win or lose
        if(NumberManager.instance.playerVal <= NumberManager.instance.targetVal && NumberManager.instance.oppVal > NumberManager.instance.targetVal && NumberManager.instance.playerVal >= 0)
        {
            image.sprite = victory;

            AkSoundEngine.StopAll();

            AkSoundEngine.PostEvent("Play_Battle_Won", sfxObj);
            

        }
        else if(NumberManager.instance.playerVal <= NumberManager.instance.targetVal && NumberManager.instance.oppVal <= NumberManager.instance.targetVal && Mathf.Abs(NumberManager.instance.targetVal - NumberManager.instance.playerVal) < Mathf.Abs(NumberManager.instance.targetVal - NumberManager.instance.oppVal))
        {
            image.sprite = victory;

            AkSoundEngine.StopAll();

            AkSoundEngine.PostEvent("Play_Battle_Won", sfxObj);
        }
        else
        {
            image.sprite = defeat;

            AkSoundEngine.StopAll();

            AkSoundEngine.PostEvent("Play_Battle_Lost", sfxObj);
            //tie or lose
        }

        elapsed = 0f;
        isFading = true;
    }
}

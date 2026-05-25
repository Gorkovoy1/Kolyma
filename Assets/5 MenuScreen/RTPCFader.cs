using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class RTPCFader : MonoBehaviour
{
    public float currentValue = 100f;
    public static RTPCFader instance;
    public bool done = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

   // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.SetRTPCValue("MusicVolume", currentValue);
        AkSoundEngine.SetRTPCValue("SFXVolume", currentValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FadeOut()
    {
        done = false;
        float duration = 1f;
        float time = 0f;

        float startValue = currentValue;
        float endValue = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            currentValue = Mathf.Lerp(
                startValue,
                endValue,
                time / duration
            );

            AkSoundEngine.SetRTPCValue("MusicVolume", currentValue);
            AkSoundEngine.SetRTPCValue("SFXVolume", currentValue);

            yield return null;
        }

        AkSoundEngine.SetRTPCValue("MusicVolume", 0f);
        AkSoundEngine.SetRTPCValue("SFXVolume", 0f);
        done = true;
        AkSoundEngine.StopAll();
    }

    public IEnumerator FadeIn()
    {
        float duration = 2f;
        float time = 0f;

        float startValue = currentValue;
        float endValue = 100f;

        while (time < duration)
        {
            time += Time.deltaTime;

            currentValue = Mathf.Lerp(
                startValue,
                endValue,
                time / duration
            );

            AkSoundEngine.SetRTPCValue("MusicVolume", currentValue);
            AkSoundEngine.SetRTPCValue("SFXVolume", currentValue);

            yield return null;
        }

        AkSoundEngine.SetRTPCValue("MusicVolume", 100f);
        AkSoundEngine.SetRTPCValue("SFXVolume", 100f);
    }
}

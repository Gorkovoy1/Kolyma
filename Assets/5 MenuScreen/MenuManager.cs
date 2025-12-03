using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    // Start is called before the first frame update
    void Start()
    {
        mainMenu.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume)*20);
    }

    public void SetMaster(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);
        Debug.Log(volume);
    }

    public void SetSFX(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume)*20);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToBattle()
    {
        AkSoundEngine.StopAll();
        SceneManager.LoadScene("10 BattleSceneJ");
    }

    public void DisplayOptions()
    {
        mainMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenu.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        AkSoundEngine.StopAll();
        SceneManager.LoadScene("3 DIALOGUE1");
    }

    
}

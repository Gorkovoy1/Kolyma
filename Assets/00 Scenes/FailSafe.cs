using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailSafe : MonoBehaviour
{
    public static FailSafe instance;


    void Awake()
    {
        if(instance == null)
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
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha0))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha1))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(3); //dialogue 1
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha2))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(4);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha3))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(6);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha4))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(7);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha5))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(9);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha6))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(11);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha7))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(5);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha8))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(8);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Alpha9))
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(10);
        }
    }
}

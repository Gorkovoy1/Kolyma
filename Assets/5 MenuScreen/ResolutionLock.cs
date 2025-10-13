using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionLock : MonoBehaviour
{
    void Awake()
    {
        int targetWidth = 1920;
        int targetHeight = 1080;
        bool fullscreen = true;

        Screen.SetResolution(targetWidth, targetHeight, fullscreen);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

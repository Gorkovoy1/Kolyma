using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayRate : MonoBehaviour
{
    public VisualEffect morningFog;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.185f;
    }

    // Update is called once per frame
    void Update()
    {
        morningFog.playRate = speed;
    }
}

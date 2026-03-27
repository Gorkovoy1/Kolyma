using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDiceTop : MonoBehaviour
{
    public int topVal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        topVal = GetTopFace();
    }

    public int GetTopFace()
    {
        Vector3 up = Vector3.up;
        float maxDot = -1f;
        int topFace = -1;

        // Define each face direction and their associated number
        (Vector3 direction, int faceNumber)[] faceDirs = new[]
        {
            (transform.up, 3),      // Top face
            (-transform.up, 4),     // Bottom face
            (transform.forward, 1), // Front
            (-transform.forward, 6),// Back
            (transform.right, 5),   // Right
            (-transform.right, 2),  // Left
        };

        foreach (var (dir, face) in faceDirs)
        {
            float dot = Vector3.Dot(dir, up);
            if (dot > maxDot)
            {
                maxDot = dot;
                topFace = face;
            }
        }
        //Debug.Log(topFace);
        return topFace;
    }
}

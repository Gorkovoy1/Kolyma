using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public Rigidbody diceRigidbody;
    public float throwForce;
    public float spinForce;
    public Vector3 startPos;
    public float extraGravityMultiplier;

    public float velocityThreshold = 0.01f;
    public float angularVelocityThreshold = 0.01f;

    public bool isFrozen = false;

    public int topVal;

    // Start is called before the first frame update
    void Start()
    {
        diceRigidbody = this.GetComponent<Rigidbody>();
        diceRigidbody.sleepThreshold = 0.1f;
        startPos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        diceRigidbody.AddForce(Physics.gravity * extraGravityMultiplier);
        topVal = GetTopFace();

        /*
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Top face: " + GetTopFace());
            topVal = GetTopFace();
        }
        

        

        if (Input.GetKeyUp(KeyCode.W))
        {
            Debug.Log("W");
            isFrozen = false;
            FlingDice();
            //StartCoroutine(MakeFrozen());
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            this.gameObject.transform.position = startPos;
        }
        
        if(diceRigidbody.velocity.magnitude > velocityThreshold &&
                diceRigidbody.angularVelocity.magnitude > angularVelocityThreshold)
        {
            isFrozen = true;
        }


        if (isFrozen &&
                diceRigidbody.velocity.magnitude < velocityThreshold &&
                diceRigidbody.angularVelocity.magnitude < angularVelocityThreshold)
        {
            // Freeze when nearly stopped
            diceRigidbody.velocity = Vector3.zero;
            diceRigidbody.angularVelocity = Vector3.zero;
            diceRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        else if (!isFrozen)
        {
            
            diceRigidbody.constraints = RigidbodyConstraints.None;
        }
        */
    }

    void FlingDice()
    {
        // Reset existing movement
        diceRigidbody.velocity = Vector3.zero;
        diceRigidbody.angularVelocity = Vector3.zero;

        // Randomized forward force
        Vector3 forceDirection = transform.forward * Random.Range(0.8f, 1.2f) +
                                 transform.up * Random.Range(1f, 2f) +
                                 transform.right * Random.Range(-0.5f, 0.5f);
        diceRigidbody.AddForce(forceDirection.normalized * throwForce, ForceMode.Impulse);

        // Random torque for natural roll/spin
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * spinForce; // No normalization here
        diceRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
    }

    IEnumerator MakeFrozen()
    {
        yield return new WaitForSeconds(1f);
        isFrozen = true;
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
        Debug.Log(topFace);
        return topFace;
    }

}

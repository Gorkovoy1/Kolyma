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


        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W");
            FlingDice();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            this.gameObject.transform.position = startPos;
        }

       
    }

    void FlingDice()
    {
        // Reset existing movement
        diceRigidbody.velocity = Vector3.zero;
        diceRigidbody.angularVelocity = Vector3.zero;

        // Forward-only force (relative to world or dice orientation)
        Vector3 forceDirection = transform.forward + transform.up; // Adjust if needed (e.g., transform.forward)
        forceDirection.Normalize();
        diceRigidbody.AddForce(forceDirection * throwForce, ForceMode.Impulse);

        // Random torque for natural roll/spin
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * spinForce;

        diceRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
    }
}

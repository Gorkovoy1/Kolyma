using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBoxManager : MonoBehaviour
{
    public GameObject dicePrefab;       // Your dice prefab
    public int diceCount = 4;
    public Vector3 boxCenter;
    public Vector3 boxSize = new Vector3(1f, 0.5f, 1f); // size of spawn area
    public float throwForce = 10f;
    public float spinForce = 10f;
    public Transform diceParent;

    private List<Rigidbody> diceBodies = new List<Rigidbody>();

    void Start()
    {
        SpawnDice();
    }

    void SpawnDice()
    {
        for (int i = 0; i < diceCount; i++)
        {
            Vector3 randomPos = boxCenter + new Vector3(
                Random.Range(-boxSize.x / 2, boxSize.x / 2),
                Random.Range(0.1f, boxSize.y),
                Random.Range(-boxSize.z / 2, boxSize.z / 2)
            );

            GameObject dice = Instantiate(dicePrefab, randomPos, Random.rotation, diceParent);
            Rigidbody rb = dice.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();

            diceBodies.Add(rb);
        }

        RollAllDice();
    }

    public void RollAllDice()
    {
        foreach (Rigidbody rb in diceBodies)
        {
            rb.WakeUp(); // make sure Rigidbody is active

            Vector3 forceDir = transform.up * Random.Range(0.8f, 1.2f) +
                                 transform.right * Random.Range(1f, 2f) +
                                 transform.forward * Random.Range(-0.5f, 0.5f);

            rb.AddForce(forceDir.normalized * throwForce, ForceMode.Impulse);

            Vector3 torque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * spinForce;

            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TutorialScripts
{
    

    public class TutorialDice : MonoBehaviour
    {
        public GameObject dicePrefab;       // Your dice prefab
        public int diceCount = 4;
        public Vector3 boxCenter;
        public Vector3 boxSize = new Vector3(1f, 0.5f, 1f); // size of spawn area
        public float throwForce = 10f;
        public float spinForce = 10f;
        public Transform diceParent;
        public Image blackImage;

        public TextMeshProUGUI target;
        public GameObject table;

        private List<Rigidbody> diceBodies = new List<Rigidbody>();

        public float fadeTime;
        public bool fadingIn = false;
        public bool fadingOut = false;
        private float elapsed;

        public GameObject[] diceList;

        void Start()
        {
            //deactivate everything but the black
            target.gameObject.SetActive(false);
            table.SetActive(false);

            StartCoroutine(FadeInDice());

        }

        void Update()
        {
            if (fadingIn)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / fadeTime);
                Color color = blackImage.color;
                color.a = alpha;
                blackImage.color = color;

                if (alpha >= 1f)
                {
                    fadingIn = false; // Fade finished
                }
            }
            else if (fadingOut)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(1f - (elapsed / fadeTime));
                Color color = blackImage.color;
                color.a = alpha;
                blackImage.color = color;

                if (alpha <= 0f)
                {
                    fadingOut = false; // Fade finished
                }
            }
        }

        IEnumerator FadeInDice()
        {
            // Start fading in
            elapsed = 0f;
            //fadingIn = true;
            //yield return new WaitForSeconds(fadeTime); // wait for fade in to finish

            // Reactivate everything
            yield return new WaitForSeconds(1f);

            table.SetActive(true);

            // Fade out
            elapsed = 0f;
            fadingOut = true;
            yield return new WaitForSeconds(fadeTime / 2); // wait for fade out to finish

            // Roll the dice
            SpawnDice();
            target.gameObject.SetActive(true);

            yield return new WaitForSeconds(2.5f);

            elapsed = 0f;
            fadingIn = true;
            yield return new WaitForSeconds(1f);
        }

        IEnumerator FadeDiceAlpha(float startAlpha, float endAlpha, float fadeTime)
        {
            float t = 0f; // normalized time from 0 to 1
            Color color = blackImage.color;
            color.a = startAlpha;
            blackImage.color = color;

            // Instead of while loop, use a for loop with frame-based increment
            for (; t < 1f; t += Time.deltaTime / fadeTime)
            {
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                blackImage.color = color;
                yield return null; // wait for next frame
            }

            // Ensure final alpha
            color.a = endAlpha;
            blackImage.color = color;
        }



        void SpawnDice()
        {
            
            foreach(GameObject g in diceList)
            {
                g.SetActive(true);
            }

            //RollAllDice();
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
}



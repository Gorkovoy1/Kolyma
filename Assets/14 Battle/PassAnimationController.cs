using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassAnimationController : MonoBehaviour
{
    public float duration;
    public Vector3 startPos;
    public Vector3 midPos;
    public Vector3 endPos;
    public RectTransform oppTextRect;
    public RectTransform playerTextRect;

    public bool playerPass = false;
    public bool oppPass = false;

    private RectTransform textRect;

    public bool playerLastTurnPassed = false;
    public bool oppLastTurnPassed = false;

    public BattleEndController battleEndController;

    public bool triggerGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerPass)
        {
            playerPass = false;
            playerLastTurnPassed = true;
            midPos = new Vector3(midPos.x, -115f, midPos.z);
            endPos = new Vector3(endPos.x, -115f, endPos.z);
            startPos = new Vector3(0f, -115f, startPos.z);
            StartCoroutine(AnimatePassText(playerTextRect));
        }
        else if(oppPass)
        {
            oppPass = false;
            oppLastTurnPassed = true;
            midPos = new Vector3(midPos.x, 115f, midPos.z);
            endPos = new Vector3(endPos.x, 115f, endPos.z);
            startPos = new Vector3(0f, 115f, startPos.z);
            StartCoroutine(AnimatePassText(oppTextRect));
        }

        //everyt ime a card is played set lastturnpassed to false
        //if both last turns are true, game over and calculate who wins
        if(playerLastTurnPassed && oppLastTurnPassed && !triggerGameOver)
        {
            triggerGameOver = true;
            //toggle game over
            battleEndController.startBattleEnd = true;
        }
    }

    IEnumerator AnimatePassText(RectTransform textRect)
    {
        yield return LerpPosition(textRect, startPos, midPos, duration);

        yield return new WaitForSeconds(0.5f);

        yield return LerpPosition(textRect, midPos, endPos, duration);
    }

    IEnumerator LerpPosition(RectTransform textRect, Vector3 start, Vector3 end, float time)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / time)
        {
            textRect.anchoredPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }
        textRect.anchoredPosition = end;
    }
}

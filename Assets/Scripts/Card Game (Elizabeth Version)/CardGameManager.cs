using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardGameManager : MonoBehaviour
{

    public enum State {
        INIT, //tasks to complete ONCE as the scene loads in, before anything starts. Use for technical behind the scenes stuff, eg loading art and sound
        STARTGAME, //tasks to complete ONCE at the start of the game, after INIT. eg offering the choice to double bet
        STARTROUND, //tasks to complete at the start of each round of gameplay, eg dealing cards.
        PLAYERTURN, //the player can take their turn and play cards here
        OPPONENTTURN, /*opponent AI takes their turn and plays cards. Will access the AI (NYI) and any particulars for this opponent via the opponent scriptableobject.
        The opponents AI will need to be coded too- look into Behavior Trees maybe?- and the AI will dictate an action, pass the action to this manager to be performed, animated, and completed.
        Once this manager completes the designated action, pass back to opponent AI to determine next move*/
        ENDTURN, //tasks to complete at the end of a turn. eg draws a card for current turn, then flips to other person's turn. Could perhaps reorg this dep. on needs- states for OPPONENTEND and PLAYEREND maybe?
        ENDROUND, //tasks to complete at the end of each round. eg check round winner, add score, end game if there is a winner or start next round if not.
        ENDGAME, //tasks to complete ONCE when the game is over. eg cleanup vars from gameplay, record winner, transfer out of card game scene
        PAUSED //player has paused the game- this'll mean something when there's a pause menu. enter and exit this state to basically pause the game.
    };

    [Header("Card Game Info")]
    //This class manages the card game. It is designed to attach to the card game scene and load and run one card game from start to finish.

    public int roundCount = 0; //defaults to round 0 whenever a new game is started

    public CardGameCharacter opponent; //scriptable object class containing opponent data
    public CardGameCharacter player; //scriptable object class containing player data

    public List<NumberCard> numberDeck; //the numbers deck- assuming this is communal?
    private List<SpecialDeckCard> playerDeck, opponentDeck, playerHand, opponentHand; //list for storing current deck and hand states

    public bool isStoryBattle = false; //is this a one-round story battle or not? pass this var in from outside when triggering the battle. if not set defaults to normal 3 round battle.

    private int opponentPoints, playerPoints = 0; //rounds won score for each person

    public int bet = 5; //amt bet on game

    private int targetValue; //target value to win a round
    private int opponentCurrValue, playerCurrValue = 0; //current progress towards target value.

    private bool roundOver = false; //becomes true when someone meets or exceeds target value.
    public State state; //current state
    private State prevState; //record which state we were in before we paused so we can go back to it


    [Header("UI References")] //references to all the UI elements in the scene

    public Transform panelTransform;
    public Transform playerHandTransform;
    public Transform opponentHandTransform;
    public TextMeshProUGUI opponentSumText;
    public TextMeshProUGUI playerSumText;
    public TextMeshProUGUI targetValueText;
    public GameObject discardPile;

    // Awake called before Start as soon as loaded into scene
    void Awake() {
        state = State.INIT;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case State.INIT:
                //loading tasks go here
                opponentDeck = opponent.deckList;
                playerDeck = player.deckList;
                state = State.STARTGAME;
                break;

            case State.STARTGAME:
                /*NYI
                display option to double bet
                if(betDoubled) {
                    bet = 10;
                }*/
                state = State.STARTROUND;
                break;

            case State.STARTROUND:
                playerCurrValue = 0;
                opponentCurrValue = 0;
                DrawCards(opponent, 4, 6);
                DrawCards(player, 4, 6);
                roundCount += 1;

                //roll dice - two for each player
                //if player dice higher, state = State.PLAYERTURN;
                //if opponent dice higher, state = State.OPPONENTTURN;
                int playerTotal = 0;
                int opponentTotal = 0;
                while(playerTotal == opponentTotal) {
                    playerTotal = Random.Range(1, 7) + Random.Range(1, 7);
                    opponentTotal = Random.Range(1,7) + Random.Range(1, 7);
                }

                if(opponentTotal > playerTotal) {
                    state = State.OPPONENTTURN;
                }
                else {
                    state = State.PLAYERTURN;
                }
                break;

            case State.PLAYERTURN:
                /*NYI
                Take input and perform actions for player turn. swap to end turn when clicking end turn button.
                */
                //player plays one special card OR uses their action AND may swap out one of their cards from their hand - they can also pass
                Debug.Log("Player turn");
                prevState = State.PLAYERTURN; //leveraging this so ENDTURN knows whose turn ended. if this gets messy we can make separate ENDTURN states.
                state = State.ENDTURN;
                break;

            case State.OPPONENTTURN:
                //use AI to choose best action - sift through if opp closer or player closer, if better to attack or defend
                Debug.Log("opponent turn");
                prevState = State.OPPONENTTURN;
                state = State.ENDTURN;
                break;

            case State.ENDTURN:
                if(playerCurrValue >= targetValue || opponentCurrValue >= targetValue) {
                    state = State.ENDROUND;
                }
                if(prevState == State.PLAYERTURN) {
                    DrawCards(player, 0, 1); 
                    state = State.OPPONENTTURN;
                }
                else{
                    DrawCards(opponent, 0, 1);
                    state = State.PLAYERTURN;
                }
                break;

            case State.ENDROUND:
                Debug.Log("End round");
                if(playerCurrValue >= targetValue) {
                    playerPoints += 1;
                }
                else{
                    opponentPoints += 1;
                }

                if(playerPoints >= 2 || opponentPoints >= 2 || roundCount >= 3) {
                    state = State.ENDGAME;
                }
                else{
                    /*NYI
                    remove leftover cards from hands
                    */
                    Debug.Log("New Round");
                    state = State.STARTROUND;
                }
                break;

            case State.ENDGAME:
                Debug.Log("Game over!");
                if(playerPoints >= 2) {
                    Debug.Log("winner is player!");
                }
                else{
                    Debug.Log("winner is opponent!");
                }
                /*NYI
                transfer out of card game scene and record winner of match in game data */
                break;

            case State.PAUSED:
                /*NYI
                if(exit pause menu) {
                    state = prevState;
                }*/
                break;
        }
    }

    void DrawCards(CardGameCharacter target, int numberCards, int specialCards) {
        /*NYI
        draw specified number of cards from each deck and put it in target's hand */
        return;
    }

    void ShuffleCards(List<GenericCard> shuffle) {


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        SELECTCARDS, //selection mode to pick cards to swap or discard, etc etc
        ENDTURN, //tasks to complete at the end of a turn. eg draws a card for current turn, then flips to other person's turn. Could perhaps reorg this dep. on needs- states for OPPONENTEND and PLAYEREND maybe?
        ENDROUND, //tasks to complete at the end of each round. eg check round winner, add score, end game if there is a winner or start next round if not.
        ENDGAME, //tasks to complete ONCE when the game is over. eg cleanup vars from gameplay, record winner, transfer out of card game scene
        PAUSED //player has paused the game- this'll mean something when there's a pause menu. enter and exit this state to basically pause the game.
    };

   private struct CardSelectSettings{
        public readonly int numCards {get;}
        public readonly SpecialKeyword cardType{get;}
        public readonly SpecialKeyword selectionPurpose{get;}
        public readonly bool setupPlayerUI{get;}

        public CardSelectSettings(int n, SpecialKeyword c, SpecialKeyword s, bool p){
            numCards = n;
            cardType = c;
            selectionPurpose = s;
            setupPlayerUI = p;
        }
    }

    [Header("Card Game Info")]
    //This class manages the card game. It is designed to attach to the card game scene and load and run one card game from start to finish.

    public int roundCount = 0; //defaults to round 0 whenever a new game is started

    public CardGameCharacter opponent; //scriptable object class containing opponent data
    public CardGameCharacter player; //scriptable object class containing player data

    public List<NumberCard> numberDeck; //the numbers deck- assuming this is communal?

    public bool isStoryBattle = false; //is this a one-round story battle or not? pass this var in from outside when triggering the battle. if not set defaults to normal 3 round battle.

    private int opponentPoints, playerPoints = 0; //rounds won score for each person

    public int bet = 5; //amt bet on game

    private int targetValue; //target value to win a round
   
    private bool roundOver = false; //becomes true when someone meets or exceeds target value.
    public State state; //current state
    private State prevState; //record which state we were in before we paused so we can go back to it
    private bool opponentEndRound, playerEndRound;

    private List<GenericCard> discardPile = new List<GenericCard>();

    [HideInInspector] public List<DisplayCard> activeCardVisuals;

    private bool printed = false;
    private bool selectionConfirmation = false;
    private bool enableSelectionConfirmButton = false;

    private List<GameObject> playerNegativeCards = new List<GameObject>();
    private List<GameObject> opponentNegativeCards = new List<GameObject>();
    private List<int> playerNegativeCardsValues = new List<int>();
    private List<int> opponentNegativeCardsValues= new List<int>();
    private bool selectCoroutineRunning = false;
    private Stack<CardSelectSettings> cardSelectStack = new Stack<CardSelectSettings>();

    [Header("UI References")] //references to all the UI elements in the scene
    public GameObject cardVisualPrefab;
    public GameObject endTurnButton, selectConfirmButton;
    public Transform panelTransform;
    public Transform playerHandTransform;
    public Transform opponentHandTransform;
    public TextMeshProUGUI opponentSumText, playerSumText, targetValueText, selectionAmountText;
    public Transform opponentNumberZone;
    public Transform playerNumberZone;
    public Toggle playerRoundToggle;
    public Toggle opponentRoundToggle;
    public Transform board;
    public int offset;
    private float playerPos = 0f;
    private float negPos;
    private float opponentPos = 0f;
    private float opponentNegPos;
    private EventSystem eventSystem;


    public int offsetInteger;
    public int scaleInteger; 
    public float scaleNumber;

    // Awake called before Start as soon as loaded into scene
    void Awake() {
        state = State.INIT;
    }

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        

    }

    // Update is called once per frame
    void Update()
    {
        //Update curr value text and such every frame because its easier to just do it here
        opponentSumText.text = opponent.name + " CURR VALUE: " + opponent.currValue;
        playerSumText.text = player.name + " CURR VALUE: " + player.currValue;

        //update selection confirmation button
        selectConfirmButton.SetActive(enableSelectionConfirmButton); 

        //check if we need to be selecting cards because a selection event is queued
        if(cardSelectStack.Count > 0 && state != State.SELECTCARDS) {
            prevState = state;
            state = State.SELECTCARDS;
        }

        //Ye Olde Turn Manager State Machine
        switch (state) {
            case State.INIT:
                //loading tasks go here
                player.FlushGameplayVariables();
                opponent.FlushGameplayVariables();
                opponentRoundToggle.isOn = false;
                playerRoundToggle.isOn = false;
                state = State.STARTGAME;
                break;

            case State.STARTGAME:
                /*NYI
                display option to double bet
                if(betDoubled) {
                    bet = 10;
                }*/
                foreach(SpecialDeckCard card in player.deckList) {
                    player.deck.Add(card);
                }
                foreach(SpecialDeckCard card in opponent.deckList) {
                    opponent.deck.Add(card);
                }
                state = State.STARTROUND;
                break;

            case State.STARTROUND:
                playerEndRound = false;
                opponentEndRound = false;
                discardPile.Clear();
                ShuffleCards(player.deck);
                ShuffleCards(opponent.deck);
                ShuffleCards(numberDeck);
                DrawSpecialCards(opponent, 6);
                DrawSpecialCards(player,6);
                DrawNumberCards(opponent, 4);
                DrawNumberCards(player, 4);
                roundCount += 1;
                targetValue = Random.Range(1,7) + Random.Range(1,7) + Random.Range(1,7) + Random.Range(1,7);
                targetValueText.text = "TARGET VALUE: " + targetValue;
                selectionConfirmation = false;
                state = State.PLAYERTURN;
                /*//roll dice - two for each player
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
                }*/
                break;

            case State.PLAYERTURN:
                /*NYI
                Take input and perform actions for player turn. swap to end turn when clicking end turn button.
                */
                //player plays one special card OR uses their action AND may swap out one of their cards from their hand - they can also pass
                playerEndRound = playerRoundToggle.isOn;
                opponentEndRound = opponentRoundToggle.isOn;
                break;

            case State.OPPONENTTURN:
                //use AI to choose best action - sift through if opp closer or player closer, if better to attack or defend
                ButtonEndTurn(); //AI NYI so just skip turn for now.
                break;

            case State.ENDTURN:
                if(playerEndRound && opponentEndRound) {
                    state = State.ENDROUND;
                }
                if(prevState == State.PLAYERTURN && !opponentEndRound) {
                    DrawSpecialCards(opponent, 1); 
                    state = State.OPPONENTTURN;
                    player.discardFlag = false;
                }
                else if(prevState == State.OPPONENTTURN && !playerEndRound){
                    DrawSpecialCards(player, 1);
                    state = State.PLAYERTURN;
                    opponent.discardFlag = false;
                }
                break;
            case State.SELECTCARDS:
                //this state mostly exists to lock the player out of performing unwanted actions and acknowledge to the rest of the system that the player is in the
                //middle of picking a card for some purpose.   
                if(!selectCoroutineRunning && cardSelectStack.Count > 0) {
                    CardSelectSettings curr = cardSelectStack.Pop();
                    StartCoroutine(SelectCard(curr.numCards, curr.cardType, curr.selectionPurpose, curr.setupPlayerUI));
                } 
                break;

            case State.ENDROUND:
                Debug.Log("End round");
                if(Mathf.Abs(targetValue - player.currValue) < Mathf.Abs(targetValue - opponent.currValue)) {
                    Debug.Log(player.name + " won this round");
                    playerPoints += 1;
                }
                else if(Mathf.Abs(targetValue - player.currValue) > Mathf.Abs(targetValue - opponent.currValue)) {
                    Debug.Log(opponent.name + " won this round");
                    opponentPoints += 1;
                }
                else {
                    Debug.Log("This round was a tie with no victor");
                }

                if(playerPoints >= 2 || opponentPoints >= 2 || roundCount >= 3) {
                    state = State.ENDGAME;
                }
                else{
                    playerPos = 0f;
                    opponentPos = 0f;
                    playerNegativeCardsValues.Clear();
                    opponentNegativeCardsValues.Clear();
                    playerNegativeCards.Clear();
                    opponentNegativeCards.Clear();
                    foreach(DisplayCard card in activeCardVisuals) {
                        if(card.baseCard is NumberCard) {
                            numberDeck.Add((NumberCard) card.baseCard);
                        }
                        Destroy(card.gameObject);
                    }
                    activeCardVisuals.Clear();
                    discardPile.Clear();
                    player.FlushGameplayVariables();
                    opponent.FlushGameplayVariables();
                    foreach(SpecialDeckCard card in player.deckList) {
                        player.deck.Add(card);
                    }
                    foreach(SpecialDeckCard card in opponent.deckList) {
                        opponent.deck.Add(card);
                    }
                    playerRoundToggle.isOn = false;
                    opponentRoundToggle.isOn = false;
                    Debug.Log("New Round");
                    state = State.STARTROUND;
                }
                break;

            case State.ENDGAME:
                if(!printed) {
                    Debug.Log("Game over!");
                    if(playerPoints >= 2) {
                        Debug.Log("winner is " + player.name);
                    }
                    else{
                        Debug.Log("winner is " + opponent.name);
                    }
                    printed = true;
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

    void DrawNumberCards(CardGameCharacter target, int numberCards) {
        for(int i = 0; i < numberCards; i++) {
            if(numberDeck.Count == 0) {
                Debug.Log("Number Deck is Empty");
                break;
            }
            NumberCard newCard = numberDeck[0];
            target.numberHand.Add(newCard);
            numberDeck.Remove(newCard);
            GameObject newCardVisual = Instantiate(cardVisualPrefab);
            DisplayCard newCardDisplay = newCardVisual.GetComponent<DisplayCard>();
            newCardDisplay.owner = target;
            newCardDisplay.baseCard = newCard;
            if(target == player) {
                PlaceCard(newCardVisual, target, newCard.value);
                player.currValue += newCard.value;
            }
            else {
                PlaceCard(newCardVisual, target, newCard.value);
                opponent.currValue += newCard.value;
            }
            activeCardVisuals.Add(newCardDisplay);
       } 
    }

    void PlaceCard(GameObject card, CardGameCharacter target, int value)
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        int screenWidth = Screen.width;

        int screenHeight = Screen.height;

        offset = (screenWidth/offsetInteger)*51/128;
        scaleNumber = (screenWidth/offsetInteger);

        if(target == player)
        {
            card.transform.SetParent(board);
            if(value>0)
            {
                card.transform.localScale = new Vector3 (3/scaleNumber, 3/scaleNumber, 3/scaleNumber);
                card.transform.localPosition = new Vector3(playerPos, -60, 0);
                playerPos = playerPos + value*offset;

            }
            else{
                playerNegativeCards.Add(card);
                playerNegativeCardsValues.Add(value);
            }
            
        } //-60 -170 150 40
        else
        {
            card.transform.SetParent(board);
            if(value>0)
            {
                Debug.Log(screenHeight);
                Debug.Log(screenWidth);
                card.transform.localScale = new Vector3 (3/scaleNumber, 3/scaleNumber, 3/scaleNumber);
                card.transform.localPosition = new Vector3(opponentPos, 150, 0);
                opponentPos = opponentPos + value*offset;
            }
            else{
                opponentNegativeCards.Add(card);
                opponentNegativeCardsValues.Add(value);
            }
        }

        negPos = playerPos - 10*offset;
        opponentNegPos = opponentPos - 10*offset;
        PlaceNegativeCard(playerNegativeCards, playerNegativeCardsValues, negPos, player);
        PlaceNegativeCard(opponentNegativeCards, opponentNegativeCardsValues, opponentNegPos, opponent);
        
    }

    void PlaceNegativeCard(List<GameObject> negativeCards, List<int> negativeCardsValues, float pos, CardGameCharacter target)
    {
        int x = 0;
        if(target == player)
        {
            x = -170;
        }
        else{
            x = 40;
        }
        for(int i = 0; i < negativeCards.Count; i++)
        {
            negativeCards[i].transform.localScale = new Vector3 (3/scaleNumber, 3/scaleNumber, 3/scaleNumber);
            negativeCards[i].transform.localPosition = new Vector3(pos, x, 0);
            pos = pos + negativeCardsValues[i]*offset;
        }
    }

    void DrawSpecialCards(CardGameCharacter target, int specialCards) {
        /*NYI
        draw specified number of cards from each deck and put it in target's hand */
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        int screenWidth = Screen.width;

        int screenHeight = Screen.height;

        float scaleFactor = Mathf.Min(screenWidth, screenHeight) * 0.0002f;

        for(int i = 0; i < specialCards; i ++) {
            if(target.deck.Count == 0) {
                Debug.Log(target.name + " is out of cards!");
                break;
            }
            SpecialDeckCard newCard = target.deck[0];
            target.hand.Add(newCard);
            target.deck.Remove(newCard);
            GameObject newCardVisual = Instantiate(cardVisualPrefab);
            DisplayCard newCardDisplay = newCardVisual.GetComponent<DisplayCard>();
            newCardDisplay.owner = target;
            newCardDisplay.baseCard = newCard;
            if(target == player) {
                
                newCardVisual.transform.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);
                newCardVisual.transform.SetParent(playerHandTransform);
            }
            else {
                newCardVisual.transform.localScale =  new Vector3 (scaleFactor, scaleFactor, scaleFactor);
                newCardVisual.transform.SetParent(opponentHandTransform);
            }
            activeCardVisuals.Add(newCardDisplay);
        }
    }

    //enter the select card state
    private IEnumerator SelectCard(int numCards, SpecialKeyword cardType, SpecialKeyword selectionPurpose, bool setupPlayerUI = true, List<DisplayCard> selectedCards = null) {
        selectCoroutineRunning = true;
        enableSelectionConfirmButton = false;
        if(selectedCards == null) {
            selectedCards = new List<DisplayCard>();
        }
        if(setupPlayerUI) {
            UIToggleSelectionMode(true);
        }
        else{
            UIToggleSelectionMode(false);
        }
        //check if need to select more cards than the player has, in which case just select everything they have.
        if(setupPlayerUI && cardType == SpecialKeyword.TYPE_SPECIAL && numCards > player.hand.Count) {
            numCards = player.hand.Count;
        }
        else if(setupPlayerUI && cardType == SpecialKeyword.TYPE_NUMBER && numCards > player.numberHand.Count) {
            numCards = player.numberHand.Count;
        }
        else if(!setupPlayerUI && cardType == SpecialKeyword.TYPE_NUMBER && numCards > opponent.numberHand.Count) {
            numCards = opponent.numberHand.Count;
        }
        else if(!setupPlayerUI && cardType == SpecialKeyword.TYPE_SPECIAL && numCards > opponent.hand.Count) {
            numCards = opponent.hand.Count;
        }

        selectionAmountText.text = "Select " + numCards + " total cards!";
        
        selectionConfirmation = false;
        while(selectedCards.Count < numCards || !selectionConfirmation && numCards > 0) {
            if(!setupPlayerUI) {
                enableSelectionConfirmButton = false;
                Debug.Log("AI for selecting cards NYI, using temp random selection");
                if(numCards >= opponent.hand.Count) {
                    foreach(DisplayCard card in activeCardVisuals) {
                        if(card.baseCard is SpecialDeckCard && card.owner == opponent) {
                            selectedCards.Add(card);
                        }
                    }
                }
                else {
                    for(int i = 0; i < numCards; i++) {
                        int j = Random.Range(0, opponent.hand.Count - 1) ;
                        DisplayCard hitCard = activeCardVisuals.Find(delegate(DisplayCard c) {
                            return (c.baseCard == opponent.hand[j] && c.owner == opponent);
                        });
                        while(selectedCards.Find(delegate(DisplayCard c) {
                            return hitCard.gameObject.GetInstanceID() == c.gameObject.GetInstanceID();
                        }) != null) {
                            j = Random.Range(0, opponent.hand.Count - 1) ;
                            hitCard = activeCardVisuals.Find(delegate(DisplayCard c) {
                                return (c.baseCard == opponent.hand[j] && c.owner == opponent);
                            });
                        }
                        selectedCards.Add(hitCard);
                    }
                }
                selectionConfirmation = true;
                yield return null;
            } 
            else if(Input.GetMouseButtonDown(0)) {
                RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Card"));
                if(hit.collider != null) {
                    DisplayCard hitCard = hit.collider.gameObject.GetComponent<DisplayCard>();
                    if ((hitCard.baseCard is NumberCard && cardType == SpecialKeyword.TYPE_NUMBER) || 
                    (hitCard.baseCard is SpecialDeckCard && cardType == SpecialKeyword.TYPE_SPECIAL)){
                        if(selectedCards.Find(delegate(DisplayCard c) {
                        return hitCard.gameObject.GetInstanceID() == c.gameObject.GetInstanceID();
                        }) == null) {
                            if(selectedCards.Count == numCards) {
                                Debug.Log("Selected too many cards, deselect something first!");
                            }
                            else{
                                Debug.Log("Selected: " + hitCard.baseCard.name);
                                hitCard.ToggleSelected();
                                selectedCards.Add(hitCard);
                            }
                        }
                        else{
                            Debug.Log("Deselected: " + hitCard.baseCard.name);
                            hitCard.ToggleSelected();
                            selectedCards.Remove(hitCard);
                        }
                    }
                }
                if(selectedCards.Count >= numCards && setupPlayerUI) {

                    enableSelectionConfirmButton = true;
                    selectionConfirmation = false;
                }
                else if(!setupPlayerUI) {
                    selectionConfirmation = true;
                }
                else{
                    enableSelectionConfirmButton = false;
                    selectionConfirmation = false;
                }

            }
            yield return null;
        }
        switch(selectionPurpose) {
            case SpecialKeyword.EFFECT_DISCARD:
                foreach(DisplayCard card in selectedCards) {
                    DiscardCard(card);
                }
                break;
        }
        selectionConfirmation = false;
        UIToggleSelectionMode(false);
        state = prevState;
        selectCoroutineRunning = false;
    }

    //play a special card
    public void PlayCard(DisplayCard display) {
        Debug.Log(display.owner.name + " played " + display.baseCard.name);
        SpecialDeckCard card = (SpecialDeckCard) display.baseCard;
        discardPile.Add(display.baseCard);
        display.owner.hand.Remove(card);
        activeCardVisuals.Remove(display);
        Destroy(display.gameObject);

        List<SpecialKeyword> keywords = card.keywords;
        List<int> values = card.values;
        CardGameCharacter playerTarget = display.owner;
        CardGameCharacter opponentTarget;
        if(playerTarget == opponent) {
            opponentTarget = player;
        }
        else {
            opponentTarget = opponent;
        }
        //do the decision tree (separate function because i forsee this eventually using recursion)
        ExecuteCardEffect(keywords, values, playerTarget, opponentTarget);
    }

    //discard a special card
    public void DiscardCard(DisplayCard display) {
        //NYI remove card from hand, update top of discard pile to show this card, add to discarded cards
        Debug.Log(display.owner.name + " discarded " + display.baseCard.name);
        if(display.baseCard is SpecialDeckCard) {
            display.owner.hand.Remove((SpecialDeckCard)display.baseCard);
        }
        else {
            display.owner.numberHand.Remove((NumberCard)display.baseCard);
        }
        display.owner.discardFlag = true;
        discardPile.Add(display.baseCard);
        activeCardVisuals.Remove(display);
        Destroy(display.gameObject);
    }

    void ShuffleCards(List<SpecialDeckCard> shuffle) {
        for(int i = shuffle.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1) ;
            SpecialDeckCard temp = shuffle[i];
            shuffle[i] = shuffle [j];
            shuffle [j] = temp;
        }
    }
    void ShuffleCards(List<NumberCard> shuffle) {
        for(int i = shuffle.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1) ;
            NumberCard temp = shuffle[i];
            shuffle[i] = shuffle [j];
            shuffle [j] = temp;
        }
    }

    void UIToggleSelectionMode(bool toggle) {
        if(toggle) {
            endTurnButton.SetActive(false);
            selectConfirmButton.SetActive(true);
            playerRoundToggle.gameObject.SetActive(false);
            opponentRoundToggle.gameObject.SetActive(false);
            selectionAmountText.gameObject.SetActive(true);
        }
        else {
            endTurnButton.SetActive(true);
            selectConfirmButton.SetActive(false);
            playerRoundToggle.gameObject.SetActive(true);
            opponentRoundToggle.gameObject.SetActive(true);
            selectionAmountText.gameObject.SetActive(false);
            enableSelectionConfirmButton = false;
        }
    }

    public void ButtonEndTurn() {
        prevState =  state;
        state = State.ENDTURN;
    }
    public void ButtonConfirmSelection(){
        selectionConfirmation = true;
    }

    //Very PROTOTYPE version of the card decision tree
    void ExecuteCardEffect(List<SpecialKeyword> keywords, List<int> values, CardGameCharacter playerTarget, CardGameCharacter opponentTarget) {
        List<SpecialKeyword> currKeys = new List<SpecialKeyword>();
        List<int> currValues = new List<int>();
        List<SpecialKeyword> truncatedKeys = new List<SpecialKeyword>();
        List<int> truncatedValues = new List<int>();
        int skippedValues = 0;
        for(int i = 0; i < keywords.Count; i++ ) {
            if(keywords[i] == SpecialKeyword.END_COMMAND) {
                for(int j = i+1; j < keywords.Count; j++) {
                    truncatedKeys.Add(keywords[j]);
                }
                for(int k = i - skippedValues; k < values.Count; k++) {
                    truncatedValues.Add(values[k]);
                }
                ExecuteCardEffect(truncatedKeys, truncatedValues, playerTarget, opponentTarget);
                break;
            }
            else if(keywords[i] == SpecialKeyword.TARGET_PLAYER || keywords[i] == SpecialKeyword.TARGET_OPPONENT || keywords[i] == SpecialKeyword.CON_STORE_ADDITIONAL_INTEGER) {
                currKeys.Add(keywords[i]);
                currValues.Add(values[i - skippedValues]);
            }
            else{
                currKeys.Add(keywords[i]);
                skippedValues += 1;
            }
        }
        keywords = currKeys;
        values = currValues;
        /*Debug.Log("Keywords: " );
        foreach(SpecialKeyword k in currKeys) {
            Debug.Log(k);
        }
        Debug.Log("Values: " );
        foreach(int v in currValues) {
            Debug.Log(v);
        }*/
        SpecialKeyword effectType = keywords[0];

        switch(effectType) {
            case SpecialKeyword.EFFECT_NONE:
                Debug.Log("This card has no effect");
            break;
            case SpecialKeyword.EFFECT_ADDVALUE:
                /*EFFECT_ADDVALUE ANTICPATED SYNTAX:
                keywords[i] = target to add value to
                values[i - 1] = amount to add to keywords[i]
                */
                for(int i = 1; i < keywords.Count; i++) {
                    if(keywords[i] == SpecialKeyword.TARGET_PLAYER) {
                        playerTarget.currValue += values[i-1];
                    }
                    else{
                        opponentTarget.currValue += values[i-1];
                    }
                }
            break;
            case SpecialKeyword.EFFECT_DRAW:
                /* EFFECT_DRAWCARD ANTICIPATED SYNTAX
                keywords[last item] = type of card to draw
                keywords[i] -> keywords[2nd to last item] = target to draw to
                values[i-1] = # cards to draw*/
                SpecialKeyword cardType = keywords[keywords.Count - 1];

                for(int i = 1; i< keywords.Count - 1; i++) {
                    if(keywords[i] == SpecialKeyword.TARGET_PLAYER && cardType == SpecialKeyword.TYPE_SPECIAL) {
                        DrawSpecialCards(playerTarget, values[i-1]);
                    }
                    else if(keywords[i] == SpecialKeyword.TARGET_PLAYER && cardType == SpecialKeyword.TYPE_NUMBER){
                        Debug.Log("drawing number cards NYI");
                    }
                    else if(keywords[i] == SpecialKeyword.TARGET_OPPONENT && cardType == SpecialKeyword.TYPE_NUMBER){
                        Debug.Log("drawing number cards NYI");
                    }
                    else if(keywords[i] == SpecialKeyword.TARGET_OPPONENT && cardType == SpecialKeyword.TYPE_SPECIAL){
                        DrawSpecialCards(opponentTarget, values[i-1]);
                    }
                }

            break;
            case SpecialKeyword.EFFECT_DISCARD:
                /*EFFECT_DISCARD ANTICIPATED SYNTAX
                keywords[last item] = type of card to discard
                keywords[1 -> 2nd to last item] = the discard target
                values[0] = # to discard.
                
                done in a coroutine and selectcards state to allow everyone to select their cards which may take multiple framesit is presently set up 
                only to work if there is only one card type that needs discarding*/
                for(int i = 1; i < keywords.Count - 1; i++ ) {
                    if(keywords[i] == SpecialKeyword.TARGET_PLAYER) {
                        CardSelectSettings newSettings = new CardSelectSettings(values[i-1], keywords[keywords.Count -1], keywords[0], playerTarget == player);
                        cardSelectStack.Push(newSettings);
                    }
                    else{
                        CardSelectSettings newSettings = new CardSelectSettings(values[i-1], keywords[keywords.Count -1], keywords[0], !playerTarget == player);
                        cardSelectStack.Push(newSettings);
                    }
                }
            break;
            case SpecialKeyword.EFFECT_CONDITIONAL:
                //first, if keyword is conditional, verify the CONDITION. this will return a bool. if true, execute SUCCESS_PATH. if false, execute FAILURE_PATH
                //every conditional card must have EFFECT_CONDITION, CONDITION_[target] and then SUCCESS_PATH and FAILURE_PATH

                List<SpecialKeyword> conditionalFlags = new List<SpecialKeyword>();
                List<SpecialKeyword> successCommand = new List<SpecialKeyword>();
                List<SpecialKeyword> failCommand = new List<SpecialKeyword>();

                List<int> conditionalValues = new List<int>();
                List<int> successValues = new List<int>();
                List<int> failValues = new List<int>();
                skippedValues = 0;
                List<SpecialKeyword> sortingBin = conditionalFlags;
                List<int> valuesBin = conditionalValues;

                for(int i = 1; i < keywords.Count; i++) {
                    if(keywords[i] == SpecialKeyword.SUCCESS_PATH) {
                        sortingBin = successCommand;
                        valuesBin = successValues;
                    }
                    else if(keywords[i] == SpecialKeyword.FAILURE_PATH) {
                        sortingBin = failCommand;
                        valuesBin = failValues;
                    }
                    else if(keywords[i] == SpecialKeyword.END_COMMAND){
                        break;
                    }

                    if(keywords[i] == SpecialKeyword.TARGET_PLAYER || keywords[i] == SpecialKeyword.TARGET_OPPONENT || keywords[i] == SpecialKeyword.CON_STORE_ADDITIONAL_INTEGER) {
                        sortingBin.Add(keywords[i]);
                        valuesBin.Add(values[i - 1 - skippedValues]);
                    }
                    else{
                        if(keywords[i] != SpecialKeyword.SUCCESS_PATH && keywords[i] != SpecialKeyword.FAILURE_PATH) {
                            sortingBin.Add(keywords[i]);
                        }
                        skippedValues += 1;
                    }
                    
                }

                bool successCheck = true;
                int flagsConsumed = 0;
                int valuesConsumed = 0;
                CardGameCharacter flagTarget;

                for(int i = 0; i < conditionalFlags.Count; i++) {
                    flagsConsumed = 0;
                    if(conditionalFlags[i+1] == SpecialKeyword.TARGET_PLAYER) {
                        flagTarget = playerTarget;
                    }
                    else{
                        flagTarget = opponentTarget;
                    }
                    switch (conditionalFlags[i]){
                        case SpecialKeyword.CON_CARD_QUANTITY:
                            Debug.Log("quantity");
                            successCheck = successCheck & ConditionalCardQuantity(flagTarget, conditionalFlags[i+2], conditionalValues[valuesConsumed], conditionalValues[valuesConsumed + 1]);
                            flagsConsumed += 3;
                            valuesConsumed += 2;
                            break;
                        case SpecialKeyword.CON_DISCARD_FLAG:
                            Debug.Log("discard");
                            successCheck = successCheck & ConditionalDiscardFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_FLIP_FLAG:
                            Debug.Log("flip");
                            successCheck = successCheck & ConditionalFlipFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_HAS_CLASS_CARD:
                            Debug.Log("has class card");
                            successCheck = successCheck & ConditionalHasClassCard(flagTarget, (NumberCard.NumberClass)conditionalValues[valuesConsumed], conditionalValues[valuesConsumed+1]);
                            flagsConsumed += 3;
                            valuesConsumed += 2;
                            break;
                        case SpecialKeyword.CON_HAS_DUPLICATE:
                            Debug.Log("has dupe");
                            successCheck = successCheck & ConditionalHasDuplicate(flagTarget, conditionalFlags[i+2]);
                            flagsConsumed += 2;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_HAS_VALUE_CARD:
                            Debug.Log("has value");
                            successCheck = successCheck & ConditionalHasValueCard(flagTarget, conditionalValues[valuesConsumed], conditionalValues[valuesConsumed + 1]);
                            flagsConsumed += 2;
                            valuesConsumed += 2;
                            break;
                        case SpecialKeyword.CON_SWAP_FLAG:
                            Debug.Log("swap");
                            successCheck = successCheck & ConditionalSwapFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_TRANSFER_FLAG:
                            Debug.Log("transfer");
                            successCheck = successCheck & ConditionalTransferFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_COMPARE_AGAINST_TARGET:
                            Debug.Log("compare against target");
                            successCheck = successCheck & ConditionalCompareAgainstTarget(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                    }
                    i += flagsConsumed;
                }

                Debug.Log(successCheck);

                if(successCheck) {
                    ExecuteCardEffect(successCommand, successValues, playerTarget, opponentTarget);
                }
                else {
                    ExecuteCardEffect(failCommand, failValues, playerTarget, opponentTarget);
                }

            break;
        }
    }

/*TARGET -> color
STORE INT -> count*/
    private bool ConditionalHasClassCard(CardGameCharacter target, NumberCard.NumberClass color, int count = 1){
        int x = 0;
        foreach(NumberCard c in target.numberHand) {
            if(c.cardClass == color) {
                x++;
            }
        }
        return x >= count;
    }
/*TARGET -> value
STORE INT -> count*/
    private bool ConditionalHasValueCard(CardGameCharacter target, int value, int count = 1){
        int x = 0;
        foreach(NumberCard c in target.numberHand) {
            if(c.value == value){
                x++;
            }
        }
        return x >= count;
    }
/*TARGET -> -1 (N/A)*/
    private bool ConditionalHasDuplicate(CardGameCharacter target, SpecialKeyword type){
        if(type == SpecialKeyword.TYPE_SPECIAL) {
            for(int i = 0; i < target.hand.Count - 1; i++){
                for(int j = i + 1; j < target.hand.Count; j++) {
                    if(target.hand[i].name == target.hand[j].name) {
                        return true;
                    }
                }
            }
        }
        else {
            for(int i = 0; i < target.numberHand.Count - 1; i++){
                for(int j = i + 1; j < target.numberHand.Count; j++) {
                    if(target.numberHand[i].name == target.numberHand[j].name) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
/*TARGET -> -1 (N/A)*/
    private bool ConditionalDiscardFlag(CardGameCharacter target){
        return target.discardFlag;
    }
/*TARGET -> -1 (N/A)*/
    private bool ConditionalSwapFlag(CardGameCharacter target){
        return target.swapFlag;
    }
/*TARGET -> -1 (N/A)*/
    private bool ConditionalFlipFlag(CardGameCharacter target){
        return target.flipFlag;
    }
/*TARGET -> -1 (N/A)*/
    private bool ConditionalTransferFlag(CardGameCharacter target){
        return target.transferFlag;
    }
    /*TARGET -> min
    STORE INT -> max*/
    private bool ConditionalCardQuantity(CardGameCharacter target, SpecialKeyword type, int min, int max){
       if(type == SpecialKeyword.TYPE_SPECIAL) {
        return (target.hand.Count >= min && target.hand.Count <= max);
       } 
       else {
        return (target.numberHand.Count >= min && target.numberHand.Count <= max);
       }
    }

/* TARGET -> -1 (N/A) */
    private bool ConditionalCompareAgainstTarget(CardGameCharacter target) {
        return target.currValue >= targetValue;
    }

}

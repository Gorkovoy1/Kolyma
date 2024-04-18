using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public struct CardSelectSettings
{
    public readonly int numCards { get; }
    public readonly CardType cardType { get; }
    public readonly Effect selectionPurpose { get; }

    public readonly CharacterInstance targetCharacter; 
    public readonly bool setupPlayerUI { get; }

    public CardSelectSettings(int number, CardType card, Effect effect, CharacterInstance character, bool playerUI)
    {
        numCards = number;
        cardType = card;
        selectionPurpose = effect;
        targetCharacter = character;
        setupPlayerUI = playerUI;
    }
}

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


    [Header("Card Game Info")]
    //This class manages the card game. It is designed to attach to the card game scene and load and run one card game from start to finish.

    public int roundCount = 0; //defaults to round 0 whenever a new game is started

    public CardGameCharacter playerCharacter, opponentCharacter;

    public CharacterInstance player, opponent; 

    public List<NumberCard> numberDeck; //the numbers deck- assuming this is communal?

    public bool isStoryBattle = false; //is this a one-round story battle or not? pass this var in from outside when triggering the battle. if not set defaults to normal 3 round battle.

    private int opponentPoints, playerPoints = 0; //rounds won score for each person

    public int bet = 5; //amt bet on game

    public int targetValue; //target value to win a round

    private bool roundOver = false; //becomes true when someone meets or exceeds target value.
    public State state = State.INIT; //current state
    private State prevState; //record which state we were in before we paused so we can go back to it
    private bool opponentEndRound, playerEndRound;

    private List<GenericCard> discardPile = new List<GenericCard>();

    [HideInInspector] public List<DisplayCard> activeCardVisuals;

    private bool printed = false;
    private bool selectionConfirmation = false;
    private bool enableSelectionConfirmButton = false;

    //private List<GameObject> playerNegativeCards = new List<GameObject>();
    //private List<GameObject> opponentNegativeCards = new List<GameObject>();
    //private List<int> playerNegativeCardsValues = new List<int>();
    //private List<int> opponentNegativeCardsValues = new List<int>();
    private bool selectCoroutineRunning = false;
    public Stack<CardSelectSettings> cardSelectStack = new Stack<CardSelectSettings>();

    [Header("UI References")] //references to all the UI elements in the scene
    public GameObject cardVisualPrefab;
    public GameObject endTurnButton, selectConfirmButton;
    public NumberCardOrganizer OpponentCardZone, PlayerCardZone, OpponentNegativeCardZone, PlayerNegativeCardZone;
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


    public int offsetInteger;
    public int scaleInteger;
    public float scaleNumber;

    public static CardGameManager Instance;

    public TextMeshProUGUI FlipText, SwapText;
    public Button FlipButton, SwapButton;


    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //Update curr value text and such every frame because its easier to just do it here
        /* Very Inefficient! UI rendering updates are costly in Unity. You should keep them to a minimum */

        //update selection confirmation button
        selectConfirmButton.SetActive(enableSelectionConfirmButton);

        //check if we need to be selecting cards because a selection event is queued
        if (cardSelectStack.Count > 0 && state != State.SELECTCARDS) {
            prevState = state;
            state = State.SELECTCARDS;
        }

        //Ye Olde Turn Manager State Machine
        /*Most of these probably shouldn't be in Update*/
        switch (state) {
            case State.INIT:
                Init();
                break;

            case State.STARTGAME:
                StartGame();
                break;

            case State.STARTROUND:
                StartRound();
                break;

            case State.PLAYERTURN:
                PlayerTurn();
                break;

            case State.OPPONENTTURN:
                OpponentTurn();
                break;

            case State.ENDTURN:
                EndTurn();
                break;
            case State.SELECTCARDS:
                SelectCard();
                break;

            case State.ENDROUND:
                EndRound();
                break;

            case State.ENDGAME:
                EndGame();
                break;

            case State.PAUSED:
                Paused();
                break;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    void Init()
    {
        //loading tasks go here
        player = gameObject.AddComponent<CharacterInstance>();
        player.Init(playerCharacter, PlayerCardZone, PlayerNegativeCardZone);
        opponent = gameObject.AddComponent<CharacterInstance>();
        opponent.Init(opponentCharacter, OpponentCardZone, OpponentNegativeCardZone);
        player.FlushGameplayVariables();
        opponent.FlushGameplayVariables();
        opponentRoundToggle.isOn = false;
        playerRoundToggle.isOn = false;
        state = State.STARTGAME;
    }

    void StartGame()
    {
        /*NYI
        display option to double bet
        if(betDoubled) {
            bet = 10;
        }*/
        foreach (SpecialDeckCard card in player.character.deckList)
        {
            player.deck.Add(card);
        }
        foreach (SpecialDeckCard card in opponent.character.deckList)
        {
            opponent.deck.Add(card);
        }
        state = State.STARTROUND;
    }

    void StartRound()
    {
        playerEndRound = false;
        opponentEndRound = false;
        discardPile.Clear();
        ShuffleCards(player.deck);
        ShuffleCards(opponent.deck);
        ShuffleCards(numberDeck);
        DrawSpecialCards(opponent, 6);
        DrawSpecialCards(player, 6);
        DrawNumberCards(opponent, 4);
        DrawNumberCards(player, 4);
        roundCount += 1;
        targetValue = Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
        targetValueText.text = "" + targetValue; //"TARGET VALUE: " + 
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
    }

    public void UpdateValues()
    {
        opponent.currValue = opponent.addedValues;
        foreach(DisplayCard card in opponent.numberDisplayHand)
        {
            opponent.currValue += card.value;
        }
        player.currValue = player.addedValues;
        foreach (DisplayCard card in player.numberDisplayHand)
        {
            player.currValue += card.value;
        }
        opponentSumText.text = opponent.character.name + ":\n" + opponent.currValue;//CURR VALUE
        playerSumText.text = player.character.name + ":\n" + player.currValue;//CURR VALUE
    }

    public void ToggleFlipSelectionMode(bool on, bool forced = false)
    {
        player.CurrentlyFlipping = on;
        player.FlippingForced = forced;
    }

    public void ToggleFlip()
    {
        if (!player.CurrentlySwapping)
        {
            SetFlip(!player.CurrentlyFlipping);
        }
    }

    public void SetFlip(bool newFlip)
    {
        player.CurrentlyFlipping = newFlip;

        FlipText.text = player.CurrentlyFlipping ? "FLIPPING..." : "FLIP";
        if (player.CurrentlyFlipping)
        {
            //CardSelectSettings drawSettings = new CardSelectSettings(1, SpecialKeyword.TYPE_SPECIAL, SpecialKeyword.EFFECT_DRAW, true);
            //cardSelectStack.Push(drawSettings);
            CardSelectSettings flipSettings = new CardSelectSettings(1, CardType.Number, Effect.Flip, player, true);
            cardSelectStack.Push(flipSettings);
        }
    }

    public void EndFlip()
    {
        FlipText.text = "FLIPPED";
        player.FlippedThisTurn = true;
        player.CurrentlyFlipping = false;
        FlipButton.interactable = false;
    }

    public void ToggleSwap()
    {
        if(!player.CurrentlyFlipping)
        {
            SetSwap(!player.CurrentlySwapping);
        }
    }

    public void SetSwap(bool newSwap)
    {
        if(!player.CurrentlyFlipping)
        {
            player.CurrentlySwapping = newSwap;

            SwapText.text = player.CurrentlySwapping ? "SWAPPING..." : "SWAP";

            if (player.CurrentlySwapping)
            {
                //CardSelectSettings drawSettings = new CardSelectSettings(1, SpecialKeyword.TYPE_SPECIAL, SpecialKeyword.EFFECT_DRAW, true);
                //cardSelectStack.Push(drawSettings);
                CardSelectSettings swapSettings = new CardSelectSettings(1, CardType.Number, Effect.Swap, player, true);
                cardSelectStack.Push(swapSettings);
            }
        }
    }

    public void EndSwap()
    {
        SwapText.text = "SWAPPED";
        player.SwappedThisTurn = true;
        player.CurrentlySwapping = false;
        SwapButton.interactable = false;
    }

    void PlayerTurn()
    {
        /*NYI
        Take input and perform actions for player turn. swap to end turn when clicking end turn button.
        */
        //player plays one special card OR uses their action AND may swap out one of their cards from their hand - they can also pass
        playerEndRound = playerRoundToggle.isOn;
        opponentEndRound = opponentRoundToggle.isOn;
    }

    void OpponentTurn()
    {
        //use AI to choose best action - sift through if opp closer or player closer, if better to attack or defend
        ButtonEndTurn(); //AI NYI so just skip turn for now.
    }

    void EndTurn()
    {
        if (playerEndRound && opponentEndRound)
        {
            state = State.ENDROUND;
        }
        if (prevState == State.PLAYERTURN && !opponentEndRound)
        {
            DrawSpecialCards(opponent, 1);
            state = State.OPPONENTTURN;
            player.FlushFlags();
        }
        else if (prevState == State.OPPONENTTURN && !playerEndRound)
        {
            DrawSpecialCards(player, 1);
            state = State.PLAYERTURN;
            opponent.FlushFlags();
        }
    }

    void SelectCard()
    {
        //this state mostly exists to lock the player out of performing unwanted actions and acknowledge to the rest of the system that the player is in the
        //middle of picking a card for some purpose.   
        if (!selectCoroutineRunning && cardSelectStack.Count > 0)
        {
            CardSelectSettings curr = cardSelectStack.Pop();
            StartCoroutine(SelectCardState(curr.numCards, curr.cardType, curr.selectionPurpose, curr.targetCharacter, curr.setupPlayerUI));
        }

    }

    void EndRound()
    {
        Debug.Log("End round");
        if (Mathf.Abs(targetValue - player.currValue) < Mathf.Abs(targetValue - opponent.currValue))
        {
            Debug.Log(player.character.name + " won this round");
            playerPoints += 1;
        }
        else if (Mathf.Abs(targetValue - player.currValue) > Mathf.Abs(targetValue - opponent.currValue))
        {
            Debug.Log(opponent.character.name + " won this round");
            opponentPoints += 1;
        }
        else
        {
            Debug.Log("This round was a tie with no victor");
        }

        if (playerPoints >= 2 || opponentPoints >= 2 || roundCount >= 3)
        {
            state = State.ENDGAME;
        }
        else
        {
            playerPos = 0f;
            opponentPos = 0f;
            //playerNegativeCardsValues.Clear();
            //opponentNegativeCardsValues.Clear();
            //playerNegativeCards.Clear();
            //opponentNegativeCards.Clear();
            foreach (DisplayCard card in activeCardVisuals)
            {
                if (card.baseCard is NumberCard)
                {
                    numberDeck.Add((NumberCard)card.baseCard);
                }
                Destroy(card.gameObject);
            }
            activeCardVisuals.Clear();
            discardPile.Clear();
            player.FlushGameplayVariables();
            opponent.FlushGameplayVariables();
            foreach (SpecialDeckCard card in player.character.deckList)
            {
                player.deck.Add(card);
            }
            foreach (SpecialDeckCard card in opponent.character.deckList)
            {
                opponent.deck.Add(card);
            }
            playerRoundToggle.isOn = false;
            opponentRoundToggle.isOn = false;
            Debug.Log("New Round");
            state = State.STARTROUND;
        }
    }

    void EndGame()
    {
        if (!printed)
        {
            Debug.Log("Game over!");
            if (playerPoints >= 2)
            {
                Debug.Log("winner is " + player.character.name);
            }
            else
            {
                Debug.Log("winner is " + opponent.character.name);
            }
            printed = true;
        }
        /*NYI
        transfer out of card game scene and record winner of match in game data */
    }

    void Paused()
    {
        /*NYI
        if(exit pause menu) {
            state = prevState;
        }*/
    }

    public void DrawNumberCards(CharacterInstance target, int numberCards) {
        for(int i = 0; i < numberCards; i++) {
            if(numberDeck.Count == 0) {
                Debug.Log("Number Deck is Empty");
                break;
            }
            NumberCard newCard = numberDeck[0];
            numberDeck.Remove(newCard);
            GameObject newCardVisual = Instantiate(cardVisualPrefab);
            DisplayCard newCardDisplay = newCardVisual.GetComponent<DisplayCard>();
            target.numberDisplayHand.Add(newCardDisplay);
            target.test.Add(newCardVisual);
            //newCardDisplay.owner = target;
            //newCardDisplay.baseCard = newCard;
            newCardDisplay.InitNumberCard(newCard, target);
            PlaceCard(newCardVisual, target, newCard.value);
            target.currValue += newCard.value;
            Debug.Log("New Number Card for " + target.character.name + ": " + newCard.value);
            /*if (target == player) {
                player.currValue += newCard.value;
            }
            else {
                PlaceCard(newCardVisual, target, newCard.value);
                opponent.currValue += newCard.value;
            }*/
            activeCardVisuals.Add(newCardDisplay);
       }
        UpdateValues();
    }

    void PlaceCard(GameObject card, CharacterInstance target, int value)
    {
        /*Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        offset = (screenWidth/offsetInteger)*51/128;
        scaleNumber = (screenWidth/offsetInteger);*/

        //note since its 3/scaleNumber, if screensize is tiny then the cards are super large - bugproof this
        //also find a way to unify scaling based on maybe ratio of width vs height so that everything scales consistently?

        if(target == player)
        {
            //card.transform.SetParent(PlayerCardZone);

            if(value <=0)
            {
                PlayerNegativeCardZone.PlaceCard(card);
                //playerNegativeCards.Add(card);
                //playerNegativeCardsValues.Add(value);
            }
            else
            {
                PlayerCardZone.PlaceCard(card);
            }
            /*if (value>0)
            {
                //card.transform.localScale = new Vector3 (3/scaleNumber, 3/scaleNumber, 3/scaleNumber);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector3(playerPos, screenHeight/7, 0);
                playerPos = playerPos + value*offset;

            }
            else
            {
                playerNegativeCards.Add(card);
                playerNegativeCardsValues.Add(value);
            }*/
            
        } 
        else
        {
            if(value <= 0)
            {
                OpponentNegativeCardZone.PlaceCard(card);
                //opponentNegativeCards.Add(card);
                //opponentNegativeCardsValues.Add(value);
            }
            else
            {
                OpponentCardZone.PlaceCard(card);
            }
            /*if(value>0)
            {
                Debug.Log(screenHeight);
                Debug.Log(screenWidth);
                //card.transform.localScale = new Vector3 (3/scaleNumber, 3/scaleNumber, 3/scaleNumber);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector3(opponentPos, -(screenHeight/14), 0);
                opponentPos = opponentPos + value*offset;
            }
            else{
                opponentNegativeCards.Add(card);
                opponentNegativeCardsValues.Add(value);
            }*/
        }
        /*int cardSize = screenWidth/170;
        negPos = playerPos - cardSize*offset;
        opponentNegPos = opponentPos - cardSize*offset;*/
        //PlaceNegativeCard(playerNegativeCards, PlayerNegativeCardZone);//, playerNegativeCardsValues, negPos, player);
        //PlaceNegativeCard(opponentNegativeCards, OpponentNegativeCardZone);//, opponentNegativeCardsValues, opponentNegPos, opponent);
    }

    /*void PlaceNegativeCard(GameObject negativeCard, Transform zone)//, List<int> negativeCardsValues, float pos, CardGameCharacter target)
    {
        foreach(GameObject card in negativeCards)
        {
            card.transform.SetParent(zone);
        }
        /*int x = 0;
        int screenHeight = Screen.height;
        int screenWidth = Screen.width;
        if(target == player)
        {
            x = (screenHeight/14);
        }
        else{
            x = -(screenHeight/7);
        }
        for(int i = 0; i < negativeCards.Count; i++)
        {
            //negativeCards[i].transform.localScale = new Vector3 (3/scaleNumber, 3/scaleNumber, 3/scaleNumber);
            negativeCards[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(pos, x, 0);
            pos = pos + negativeCardsValues[i]*offset;
        }
    }*/

    public void DrawSpecialCards(CharacterInstance target, int specialCards) {
        /*NYI
        draw specified number of cards from each deck and put it in target's hand */
        /*Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        float scaleFactor = Mathf.Min(screenWidth, screenHeight) * 0.00025f;*/

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
            newCardDisplay.InitSpecialCard(newCard, target);
            newCardDisplay.owner = target;
            newCardDisplay.baseCard = newCard;
            if(target == player) {
                
                //newCardVisual.transform.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);
                newCardVisual.transform.SetParent(playerHandTransform);
            }
            else {
                //newCardVisual.transform.localScale =  new Vector3 (scaleFactor, scaleFactor, scaleFactor);
                newCardVisual.transform.SetParent(opponentHandTransform);
            }
            activeCardVisuals.Add(newCardDisplay);
        }
    }

    //enter the select card state
    private IEnumerator SelectCardState(int numCards, CardType cardType, Effect effect, CharacterInstance targetCharacter, bool setupPlayerUI = true, List<DisplayCard> selectedCards = null) {
        selectCoroutineRunning = true;
        enableSelectionConfirmButton = false;

        if(selectedCards == null) 
            selectedCards = new List<DisplayCard>();
        
        UIToggleSelectionMode(setupPlayerUI);
        /*if (setupPlayerUI) {
            UIToggleSelectionMode(true);
        }
        else{
            UIToggleSelectionMode(false);
        }*/

        //check if need to select more cards than the player has, in which case just select everything they have.

        if(setupPlayerUI)
        {
            switch(cardType)
            {
                case CardType.Special:
                    if (numCards > player.hand.Count)
                    {
                        numCards = player.hand.Count;
                    }
                    break;
                case CardType.Number:
                    if (numCards > player.numberDisplayHand.Count)
                    {
                        numCards = player.numberDisplayHand.Count;
                    }
                    break;
            }
        }
        else
        {
            switch (cardType)
            {
                case CardType.Special:
                    if(numCards > opponent.hand.Count)
                    {
                        numCards = opponent.hand.Count;
                    }
                    break;
                case CardType.Number:
                    if(numCards > opponent.numberDisplayHand.Count)
                    {
                        numCards = opponent.numberDisplayHand.Count;
                    }
                    break;
            }
        }

        /*Reorganized for readability
         * 
         * if(setupPlayerUI && cardType == SpecialKeyword.TYPE_SPECIAL && numCards > player.hand.Count) {
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
        }*/

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
                    if ((hitCard.baseCard is NumberCard && cardType == CardType.Number) || (hitCard.baseCard is SpecialDeckCard && cardType == CardType.Special)){
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
        switch(effect) {
            case Effect.Discard:
                foreach(DisplayCard card in selectedCards) {
                    DiscardCard(card);
                }
                break;
            case Effect.Swap:
                foreach (DisplayCard card in selectedCards)
                {
                    SwapCard(card);
                }
                EndSwap();
                break;
            case Effect.Flip:
                foreach(DisplayCard card in selectedCards)
                {
                    FlipCard(card);
                }
                EndFlip();
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

        //List<SpecialKeyword> keywords = card.keywords;
        //List<int> values = card.values;
        CharacterInstance playerTarget = display.owner;
        CharacterInstance opponentTarget;
        if(playerTarget == opponent) {
            opponentTarget = player;
        }
        else {
            opponentTarget = opponent;
        }
        //do the decision tree (separate function because i forsee this eventually using recursion)
        Debug.Log("Card Played: " + card.name + "\n" + card.description);
        CardEffectChecker.Instance.ExecuteEffectStatement(card.InitialEffectStatement, playerTarget, opponentTarget);
        //CardEffectChecker.Instance.ExecuteCardEffect(keywords, values, playerTarget, opponentTarget);

        UpdateValues();
    }

    //discard a special card
    public void DiscardCard(DisplayCard display) 
    {
        //NYI remove card from hand, update top of discard pile to show this card, add to discarded cards
        Debug.Log(display.owner.name + " discarded " + display.baseCard.name);
        if(display.baseCard is SpecialDeckCard) {
            display.owner.hand.Remove((SpecialDeckCard)display.baseCard);
        }
        else {
            display.owner.numberDisplayHand.Remove(display);
        }
        display.owner.DiscardedThisTurn = true;
        discardPile.Add(display.baseCard);
        activeCardVisuals.Remove(display);
        if(display.baseCard is NumberCard)
        {
            display.NumberCardOrganizer.RemoveCard(display.gameObject);
            display.gameObject.SetActive(false);
        }
        UpdateValues();
    }

    void SwapCard(DisplayCard display)
    {
        if (display.baseCard is SpecialDeckCard)
        {
            display.owner.hand.Remove((SpecialDeckCard)display.baseCard);
            DrawSpecialCards(display.owner, 1);
        }
        else
        {
            display.owner.numberDisplayHand.Remove(display);
            DrawNumberCards(display.owner, 1);
        }
        display.owner.SwappedThisTurn = true;
        numberDeck.Add((NumberCard)display.baseCard);
        activeCardVisuals.Remove(display);
        display.gameObject.SetActive(false);
        UpdateValues();
    }

    void FlipCard(DisplayCard display)
    {
        display.value *= -1;
        if(display.value <= 0)
        {
            display.owner.PositiveCardsZone.RemoveCard(display.gameObject);
            display.owner.NegativeCardsZone.PlaceCard(display.gameObject);
        }
        else
        {
            display.owner.NegativeCardsZone.RemoveCard(display.gameObject);
            display.owner.PositiveCardsZone.PlaceCard(display.gameObject);
        }
        UpdateValues();
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

    void UIToggleSelectionMode(bool toggle)
    {
        endTurnButton.SetActive(!toggle);
        selectConfirmButton.SetActive(toggle);
        playerRoundToggle.gameObject.SetActive(!toggle);
        opponentRoundToggle.gameObject.SetActive(!toggle);
        selectionAmountText.gameObject.SetActive(toggle);
        enableSelectionConfirmButton = toggle;
        /*if (toggle) {
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
        }*/
    }

    public void ButtonEndTurn() {
        prevState =  state;
        state = State.ENDTURN;
    }

    public void ButtonConfirmSelection(){
        selectionConfirmation = true;
    }
}

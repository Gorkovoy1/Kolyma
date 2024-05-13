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
    public TextMeshProUGUI opponentSumText, playerSumText, targetValueText, selectionText;
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

    public CardSelectionHandler CardSelectionHandler;

    public event OnCharacterChangeDelegate OnCharacterChange;
    public delegate void OnCharacterChangeDelegate(CharacterInstance currCharacter);
    private CharacterInstance CurrentPlayer;

    void Awake()
    {
        Instance = this;
        CardSelectionHandler = gameObject.AddComponent<CardSelectionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
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
        player.Init(playerCharacter, PlayerCardZone, PlayerNegativeCardZone, true);
        opponent = gameObject.AddComponent<CharacterInstance>();
        opponent.Init(opponentCharacter, OpponentCardZone, OpponentNegativeCardZone, true);
        player.FlushGameplayVariables();
        opponent.FlushGameplayVariables();
        opponentRoundToggle.isOn = false;
        playerRoundToggle.isOn = false;
        state = State.STARTGAME;
    }

    void StartGame()
    {
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
        else
        {
            CardSelectionHandler.EndSelectingCards();
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
                CardSelectSettings swapSettings = new CardSelectSettings(1, CardType.Number, Effect.Swap, player, true);
                cardSelectStack.Push(swapSettings);
            }
            else
            {
                CardSelectionHandler.EndSelectingCards();
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
        //playerEndRound = playerRoundToggle.isOn;
        //opponentEndRound = opponentRoundToggle.isOn;
        CurrentPlayer = player;
        OnCharacterChange(player);
    }

    void OpponentTurn()
    {
        //use AI to choose best action - sift through if opp closer or player closer, if better to attack or defend
        //ButtonEndTurn(); //AI NYI so just skip turn for now.
        CurrentPlayer = opponent;
        OnCharacterChange(opponent);
    }

    public void EndTurn()
    {
        Debug.Log(CurrentPlayer.character.name + " ends turn!");
        /*if (playerEndRound && opponentEndRound)
        {
            state = State.ENDROUND;
        }*/

        if(CurrentPlayer == player)
        {
            state = State.OPPONENTTURN;
            DrawSpecialCards(opponent, 3);
        }
        else if(CurrentPlayer == opponent)
        {
            state = State.PLAYERTURN;
            DrawSpecialCards(player, 3);
        }

        /*if (prevState == State.PLAYERTURN)// && !opponentEndRound)
       
            DrawSpecialCards(opponent, 1);
            state = State.OPPONENTTURN;
            player.FlushFlags();
        }
        else if (prevState == State.OPPONENTTURN)// && !playerEndRound)
        {
            DrawSpecialCards(player, 1);
            state = State.PLAYERTURN;
            opponent.FlushFlags();
        }*/
    }

    public List<DisplayCard> GetSelectableCards()
    {
        if(!CardSelectionHandler.Instance.SelectingCards)
        {
            return CurrentPlayer.specialDisplayHand;
        }
        else
        {
            return CardSelectionHandler.GetSelectableCards();
        }
    }

    void SelectCard()
    {
        //this state mostly exists to lock the player out of performing unwanted actions and acknowledge to the rest of the system that the player is in the
        //middle of picking a card for some purpose.   
        if (!CardSelectionHandler.SelectingCards && cardSelectStack.Count > 0)
        {
            CardSelectSettings curr = cardSelectStack.Pop();
            CardSelectionHandler.StartSelectingCards(curr);
            //StartCoroutine(SelectCardState(curr.numCards, curr.cardType, curr.selectionPurpose, curr.targetCharacter, curr.setupPlayerUI));
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
        Debug.Log("Game over!");
    }

    void Paused()
    {
        Debug.Log("Paused Not Yet Implemented!");
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
            newCardDisplay.InitNumberCard(newCard, target);
            PlaceCard(newCardVisual, target, newCard.value);
            target.currValue += newCard.value;
            //Debug.Log("New Number Card for " + target.character.name + ": " + newCard.value);
            activeCardVisuals.Add(newCardDisplay);
       }
        UpdateValues();
    }

    void PlaceCard(GameObject card, CharacterInstance target, int value)
    {
        if(target == player)
        {
            if(value <=0)
            {
                PlayerNegativeCardZone.PlaceCard(card);
            }
            else
            {
                PlayerCardZone.PlaceCard(card);
            }
        } 
        else
        {
            if(value <= 0)
            {
                OpponentNegativeCardZone.PlaceCard(card);
            }
            else
            {
                OpponentCardZone.PlaceCard(card);
            }
        }
    }

    public void DrawSpecialCards(CharacterInstance target, int specialCards) {
        for(int i = 0; i < specialCards; i ++) {
            if(target.deck.Count == 0) {
                Debug.Log(target.character.name + " is out of cards!");
                break;
            }
            SpecialDeckCard newCard = target.deck[0];
            target.deck.Remove(newCard);
            GameObject newCardVisual = Instantiate(cardVisualPrefab);
            DisplayCard newCardDisplay = newCardVisual.GetComponent<DisplayCard>();
            newCardDisplay.InitSpecialCard(newCard, target);
            newCardDisplay.owner = target;
            newCardDisplay.baseCard = newCard;
            target.specialDisplayHand.Add(newCardDisplay);
            if (target == player) {
                newCardVisual.transform.SetParent(playerHandTransform);
            }
            else {
                newCardVisual.transform.SetParent(opponentHandTransform);
            }
            activeCardVisuals.Add(newCardDisplay);
        }
    }

    public void PlayCard(DisplayCard display) {
        Debug.Log(display.owner.character.name + " played " + display.baseCard.name);
        SpecialDeckCard card = (SpecialDeckCard) display.baseCard;
        discardPile.Add(display.baseCard);
        display.owner.specialDisplayHand.Remove(display);
        activeCardVisuals.Remove(display);
        Destroy(display.gameObject);

        CharacterInstance playerTarget = display.owner;
        CharacterInstance opponentTarget;
        if(playerTarget == opponent) {
            opponentTarget = player;
        }
        else {
            opponentTarget = opponent;
        }
        CardEffectChecker.Instance.ExecuteEffectStatement(card.InitialEffectStatement, playerTarget, opponentTarget);

        UpdateValues();
    }

    public void DiscardCard(DisplayCard display) 
    {
        Debug.Log(display.owner.character.name + " discarded " + display.baseCard.name);
        if(display.baseCard is SpecialDeckCard) {
            display.owner.specialDisplayHand.Remove(display);
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

    public void SwapCard(DisplayCard display)
    {
        if (display.baseCard is SpecialDeckCard)
        {
            display.owner.specialDisplayHand.Remove(display);
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

    public void FlipCard(DisplayCard display)
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

    public void UIToggleSelectionMode(bool toggle)
    {
        endTurnButton.SetActive(!toggle);
        selectConfirmButton.SetActive(toggle);
        playerRoundToggle.gameObject.SetActive(!toggle);
        opponentRoundToggle.gameObject.SetActive(!toggle);
        selectionText.gameObject.SetActive(toggle);
        enableSelectionConfirmButton = toggle;
        if(toggle)
        {
            CardSelectSettings currSettings = CardSelectionHandler.CurrSettings;
            selectionText.text = "Select " + currSettings.numCards + " " + currSettings.cardType.ToString() + " " + (currSettings.numCards > 1 ? "cards" : "card") + " to " + currSettings.selectionPurpose.ToString() + ".";
        }
    }

    public void ButtonEndTurn() {
        prevState =  state;
        state = State.ENDTURN;
    }

    public void ButtonConfirmSelection(){
        CardSelectionHandler.ConfirmSelection();
    }
}

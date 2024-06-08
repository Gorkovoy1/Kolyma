using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        //SELECTCARDS, //selection mode to pick cards to swap or discard, etc etc
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

    public CharacterInstance SelectingCharacter;
    
    public List<NumberCard> numberDeck; //the numbers deck- assuming this is communal?

    private int opponentPoints, playerPoints = 0; //rounds won score for each person

    public int targetValue; //target value to win a round

    public State GameState = State.INIT; //current state

    private List<GenericCard> discardPile = new List<GenericCard>();

    [HideInInspector] public List<DisplayCard> activeCardVisuals;

    public Stack<CardSelectSettings> cardSelectStack = new Stack<CardSelectSettings>();

    public GameObject cardVisualPrefab;

    public static CardGameManager Instance;

    public CardSelectionHandler CardSelectionHandler;

    public event OnTurnChangeDelegate OnCharacterChange;
    public delegate void OnTurnChangeDelegate(CharacterInstance currCharacter);
    public CharacterInstance CurrentCharacter;

    public bool CharacterSelecting;

    void Awake()
    {
        Instance = this;
        CardSelectionHandler = gameObject.AddComponent<CardSelectionHandler>();
        SetNewState(State.INIT);
    }

    // Update is called once per frame
    /*void Update()
    {
        //check if we need to be selecting cards because a selection event is queued
        if (cardSelectStack.Count > 0 && GameState != State.SELECTCARDS) {
            //prevState = state;
            SetNewState(State.SELECTCARDS);
            //state = State.SELECTCARDS;
        }
        if(CharacterSelecting)
        {
            CardGameUIManager.Instance.ToggleSelectConfirmButton(CardSelectionHandler.CheckConditionsMet());
        }
    }*/

    public void SetNewState(State newState, CharacterInstance triggeringCharacter = null)
    {
        GameState = newState;
        switch (GameState)
        {
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
            //case State.SELECTCARDS:
            //    SelectingCards(triggeringCharacter);
            //    break;

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

    public void StartSelecting(CharacterInstance selectingCharacter)
    {
        CardSelectionHandler.Instance.ProcessSelect(selectingCharacter);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    void Init()
    {
        CardGameLog.Instance.ClearLog();
        GameObject newPlayerObj = new GameObject();
        newPlayerObj.name = "Player";
        player = newPlayerObj.AddComponent<CharacterInstance>();
        player.Init(playerCharacter, CardGameUIManager.Instance.PlayerPositiveCardZone, CardGameUIManager.Instance.PlayerNegativeCardZone, false);
        GameObject newOppObj = new GameObject();
        newOppObj.name = "Opponent";
        opponent = newOppObj.AddComponent<CharacterInstance>();
        opponent.Init(opponentCharacter, CardGameUIManager.Instance.OpponentPositiveCardZone, CardGameUIManager.Instance.OpponentNegativeCardZone, true);
        CardGameUIManager.Instance.Init(player, opponent);
        player.FlushGameplayVariables();
        opponent.FlushGameplayVariables();
        SetNewState(State.STARTGAME);
        //state = State.STARTGAME;
    }

    void StartGame()
    {
        CardGameLog.Instance.AddToLog("Game Start!");
        foreach (SpecialDeckCard card in player.character.deckList)
        {
            player.deck.Add(card);
        }
        foreach (SpecialDeckCard card in opponent.character.deckList)
        {
            opponent.deck.Add(card);
        }
        SetNewState(State.STARTROUND);
        //state = State.STARTROUND;
    }

    private void Update()
    {
        UpdateValues();
    }

    void StartRound()
    {
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
        CardGameUIManager.Instance.targetValueText.text = "" + targetValue; //"TARGET VALUE: " + 
        SetNewState(State.PLAYERTURN);
        //state = State.PLAYERTURN;
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
        CurrentCharacter.CurrentlyFlipping = newFlip;

        //FlipText.text = player.CurrentlyFlipping ? "FLIPPING..." : "FLIP";
        if (CurrentCharacter.CurrentlyFlipping)
        {
            //CardSelectSettings drawSettings = new CardSelectSettings(1, SpecialKeyword.TYPE_SPECIAL, SpecialKeyword.EFFECT_DRAW, true);
            //cardSelectStack.Push(drawSettings);
            CardSelectSettings flipSettings = new CardSelectSettings(1, CardType.Number, Effect.Flip, CurrentCharacter, true);
            cardSelectStack.Push(flipSettings);
            CardSelectionHandler.ProcessSelect(CurrentCharacter);
        }
        else
        {
            CardSelectionHandler.EndSelectingCards();
        }
        CardGameUIManager.Instance.ToggleFlipping(CurrentCharacter.CurrentlyFlipping);
    }

    public void EndFlip()
    {
        //FlipText.text = "FLIPPED";
        player.FlippedThisTurn = true;
        player.CurrentlyFlipping = false;
        player.DidAnAction = true;
        CardGameUIManager.Instance.ToggleFlipping(false);
        //FlipButton.interactable = false;
    }

    public void ToggleSwap()
    {
        if(!CurrentCharacter.CurrentlyFlipping)
        {
            SetSwap(!CurrentCharacter.CurrentlySwapping);
        }
    }

    public void SetSwap(bool newSwap)
    {
        if(!CurrentCharacter.CurrentlyFlipping)
        {
            CurrentCharacter.CurrentlySwapping = newSwap;

            //CardGameUIManager.Instance.ChangeSwapMode(SwapState.);
            //SwapText.text = player.CurrentlySwapping ? "SWAPPING..." : "SWAP";

            if (CurrentCharacter.CurrentlySwapping)
            {
                CardSelectSettings swapSettings = new CardSelectSettings(1, CardType.Number, Effect.Swap, CurrentCharacter, true);
                cardSelectStack.Push(swapSettings);
                CardSelectionHandler.ProcessSelect(CurrentCharacter);
            }
            else
            {
                CardSelectionHandler.EndSelectingCards();
            }

            if(CurrentCharacter == player)
                CardGameUIManager.Instance.ToggleSwapping(CurrentCharacter.CurrentlySwapping);
        }
    }

    public void EndSwap()
    {
        //SwapText.text = "SWAPPED";
        player.SwappedThisTurn = true;
        player.CurrentlySwapping = false;
        player.DidAnAction = true;
        //SwapButton.interactable = false;
        CardGameUIManager.Instance.ToggleSwapping(false);
    }

    void PlayerTurn()
    {
        CardGameLog.Instance.AddToLog("Player Turn!");
        CurrentCharacter = player;
        SelectingCharacter = player;
        OnCharacterChange(player);
        CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerTurn);

        CardGameUIManager.Instance.ResetUI();

        player.FlushFlags();
    }

    void OpponentTurn()
    {
        CardGameLog.Instance.AddToLog("Opponent Turn!");
        CurrentCharacter = opponent;
        SelectingCharacter = opponent;
        OnCharacterChange(opponent);
        CardGameUIManager.Instance.ChangeUIMode(UIMode.OpponentTurn);
        opponent.FlushFlags();
    }

    public void EndTurn()
    {
        CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " ends turn!");
        if(CurrentCharacter == player)
        {
            SetNewState(State.OPPONENTTURN);
            //state = State.OPPONENTTURN;
            DrawSpecialCards(opponent, 3);
            player.DidAnAction = false;
        }
        else if(CurrentCharacter == opponent)
        {
            SetNewState(State.PLAYERTURN);
            //state = State.PLAYERTURN;
            DrawSpecialCards(player, 3);
            opponent.DidAnAction = false;
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
            return CurrentCharacter.specialDisplayHand;
        }
        else
        {
            return CardSelectionHandler.GetSelectableCards();
        }
    }

    /*void SelectingCards(CharacterInstance selectingCharacter)
    {
        Debug.Log(selectingCharacter.character.name + " is selecting!");
        CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerSelecting);
        SelectingCharacter = selectingCharacter;
        //this state mostly exists to lock the player out of performing unwanted actions and acknowledge to the rest of the system that the player is in the
        //middle of picking a card for some purpose.   
        if (!CardSelectionHandler.SelectingCards && cardSelectStack.Count > 0)
        {
            CardSelectSettings curr = cardSelectStack.Pop();
            CardSelectionHandler.StartSelectingCards(curr);
            //StartCoroutine(SelectCardState(curr.numCards, curr.cardType, curr.selectionPurpose, curr.targetCharacter, curr.setupPlayerUI));
        }

        if(cardSelectStack.Count <= 0)
        {
            if(CurrentCharacter == player)
            {
                SetNewState(State.PLAYERTURN);
            }
            else
            {
                SetNewState(State.OPPONENTTURN);
            }
            Debug.Log(selectingCharacter.character.name + " is done selecting!");
        }
    }*/

    void EndRound()
    {
        CardGameLog.Instance.AddToLog("Round Over!");
        if (Mathf.Abs(targetValue - player.currValue) < Mathf.Abs(targetValue - opponent.currValue))
        {
            CardGameLog.Instance.AddToLog(player.character.name + " won this round");
            playerPoints += 1;
        }
        else if (Mathf.Abs(targetValue - player.currValue) > Mathf.Abs(targetValue - opponent.currValue))
        {
            CardGameLog.Instance.AddToLog(opponent.character.name + " won this round");
            opponentPoints += 1;
        }
        else
        {
            CardGameLog.Instance.AddToLog("This round was a tie with no victor");
        }

        if (playerPoints >= 2 || opponentPoints >= 2 || roundCount >= 3)
        {
            SetNewState(State.ENDGAME);
            //state = State.ENDGAME;
        }
        else
        {
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
            CardGameLog.Instance.ClearLog();
            CardGameLog.Instance.AddToLog("New Round");
            SetNewState(State.STARTROUND);
            //state = State.STARTROUND;
        }
    }

    void EndGame()
    {
        CardGameLog.Instance.AddToLog("Game over!");
    }

    void Paused()
    {
        Debug.Log("Paused Not Yet Implemented!");
    }

    public void DrawNumberCards(CharacterInstance target, int numberCards) {
        for(int i = 0; i < numberCards; i++) 
        {
            if(numberDeck.Count == 0) {
                Debug.Log("Number Deck is Empty");
                break;
            }
            NumberCard newCard = numberDeck[0];
            numberDeck.Remove(newCard);
            target.currValue += newCard.value;
            //Debug.Log("New Number Card for " + target.character.name + ": " + newCard.value);
            activeCardVisuals.Add(CreateNewDisplayNumberCard(target, newCard).GetComponent<DisplayCard>());
        }
        UpdateValues();
    }

    public GameObject CreateNewDisplayNumberCard(CharacterInstance target, NumberCard newCard)
    {
        GameObject newCardVisual = Instantiate(cardVisualPrefab);
        DisplayCard newCardDisplay = newCardVisual.GetComponent<DisplayCard>();
        target.numberDisplayHand.Add(newCardDisplay);
        newCardDisplay.InitNumberCard(newCard, target);
        PlaceCard(newCardVisual, target, newCard.value);
        return newCardVisual;
    }

    public void RemoveCardFromPlay(DisplayCard card, bool discard)
    {
        activeCardVisuals.Remove(card); 
        if (card.baseCard is NumberCard)
        {
            card.NumberCardOrganizer.RemoveCard(card.gameObject);
            card.owner.numberDisplayHand.Remove(card);
        }
        else
        {
            card.owner.specialDisplayHand.Remove(card);
        }

        if(discard)
        {
            discardPile.Add(card.baseCard);
        }
        Destroy(card.gameObject);
    }

    public void UpdateValues()
    {
        StartCoroutine(UpdateValuesDelay());
    }

    IEnumerator UpdateValuesDelay()
    {
        yield return new WaitForEndOfFrame();
        CardGameUIManager.Instance.UpdateValues();
    }

    void PlaceCard(GameObject card, CharacterInstance target, int value)
    {
        if(target == player)
        {
            if(value <=0)
            {
                CardGameUIManager.Instance.PlayerNegativeCardZone.PlaceCard(card);
            }
            else
            {
                CardGameUIManager.Instance.PlayerPositiveCardZone.PlaceCard(card);
            }
        } 
        else
        {
            if(value <= 0)
            {
                CardGameUIManager.Instance.OpponentNegativeCardZone.PlaceCard(card);
            }
            else
            {
                CardGameUIManager.Instance.OpponentPositiveCardZone.PlaceCard(card);
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
                newCardVisual.transform.SetParent(CardGameUIManager.Instance.PlayerHandTransform);
            }
            else {
                newCardVisual.transform.SetParent(CardGameUIManager.Instance.OpponentHandTransform);
            }
            activeCardVisuals.Add(newCardDisplay);
        }
    }

    public void PlayCard(DisplayCard display) {
        CardGameLog.Instance.AddToLog(display.owner.character.name + " played " + display.baseCard.name + "\nCard Effect: " + display.description.text);
        display.owner.DidAnAction = true;
        SpecialDeckCard card = (SpecialDeckCard) display.baseCard;

        CharacterInstance playerTarget = display.owner;
        CharacterInstance opponentTarget;
        if(playerTarget == opponent) {
            opponentTarget = player;
        }
        else {
            opponentTarget = opponent;
        }
        CardEffectChecker.Instance.ExecuteEffectStatement(card.InitialEffectStatement, playerTarget, opponentTarget);

        RemoveCardFromPlay(display, true);
        UpdateValues();
    }

    public void DiscardCard(DisplayCard display) 
    {
        CardGameLog.Instance.AddToLog(display.owner.character.name + " discarded " + display.baseCard.name);
        
        display.owner.DiscardedThisTurn = true;
        RemoveCardFromPlay(display, true);
        UpdateValues();
    }

    public void SwapCard(DisplayCard display)
    {
        if (display.baseCard is SpecialDeckCard)
        {
            DrawSpecialCards(display.owner, 1);
            display.owner.deck.Add(display.SpecialCard);
        }
        else
        {
            DrawNumberCards(display.owner, 1);
            numberDeck.Add((NumberCard)display.baseCard);
        }
        display.owner.SwappedThisTurn = true;
        RemoveCardFromPlay(display, false);
        UpdateValues();
    }

    public void FlipCard(DisplayCard display)
    {
        CreateNewDisplayNumberCard(display.owner, ((NumberCard)display.baseCard).oppositeCard);
        RemoveCardFromPlay(display, false);
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

    public void EndSelection()
    {
        SelectingCharacter = CurrentCharacter;
        if (SelectingCharacter == player)
        {
            CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerTurn);
        }
        else
        {
            CardGameUIManager.Instance.ChangeUIMode(UIMode.OpponentTurn);
        }
    }


    public void ButtonEndTurn() {
        //prevState =  GameState;
        //state = State.ENDTURN;
        SetNewState(State.ENDTURN);
    }

    public void ButtonConfirmSelection(){
        CardSelectionHandler.ConfirmSelection();
    }
}

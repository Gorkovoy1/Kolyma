using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public struct CardSelectSettings
{
    public readonly List<DisplayCard> SpecificCards { get; }

    public readonly int numCards { get; }
    public readonly CardType cardType { get; }
    public readonly NumberClass cardClass { get; }
    public readonly Effect selectionPurpose { get; }

    public readonly CharacterInstance selector;
    public readonly TargetCharacter target; 
    public readonly bool setupPlayerUI { get; }

    public readonly int miscValue { get; }

    public NumberOfCardsQuantifier quantifier;

    public CardSelectSettings(int number, CardType card, Effect effect, CharacterInstance selector, TargetCharacter target, bool playerUI, int miscValue, NumberClass numberClass, NumberOfCardsQuantifier quantifier) : this()
    {
        numCards = number;
        cardType = card;
        selectionPurpose = effect;
        this.selector = selector;
        this.target = target;
        setupPlayerUI = playerUI;
        this.miscValue = miscValue;
        cardClass = numberClass;
        this.quantifier = quantifier;
    }

    public CardSelectSettings(List<DisplayCard> specificCards, int number, CharacterInstance selector, TargetCharacter target, bool playerUI, int miscValue, Effect effect, NumberOfCardsQuantifier quantifier) : this()
    {
        SpecificCards = specificCards;
        selectionPurpose = effect;
        numCards = number;
        this.selector = selector;
        this.target = target;
        setupPlayerUI = playerUI;
        this.miscValue = miscValue;
        this.quantifier = quantifier;
    }
}

public class CardGameManager : MonoBehaviour
{
    public enum State {
        DEFAULT,
        INIT, //tasks to complete ONCE as the scene loads in, before anything starts. Use for technical behind the scenes stuff, eg loading art and sound
        DICEROLL,
        STARTGAME, //tasks to complete ONCE at the start of the game, after INIT. eg offering the choice to double bet
        DECKSETUP,
        STARTROUND, //tasks to complete at the start of each round of gameplay, eg dealing cards.
        PLAYERTURN, //the player can take their turn and play cards here
        OPPONENTTURN, /*opponent AI takes their turn and plays cards. Will access the AI (NYI) and any particulars for this opponent via the opponent scriptableobject.
        The opponents AI will need to be coded too- look into Behavior Trees maybe?- and the AI will dictate an action, pass the action to this manager to be performed, animated, and completed.
        Once this manager completes the designated action, pass back to opponent AI to determine next move*/
        //SELECTCARDS, //selection mode to pick cards to swap or discard, etc etc
        ENDTURN, //tasks to complete at the end of a turn. eg draws a card for current turn, then flips to other person's turn. Could perhaps reorg this dep. on needs- states for OPPONENTEND and PLAYEREND maybe?
        ENDROUND, //tasks to complete at the end of each round. eg check round winner, add score, end game if there is a winner or start next round if not.
        RESETBOARD,
        ENDGAME, //tasks to complete ONCE when the game is over. eg cleanup vars from gameplay, record winner, transfer out of card game scene
        PAUSED //player has paused the game- this'll mean something when there's a pause menu. enter and exit this state to basically pause the game.
    };


    [Header("Card Game Info")]
    //This class manages the card game. It is designed to attach to the card game scene and load and run one card game from start to finish.

    public int roundCount = 0; //defaults to round 0 whenever a new game is started

    public CardGameCharacter playerCharacter, opponentCharacter;

    public CharacterInstance player, opponent;

    public CharacterInstance SelectingCharacter;

    public List<NumberCard> OgNumberDeck;
    public List<NumberCard> UsedNumberDeck; //the numbers deck- assuming this is communal?

    public int targetValue; //target value to win a round

    public State GameState = State.INIT; //current state

    public List<DisplayCard> discardPile = new List<DisplayCard>();

    [HideInInspector] public List<DisplayCard> activeCardVisuals;

    public Stack<CardSelectSettings> cardSelectStack = new Stack<CardSelectSettings>();

    public GameObject cardVisualPrefab;

    public static CardGameManager Instance;

    public CardSelectionHandler CardSelectionHandler;

    public event OnTurnChangeDelegate OnCharacterChange;
    public delegate void OnTurnChangeDelegate(CharacterInstance currCharacter);
    public CharacterInstance CurrentCharacter, WaitingCharacter;

    public bool CharacterSelecting;

    public bool PlayerAI;

    public Dice PlayerDice1, PlayerDice2, OppDice1, OppDice2;

    public Transform PlayCardScreen;
    public TextMeshProUGUI CardPlayerText, CardNameText, CardEffectText;

    void Start()
    {
        Instance = this;
        CardSelectionHandler = gameObject.AddComponent<CardSelectionHandler>();
        SetNewState(State.INIT);
    }

    public void SetNewState(State newState, CharacterInstance triggeringCharacter = null)
    {
        if (GameState == newState) return;

        GameState = newState;
        switch (GameState)
        {
            case State.INIT:
                Init();
                break;

            case State.DICEROLL:
                StartCoroutine(DiceRoll());
                break;

            case State.STARTGAME:
                StartCoroutine(StartGame());
                break;

            case State.DECKSETUP:
                StartCoroutine(DeckSetup());
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
                StartCoroutine(EndRound());
                break;

            case State.RESETBOARD:
                StartCoroutine(ResetBoard());
                break;

            case State.ENDGAME:
                EndGame();
                break;

            case State.PAUSED:
                Paused();
                break;
        }
    }

    public void StartSelecting()
    {
        CardSelectionHandler.Instance.ProcessSelect();
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
        GameObject newOppObj = new GameObject();
        opponent = newOppObj.AddComponent<CharacterInstance>();
        newOppObj.name = "Opponent";

        player.Init(playerCharacter, CardGameUIManager.Instance.PlayerPositiveCardZone, CardGameUIManager.Instance.PlayerNegativeCardZone, PlayerAI, opponent);
        opponent.Init(opponentCharacter, CardGameUIManager.Instance.OpponentPositiveCardZone, CardGameUIManager.Instance.OpponentNegativeCardZone, true, player);

        player.Opponent = opponent;
        opponent.Opponent = player;

        CardGameUIManager.Instance.Init(player, opponent);
        SetNewState(State.DICEROLL);
        //state = State.STARTGAME;
    }

    IEnumerator DiceRoll()
    {
        yield return new WaitForSeconds(1f);

        PlayerDice1.gameObject.SetActive(true);
        PlayerDice2.gameObject.SetActive(true);
        OppDice1.gameObject.SetActive(true);
        OppDice2.gameObject.SetActive(true);
        StartCoroutine(PlayerDice1.RollTheDice(2f));
        StartCoroutine(PlayerDice2.RollTheDice(2f));
        StartCoroutine(OppDice1.RollTheDice(2f));
        StartCoroutine(OppDice2.RollTheDice(2f));

        yield return new WaitForSeconds(6f);
        player.Dice1 = PlayerDice1.finalSide;
        player.Dice2 = PlayerDice2.finalSide;
        opponent.Dice1 = OppDice1.finalSide;
        opponent.Dice2 = OppDice2.finalSide;


        int playerDice = player.Dice1 + player.Dice2;
        int opponentDice = opponent.Dice1 + opponent.Dice2;
        targetValue = playerDice + opponentDice;
        CardGameUIManager.Instance.targetValueText.text = "" + targetValue; //"TARGET VALUE: " + 
        CardGameLog.Instance.AddToLog("Target Value is " + targetValue);

        int playerPriorityDice = playerDice;
        int opponentPriorityDice = opponentDice;

        if (playerDice == opponentDice)
        {
            do
            {
                StartCoroutine(PlayerDice1.RollTheDice(2f));
                StartCoroutine(PlayerDice2.RollTheDice(2f));
                StartCoroutine(OppDice1.RollTheDice(2f));
                StartCoroutine(OppDice2.RollTheDice(2f));
                yield return new WaitForSeconds(3f);
                playerPriorityDice = PlayerDice1.finalSide + PlayerDice2.finalSide;
                opponentPriorityDice = OppDice1.finalSide + OppDice2.finalSide;
                CardGameLog.Instance.AddToLog("Tie Breaker Re-roll: " + playerPriorityDice + " (player) vs. " + opponentPriorityDice + " (opponent)");
            } while (playerPriorityDice == opponentPriorityDice);
        }

        yield return new WaitForSeconds(1f);
        PlayerDice1.gameObject.SetActive(false);
        PlayerDice2.gameObject.SetActive(false);
        OppDice1.gameObject.SetActive(false);
        OppDice2.gameObject.SetActive(false);

        if (playerPriorityDice > opponentPriorityDice)
        {
            CardGameLog.Instance.AddToLog("Player Goes First!");
            CurrentCharacter = player;
        }
        else
        {
            CardGameLog.Instance.AddToLog("Opponent Goes First!");
            CurrentCharacter = opponent;
        }
        SetNewState(State.STARTGAME);
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);

        UsedNumberDeck = new List<NumberCard>(OgNumberDeck);

        ShuffleCards(UsedNumberDeck);
        DrawNumberCards(opponent, 4);
        DrawNumberCards(player, 4);

        roundCount += 1;


        SetNewState(State.DECKSETUP);
        //state = State.STARTROUND;
    }

    IEnumerator DeckSetup()
    {

        yield return new WaitForSeconds(3f);

        CardGameLog.Instance.AddToLog("Choose your deck!");

        CardOptionsSelectionUI.Instance.FillInCards(player.character.deckList, player, 15, Effect.BuildDeck, NumberOfCardsQuantifier.EqualTo);

        for(int i = 0; i < 15; i++)
        {
            CardOptionsSelectionUI.Instance.CurrentCards[i].SelectButton.onClick.Invoke();
        }

        opponent.RandomlyChooseDeck();
        //Set up
    }

    private void Update()
    {
        UpdateValues();
    }

    public void StartRound()
    {
        discardPile.Clear();
        ShuffleCards(player.deck);
        ShuffleCards(opponent.deck);
        DrawSpecialCards(opponent, 6);
        DrawSpecialCards(player, 6);

        if(CurrentCharacter == player)
        {
            SetNewState(State.PLAYERTURN);
        }
        else
        {
            SetNewState(State.OPPONENTTURN);
        }
    }

    public void ToggleFlipSelectionMode(bool on, bool forced = false)
    {
        player.CurrentlyFlipping = on;
        player.FlippingForced = forced;
    }

    public void ToggleFlip()
    {
        if (CurrentCharacter == player && !player.DidAnAction)
        {
            SetMulligan(false);
            SetSwap(false);
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
            CardSelectSettings flipSettings = new CardSelectSettings(1, CardType.Number, Effect.Flip, CurrentCharacter, TargetCharacter.PlayerOfCard, true, 0, NumberClass.NONE, NumberOfCardsQuantifier.EqualTo);
            cardSelectStack.Push(flipSettings);
            CardSelectionHandler.ProcessSelect();
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
        player.FlippedFlag = 2;
        player.CurrentlyFlipping = false;
        player.DidAnAction = true;
        CardGameUIManager.Instance.ToggleFlipping(false);
        //FlipButton.interactable = false;
    }

    public void ToggleSwap()
    {
        if(CurrentCharacter == player && !player.DidAnAction)
        {
            SetMulligan(false);
            SetFlip(false);
            SetSwap(!CurrentCharacter.CurrentlySwapping);
        }
    }

    public void SetSwap(bool newSwap)
    {
        CurrentCharacter.CurrentlySwapping = newSwap;

        //CardGameUIManager.Instance.ChangeSwapMode(SwapState.);
        //SwapText.text = player.CurrentlySwapping ? "SWAPPING..." : "SWAP";

        if (CurrentCharacter.CurrentlySwapping)
        {
            CardSelectSettings swapSettings = new CardSelectSettings(1, CardType.Number, Effect.Swap, CurrentCharacter, TargetCharacter.PlayerOfCard, true, 0, NumberClass.NONE, NumberOfCardsQuantifier.EqualTo);
            cardSelectStack.Push(swapSettings);
            CardSelectionHandler.ProcessSelect();
        }
        else
        {
            CardSelectionHandler.EndSelectingCards();
        }

        if(CurrentCharacter == player)
            CardGameUIManager.Instance.ToggleSwapping(CurrentCharacter.CurrentlySwapping);
    }

    public void EndSwap()
    {
        //Mulligan == Swap
        if(player.CurrentlyMulliganing)
        {
            EndMulligan();
        }
        else
        {
            //SwapText.text = "SWAPPED";
            player.SwappedFlag = 2;
            player.CurrentlySwapping = false;
            player.DidAnAction = true;
            //SwapButton.interactable = false;
            CardGameUIManager.Instance.ToggleSwapping(false);
        }
    }

    public void ToggleMulligan()
    {
        if (CurrentCharacter == player && !player.DidAnAction)
        {
            SetFlip(false);
            SetSwap(false);
            SetMulligan(!CurrentCharacter.CurrentlyMulliganing);
        }
    }

    public void SetMulligan(bool newMulligan)
    {
        CurrentCharacter.CurrentlyMulliganing = newMulligan;

        //CardGameUIManager.Instance.ChangeSwapMode(SwapState.);
        //SwapText.text = player.CurrentlySwapping ? "SWAPPING..." : "SWAP";

        if (CurrentCharacter.CurrentlyMulliganing)
        {
            CardSelectSettings mulliganSettings = new CardSelectSettings(1, CardType.Special, Effect.Swap, CurrentCharacter, TargetCharacter.PlayerOfCard, true, 0, NumberClass.NONE, NumberOfCardsQuantifier.EqualTo);
            cardSelectStack.Push(mulliganSettings);
            CardSelectionHandler.ProcessSelect();
        }
        else
        {
            CardSelectionHandler.EndSelectingCards();
        }

        if (CurrentCharacter == player)
            CardGameUIManager.Instance.ToggleMulliganing(CurrentCharacter.CurrentlyMulliganing);
    }

    public void EndMulligan()
    {
        //SwapText.text = "SWAPPED";
        player.MulliganedFlag = 2;
        player.CurrentlyMulliganing = false;
        player.DidAnAction = true;
        //SwapButton.interactable = false;
        CardGameUIManager.Instance.ToggleMulliganing(false);
    }


    void PlayerTurn()
    {
        DrawSpecialCards(player, 1);
        //CardGameLog.Instance.AddToLog("Player Turn!");
        player.DidAnAction = false;
        CurrentCharacter = player;
        WaitingCharacter = opponent;
        SelectingCharacter = player;
        OnCharacterChange(player);
        CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerTurn);

        CardGameUIManager.Instance.ResetUI();
        player.HadATurn = true;
        player.DecrementFlags();

        for(int i = 0; i < player.specialDisplayHand.Count; i++)
        {
            DisplayCard currSpecialCard = player.specialDisplayHand[i];
            currSpecialCard.Playable = CardEffectChecker.Instance.CheckConditional(currSpecialCard.SpecialCard.InitialEffectStatement, player, opponent);
        }

        CheckIfRoundIsOver();
    }

    void OpponentTurn()
    {
        DrawSpecialCards(opponent, 1);
        opponent.DidAnAction = false;
        CurrentCharacter = opponent;
        WaitingCharacter = player;
        SelectingCharacter = opponent;
        OnCharacterChange(opponent);
        CardGameUIManager.Instance.ChangeUIMode(UIMode.OpponentTurn);
        opponent.HadATurn = true;
        opponent.DecrementFlags();

        for (int i = 0; i < player.specialDisplayHand.Count; i++)
        {
            DisplayCard currSpecialCard = player.specialDisplayHand[i];
            currSpecialCard.Playable = false;
        }

        CheckIfRoundIsOver();
    }

    void CheckIfRoundIsOver()
    {
        if(opponent.deck.Count != 0 || player.deck.Count != 0)
        {
            return;
        }

        for (int i = 0; i < player.specialDisplayHand.Count; i++)
        {
            DisplayCard currSpecialCard = player.specialDisplayHand[i];
            if (CardEffectChecker.Instance.CheckConditional(currSpecialCard.SpecialCard.InitialEffectStatement, player, opponent))
                return;
        }

        for (int i = 0; i < opponent.specialDisplayHand.Count; i++)
        {
            DisplayCard currSpecialCard = opponent.specialDisplayHand[i];
            if (CardEffectChecker.Instance.CheckConditional(currSpecialCard.SpecialCard.InitialEffectStatement, opponent, player))
                return;
        }

        CardGameLog.Instance.AddToLog("End Round! No more playable cards.");
        SetNewState(State.ENDROUND);
    }

    void EndTurn()
    {
        CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " Ends Turn");
        if (player.HadATurn && opponent.HadATurn && !player.DidAnAction && !opponent.DidAnAction)
        {
            SetNewState(State.ENDROUND);
        }
        else
        {
            if (CurrentCharacter == player)
            {
                SetNewState(State.OPPONENTTURN);
                //state = State.OPPONENTTURN;
            }
            else if (CurrentCharacter == opponent)
            {
                SetNewState(State.PLAYERTURN);
                //state = State.PLAYERTURN;
            }

            foreach (DisplayCard card in player.numberDisplayHand)
            {
                card.ResetFlags();
            }
            foreach (DisplayCard card in opponent.numberDisplayHand)
            {
                card.ResetFlags();
            }
        }
    }

    public void CharacterEndTurn(CharacterInstance characterEndingTurn)
    {
        if(characterEndingTurn == CurrentCharacter)
            SetNewState(State.ENDTURN);
    }

    public List<DisplayCard> GetSelectableCards()
    {
        if(!CardSelectionHandler.Instance.SelectingCards || CurrentCharacter == opponent)
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

    IEnumerator EndRound()
    {
        CardGameLog.Instance.AddToLog("Round Over!");

        bool playerLost = player.currValue > targetValue;
        bool opponentLost = opponent.currValue > targetValue;

        if (!playerLost && opponentLost)
        {
            CardGameLog.Instance.AddToLog(player.character.name + " won this round");
        }
        else if (playerLost && !opponentLost)
        {
            CardGameLog.Instance.AddToLog(opponent.character.name + " won this round");
        }
        else if (!playerLost && !opponentLost)
        {
            if (Mathf.Abs(targetValue - player.currValue) < Mathf.Abs(targetValue - opponent.currValue))
            {
                CardGameLog.Instance.AddToLog(player.character.name + " won this round");
            }
            else if (Mathf.Abs(targetValue - player.currValue) > Mathf.Abs(targetValue - opponent.currValue))
            {
                CardGameLog.Instance.AddToLog(opponent.character.name + " won this round");
            }
            else
            {
                CardGameLog.Instance.AddToLog("This round was a tie with no victor");
            }
        }

        yield return new WaitForSeconds(3f);

        SetNewState(State.ENDGAME);
        /*if (playerPoints >= 2 || opponentPoints >= 2 || roundCount >= 3)
        {
            SetNewState(State.ENDGAME);
            //state = State.ENDGAME;
        }
        else
        {
            SetNewState(State.RESETBOARD);
            //state = State.STARTROUND;
        }*/
    }

    IEnumerator ResetBoard()
    {
        targetValue = 0;

        List<DisplayCard> tmpPlayerNumbers = new List<DisplayCard>(player.numberDisplayHand);

        for(int i = 0; i < tmpPlayerNumbers.Count; i++)
        {
            RemoveCardFromPlay(tmpPlayerNumbers[i], false);
        }

        List<DisplayCard> tmpPlayerSpecials = new List<DisplayCard>(player.specialDisplayHand);

        for (int i = 0; i < tmpPlayerSpecials.Count; i++)
        {
            RemoveCardFromPlay(tmpPlayerSpecials[i], false);
        }

        player.deck.Clear();

        List<DisplayCard> tmpOppNumbers = new List<DisplayCard>(opponent.numberDisplayHand);

        for (int i = 0; i < tmpOppNumbers.Count; i++)
        {
            RemoveCardFromPlay(tmpOppNumbers[i], false);
        }

        List<DisplayCard> tmpOppSpecials = new List<DisplayCard>(opponent.specialDisplayHand);

        for (int i = 0; i < tmpOppSpecials.Count; i++)
        {
            RemoveCardFromPlay(tmpOppSpecials[i], false);
        }

        opponent.deck.Clear();

        activeCardVisuals.Clear();
        discardPile.Clear();

        CardGameLog.Instance.AddToLog("New Round!");

        yield return new WaitForSeconds(3f);

        SetNewState(State.DICEROLL);
    }

    void EndGame()
    {
        CardGameLog.Instance.AddToLog("Game over!");
    }

    void Paused()
    {
        Debug.Log("Paused Not Yet Implemented!");
    }

    public List<DisplayCard> DrawNumberCards(CharacterInstance target, int numberCards) {
        List<DisplayCard> newCards = new List<DisplayCard>();
        for(int i = 0; i < numberCards; i++) 
        {
            if(UsedNumberDeck.Count == 0) {
                Debug.Log("Number Deck is Empty");
                break;
            }
            NumberCard newCard = UsedNumberDeck[0];
            UsedNumberDeck.Remove(newCard);
            target.currValue += newCard.value;
            //Debug.Log("New Number Card for " + target.character.name + ": " + newCard.value);
            GameObject newCardObj = CreateNewDisplayNumberCard(target, newCard);
            newCards.Add(newCardObj.GetComponent<DisplayCard>());
            activeCardVisuals.Add(newCardObj.GetComponent<DisplayCard>());
        }
        UpdateValues();
        target.NewlyDrawnNumberCards = newCards;
        return newCards;
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
            discardPile.Add(card);
        }

        if(card != null)
            Destroy(card.gameObject);
    }

    public void UpdateValues()
    {
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
        List<DisplayCard> newCards = new List<DisplayCard>();
        for(int i = 0; i < specialCards; i ++) {
            if(target.deck.Count == 0) {
                Debug.Log(target.character.name + " is out of cards!");
                break;
            }
            SpecialDeckCard newCard = target.deck[0];
            target.deck.Remove(newCard);
            newCards.Add(AddSpecialCard(newCard, target));
        }
        target.NewlyDrawnSpecialCards = newCards;
    }

    public DisplayCard AddSpecialCard(SpecialDeckCard card, CharacterInstance target)
    {
        GameObject newCardVisual = Instantiate(cardVisualPrefab);
        DisplayCard newCardDisplay = newCardVisual.GetComponent<DisplayCard>();
        newCardDisplay.InitSpecialCard(card, target);
        newCardDisplay.owner = target;
        newCardDisplay.baseCard = card;
        target.specialDisplayHand.Add(newCardDisplay);
        if (target == player)
        {
            newCardVisual.transform.SetParent(CardGameUIManager.Instance.PlayerHandTransform);
        }
        else
        {
            newCardVisual.transform.SetParent(CardGameUIManager.Instance.OpponentHandTransform);
        }
        activeCardVisuals.Add(newCardDisplay);
        if(target == opponent)
        {
            newCardDisplay.SetHidden(true);
        }
        return newCardDisplay;
    }

    public void DrawFromDiscardPile(DisplayCard card, CharacterInstance target)
    {
        discardPile.Remove(card);
        activeCardVisuals.Add(card);
        card.owner = target;
        if(card.baseCard is SpecialDeckCard)
        {
            Debug.Log("Draw special from discard!");
            target.specialDisplayHand.Add(card);

            if (target == player)
            {
                card.transform.SetParent(CardGameUIManager.Instance.PlayerHandTransform);
            }
            else
            {
                card.transform.SetParent(CardGameUIManager.Instance.OpponentHandTransform);
            }
        }
        else
        {
            Debug.Log("Draw number from discard!");
            target.numberDisplayHand.Add(card);
        }
    }

    public void PlayCard(DisplayCard display) 
    {
        StartCoroutine(PlayCardWithAnimation(display));   
    }

    IEnumerator PlayCardWithAnimation(DisplayCard display)
    {
        CardGameLog.Instance.AddToLog(display.owner.character.name + " played " + display.baseCard.name + "\nCard Effect: " + display.description.text);
        display.owner.DidAnAction = true;

        PlayCardScreen.gameObject.SetActive(true);
        display.transform.SetParent(PlayCardScreen);
        display.transform.localPosition = new Vector3(0, 100f, 0);
        display.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        display.SetHidden(false);
        display.gameObject.GetComponent<DragCard>().enabled = false;
        display.gameObject.GetComponent<UIOnHoverEvent>().HideInfo();
        display.gameObject.GetComponent<UIOnHoverEvent>().enabled = false;

        CardPlayerText.text = display.owner.character.name + " Plays";
        CardNameText.text = display.SpecialCard.name;
        CardEffectText.text = display.SpecialCard.description;

        for (int i = 0; i < player.specialDisplayHand.Count; i++)
        {
            DisplayCard currSpecialCard = player.specialDisplayHand[i];
            currSpecialCard.Playable = false;
        }

        yield return new WaitForSeconds(3f);

        PlayCardScreen.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        SpecialDeckCard card = (SpecialDeckCard)display.baseCard;

        CharacterInstance playerTarget = display.owner;
        CharacterInstance opponentTarget;
        if (playerTarget == opponent)
        {
            opponentTarget = player;
        }
        else
        {
            opponentTarget = opponent;
        }
        CardEffectChecker.Instance.ExecuteEffectStatement(card.InitialEffectStatement, playerTarget, opponentTarget, false, true);

        RemoveCardFromPlay(display, true);
        UpdateValues();

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator EndEffectSequence()
    {
        if(!CardSelectionHandler.Instance.SelectingCards)
        {
            Debug.Log("End After Card!");
            yield return new WaitForSeconds(1f);
            SetNewState(State.ENDTURN);
        }
    }

    public void DiscardCard(DisplayCard display) 
    {
        CardGameLog.Instance.AddToLog(display.owner.character.name + " discarded " + display.baseCard.name);
        
        display.owner.DiscardedFlag = 2;
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
            List<DisplayCard> newCards = DrawNumberCards(display.owner, 1);
            foreach(DisplayCard card in newCards)
            {
                card.SwappedThisTurn = true;
            }
            UsedNumberDeck.Add((NumberCard)display.baseCard);
        }
        display.owner.SwappedFlag = 2;
        RemoveCardFromPlay(display, true);
        UpdateValues();
    }

    public void FlipCard(DisplayCard display)
    {
        GameObject newCard = CreateNewDisplayNumberCard(display.owner, ((NumberCard)display.baseCard).oppositeCard);
        newCard.GetComponent<DisplayCard>().FlippedThisTurn = true;
        RemoveCardFromPlay(display, false);
        UpdateValues();
    }

    public void ChangeCard(DisplayCard card, int newValue)
    {
        CharacterInstance cardOwner = card.owner;
        CardGameManager.Instance.DiscardCard(card);
        cardOwner.AddValue(newValue);
    }

    public void DuplicateCard(DisplayCard card)
    {
        if (card.baseCard is NumberCard)
        {
            card.owner.AddValue(card.value);
        }
        else
        {
            AddSpecialCard((SpecialDeckCard)card.baseCard, card.owner);
        }
    }

    public void GiveCard(DisplayCard card, bool copy = false)
    {
        CharacterInstance oppositeCharacter = card.owner == Instance.player ? Instance.opponent : Instance.player;
        if (card.baseCard is NumberCard)
        {
            DisplayCard newCard = oppositeCharacter.AddValue(card.value);
            newCard.Given = true;
        }
        else
        {
            AddSpecialCard((SpecialDeckCard)card.baseCard, oppositeCharacter);
        }
        if(!copy)
            DiscardCard(card);
    }

    public void StealCard(DisplayCard card)
    {
        CharacterInstance oppositeCharacter = card.owner == Instance.player ? Instance.opponent : Instance.player;
        if (card.baseCard is NumberCard)
        {
            oppositeCharacter.AddValue(card.value);
        }
        else
        {
            AddSpecialCard((SpecialDeckCard)card.baseCard, oppositeCharacter);
        }
        DiscardCard(card);
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

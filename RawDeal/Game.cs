using RawDealView;
using RawDealView.Formatters;
using RawDealView.Options;
using RawDeal.SuperStarClasses;

namespace RawDeal;

public class Game
{
    private View _view;
    private string _deckFolder;
    private Cards _cards = new Cards();
    private Deck _deck1;
    private Deck _deck2;
    private Player _player1;
    private Player _player2;
    public Player CurrentPlayer;
    public Player WaitingPlayer;
    private bool _gameOver;

    public Game(View view, string deckFolder)
    {
        _view = view;
        _deckFolder = deckFolder;
    }

    private bool CheckStartRequirements()
    {   
        string deckPath = _view.AskUserToSelectDeck(_deckFolder);
        _deck1 = new Deck(_view, deckPath, _cards);

        if (_deck1.CheckValidDeck())
        {
            string deckPath2 = _view.AskUserToSelectDeck(_deckFolder);
            _deck2 = new Deck(_view, deckPath2, _cards);
            return _deck2.CheckValidDeck();
        }
        
        return false;
    }

    private Deck GetStarterDeck()
    {
        if (IsDeck2Superior())
        {
            return _deck2;
        }
        else
        {
            return _deck1;
        }
    }

    private bool IsDeck2Superior()
    {
        return _deck2.DeckSuperstarCard.SuperstarValue > _deck1.DeckSuperstarCard.SuperstarValue;
    }

    private void CreatePlayers()
    {
        Deck p1Deck = GetStarterDeck();
        Deck p2Deck = (p1Deck == _deck1) ? _deck2 : _deck1;

        _player1 = new Player(_view, p1Deck, this);
        _player2 = new Player(_view, p2Deck, this);
    }
    
    private void SetPlayers()
    {
        CurrentPlayer = _player1;
        WaitingPlayer = _player2;
    }

    public void Play()
    {
        if (CheckStartRequirements())
        {
            CreatePlayers();
            StartGame();
        }
    }
    
    private void StartGame()
    {   
        SetPlayers();
        _player1.StartingDraw();
        _player2.StartingDraw();
        _view.SayThatATurnBegins(CurrentPlayer.Name);
        CurrentPlayer.DrawCard();
        CheckSuperStarTurnAbility();
        ShowGameInfo();
    }

    private void ShowGameInfo()
    {
        _view.ShowGameInfo(CurrentPlayer.PlayerInfo, WaitingPlayer.PlayerInfo);
        Turns();
    }

    private void SwitchPlayers()
    {
        (CurrentPlayer, WaitingPlayer) = (WaitingPlayer, CurrentPlayer);
    }

    private void NextTurn()
    {
        SwitchPlayers();
        RenewAbilities();
        HandlePlayerTurn();
        
        if (!_gameOver)
        {
            _view.SayThatATurnBegins(CurrentPlayer.Name);
            CheckSuperStarTurnAbility();
            ShowGameInfo();
        }
    }

    private void RenewAbilities()
    {
        if (WaitingPlayer.SuperstarCard.HasAbilityOption)
        {
            WaitingPlayer.SuperstarCard.UsedAbility = false;
        }
    }
    
    private void CheckSuperStarTurnAbility()
    {
        SuperstarCard superstarCard = CurrentPlayer.SuperstarCard;
        if (superstarCard.HasBeginningAbility)
        {
            superstarCard.Ability();
        }
    }

    private void HandlePlayerTurn()
    {
        CheckEnemyArsenal();
        if (HaveArsenal(CurrentPlayer))
        {
            CurrentPlayer.DrawCard();
        }
        else
        {
            FinishGame(WaitingPlayer);
        }
    }

    private void CheckEnemyArsenal()
    {
        if (WaitingPlayer.Arsenal.Count <= 0)
        {
            FinishGame(CurrentPlayer);
        }
    }
    private void Turns()
    {
        string selected;
        if (CurrentPlayer.SuperstarCard.HasAbilityOption && CurrentPlayer.SuperstarCard.HaveAbilityRequirements())
        {
            selected = CurrentPlayer.ShowPlayOptionsForSuperstar();
            ShowSpecialMainMenu(selected);
        }
        else
        {
            selected = CurrentPlayer.ShowPlayOptions();
            ShowMainMenu(selected);
        }
        
        
    }
    
    private void ShowMainMenu(string selected)
    {
        int number = int.Parse(selected);

        switch (number)
        {
            case 1:
                PossibleCardsToSee();
                break;
            case 2:
                SeePossibleCardsToPlay();
                break;
            case 3:
                NextTurn();
                break;
            case 4:
                Surrender();
                break;
        }
    }
    
    private void ShowSpecialMainMenu(string selected)
    {
        int number = int.Parse(selected);

        switch (number)
        {
            case 1:
                PossibleCardsToSee();
                break;
            case 2:
                SeePossibleCardsToPlay();
                break;
            case 3:
                CurrentPlayer.SuperstarCard.Ability();
                ShowGameInfo();
                break;
            case 4:
                NextTurn();
                break;
            case 5:
                Surrender();
                break;
        }
    }
    private void Surrender()
    {
        FinishGame(WaitingPlayer);
    }
    private void PossibleCardsToSee()
    {
        CardSet cardset = _view.AskUserWhatSetOfCardsHeWantsToSee();

        switch (cardset)
        {
            case CardSet.Hand:
                PrintCards(CurrentPlayer.Hand);
                break;
            case CardSet.RingArea:
                PrintCards(CurrentPlayer.RingArea);
                break;
            case CardSet.RingsidePile:
                PrintCards(CurrentPlayer.RingSide);
                break;
            case CardSet.OpponentsRingArea:
                PrintCards(WaitingPlayer.RingArea);
                break;
            case CardSet.OpponentsRingsidePile:
                PrintCards(WaitingPlayer.RingSide);
                break;
        }
    }

    private void PrintCards(List<Card> cardList)
    {
        List<string> stringList = new List<string>();
        foreach (var card in cardList)
        {
            string cardString = Formatter.CardToString(card);
            stringList.Add(cardString);
        }

        _view.ShowCards(stringList);
        ShowGameInfo();
    }

    private void PlayCard(PlayInfo playInfo, string cardString, string playedCardType)
    {
        _view.SayThatPlayerIsTryingToPlayThisCard(CurrentPlayer.Name, cardString);
        Card card = playInfo.Card;
        if (!CheckOpponentHandReversal(playInfo))
        {
            _view.SayThatPlayerSuccessfullyPlayedACard(); 
            ActionBasedOnType(card, playedCardType);
        }
    }
    
    private void ActionBasedOnType(Card card, string playedCardType)
    {
        switch (playedCardType)
        {
            case "ACTION":
                // Actions for when the card type is ACTION
                _view.SayThatPlayerMustDiscardThisCard(CurrentPlayer.Name, card.Title);
                CurrentPlayer.DiscardCardFromHand(card.IndexHand);
                CurrentPlayer.DrawCard();
                _view.SayThatPlayerDrawCards(CurrentPlayer.Name, 1);
            
                ShowGameInfo();
                break;

            case "MANEUVER":
                // Actions for when the card type is MANEUVER
                CurrentPlayer.PlayCard(card);
            
                int damage = GetCardDamage(card);
                if (damage > 0)
                {
                    AttackOpponent(damage, card);
                }
                else
                {
                    ShowGameInfo();
                }
                break;

            default:
                // Actions for when the card type is neither ACTION nor MANEUVER
                Turns();
                break;
        }
    }

    private bool CheckOpponentHandReversal(PlayInfo playInfo)
    {
        List<Card> opponentReversals = GetOpponentPlayableReversals(playInfo);
        if (opponentReversals.Any())
        {
            int selectedReversalIndex = GetSelectedReversalIndex(opponentReversals);
            if (selectedReversalIndex != -1)
            {  
                PlaySelectedReversalCard(selectedReversalIndex, playInfo);
                return true;
            }
        }

        return false;
    }

    private int GetSelectedReversalIndex(List<Card> opponentReversals)
    {
        List<PlayInfo> reversalInfoList = GetListOfPlayInfo(opponentReversals);
        List<string> opponentReversalStringCards = GetStringListOfPlayInfo(reversalInfoList);
        return _view.AskUserToSelectAReversal(WaitingPlayer.Name, opponentReversalStringCards);
    }

    private void PlaySelectedReversalCard(int selectedReversalIndex, PlayInfo playInfo)
    {
        Card card = playInfo.Card;
        List<PlayInfo> reversalInfoList = GetListOfPlayInfo(GetOpponentPlayableReversals(playInfo));
        _view.SayThatPlayerReversedTheCard(WaitingPlayer.Name, 
            GetStringListOfPlayInfo(reversalInfoList)[selectedReversalIndex]);
        CurrentPlayer.DiscardCardFromHand(card.IndexHand);
        Card reversal = reversalInfoList[selectedReversalIndex].Card;
        WaitingPlayer.PlayCard(reversal);
        NextTurn();
    }

    
    private bool CanPlayReversal(Card reversal, Card playedCard, Player player, string playedCardType)
    {
        bool hasSufficientFortitude = HasSufficientFortitude(player, reversal);
        bool cardsShareSubtype = CardsShareSubtype(playedCard, reversal, playedCardType);
        bool isReversalSpecial = IsReversalSpecial(playedCard);

        return (hasSufficientFortitude && (cardsShareSubtype || isReversalSpecial));
    }

    private bool HasSufficientFortitude(Player player, Card reversal)
    {
        int requiredFortitude = int.Parse(reversal.Fortitude);
        return player.Fortitude >= requiredFortitude;
    }

    private bool IsReversalSpecial(Card card)
    {
        const string reversalSpecial = "ReversalSpecial";
        return card.Subtypes.Contains(reversalSpecial);
    }

    private bool CardsShareSubtype(Card card1, Card card2, string playedCardType)
    {   
        if (playedCardType == "ACTION")
        {
            return card2.Subtypes.Any(subtype2 => subtype2.Contains("Action"));
        }
        return card1.Subtypes.Any(subtype1 => 
            card2.Subtypes.Any(subtype2 => subtype2.Contains(subtype1)));
    }

    
    private List<Card> GetOpponentPlayableReversals(PlayInfo playInfo)
    {
        Card playedCard = playInfo.Card;
        string playedCardType = playInfo.PlayedAs;
        List<Card> filteredCards = WaitingPlayer.Hand
            .Where(card => card.Types.Contains("Reversal") && CanPlayReversal(card, playedCard, WaitingPlayer, playedCardType)).ToList();
        return filteredCards;
    }
    private void AttackOpponent(int damage, Card card)
    {
        int currentDamage = 1;
        
        _view.SayThatSuperstarWillTakeSomeDamage(WaitingPlayer.Name, damage);
        
        while (currentDamage <= damage && !_gameOver)
        {
            bool getDamage = DamageOpponentWithCard(currentDamage, damage, card);
            currentDamage++;
            if (getDamage == false)
            {
                break;
            }
        }

        if (!_gameOver)
        {
            ShowGameInfo();
        }
    }

    private int GetCardDamage(Card card)
    {
        int damage = int.Parse(card.Damage);
        if (WaitingPlayer.SuperstarCard.Name == "MANKIND")
        {
            return damage - 1;
        }
        else return damage;
    }

    public void DamageOpponent(int currentDamage, int damage)
    {

        if (!HaveArsenal(WaitingPlayer))
        {
            FinishGame(CurrentPlayer);
        }
        
        Card removedCard = WaitingPlayer.LostCardForDamage();
        string removedCardString = Formatter.CardToString(removedCard);
        _view.ShowCardOverturnByTakingDamage(removedCardString, currentDamage, damage);
        
    }
    
    public bool DamageOpponentWithCard(int currentDamage, int damage, Card card)
    {

        if (!HaveArsenal(WaitingPlayer))
        {
            FinishGame(CurrentPlayer);
            return false;
        }
        
        Card removedCard = WaitingPlayer.LostCardForDamage();
        string removedCardString = Formatter.CardToString(removedCard);
        _view.ShowCardOverturnByTakingDamage(removedCardString, currentDamage, damage);
        
        if (removedCard.Types.Contains("Reversal") && CanPlayReversal(removedCard, card, WaitingPlayer, "MANEUVER"))
        {
            PlayReversalFromDeck(card, currentDamage, damage);
            return false;
        }
        
        return true;
    }
    

    private void PlayReversalFromDeck(Card playedCard, int currentDamage, int damage)
    {
        _view.SayThatCardWasReversedByDeck(WaitingPlayer.Name);
        if (CheckCardStuns(playedCard, currentDamage, damage))
        {
            ApplyStuns(playedCard);
        }
        NextTurn();
    }

    private bool CheckCardStuns(Card playedCard, int currentDamage, int damage)
    {
        return ((currentDamage != damage) && (playedCard.StunValue != "0"));
    }
    
    private void ApplyStuns(Card playedCard)
    {
        int stunValue = int.Parse(playedCard.StunValue);
        int cardNumber = _view.AskHowManyCardsToDrawBecauseOfStunValue(CurrentPlayer.Name, stunValue);

        _view.SayThatPlayerDrawCards(CurrentPlayer.Name, cardNumber);
        while (cardNumber > 0)
        {
            CurrentPlayer.DrawCard();
            cardNumber--;
        }
    }
    private bool PlayerHasPlayableCards(List<Card> playableCards)
    {
        if (playableCards.Count > 0)
        {
            return true;
        }
        else return false;
    }
    
    private void ChooseCardToPlay(List<Card> playableCards)
    {
        List<PlayInfo> playInfoList = GetListOfPlayInfo(playableCards);
        List<string> playStringList = GetStringListOfPlayInfo(playInfoList);
        int selection = _view.AskUserToSelectAPlay(playStringList);
            
        if (selection == -1)
        {
            ShowGameInfo();
        }
        else
        {   
            string playedCardType = playInfoList[selection].PlayedAs;
            PlayCard(playInfoList[selection], playStringList[selection], playedCardType);
        }
    }
    
    private List<string> GetStringListOfPlayInfo(List<PlayInfo> playInfoList)
    {
        
        List<string> stringList = new List<string>();
        foreach (var playInfo in playInfoList)
        {
            string playInfoString = Formatter.PlayToString(playInfo);
            stringList.Add(playInfoString);
        }
        return stringList;
    }

    private void SeePossibleCardsToPlay()
    {
        List<Card> playableCards = GetPlayableCards();
        if (!PlayerHasPlayableCards(playableCards))
        {
            _view.NothingToPlay();
            ShowGameInfo();
        }
        else
        {
            ChooseCardToPlay(playableCards);
        }
    }

    private List<Card> GetPlayableCards()
    {
        List<Card> filteredCards = CurrentPlayer.Hand
            .Where(card => card.Types.Count > 0 && IsLowerFortitude(card) && IsManeuverOrAction(card)).ToList();
        return filteredCards;
    }

    private bool IsManeuverOrAction(Card card)
    {
        return (card.Types[0] == "Maneuver" || card.Types[0] == "Action");
    }

    private bool IsLowerFortitude(Card card)
    {
        return (int.Parse(card.Fortitude) <= CurrentPlayer.Fortitude);
    }

    private List<PlayInfo> GetListOfPlayInfo(List<Card> filteredCards)
    {
        List<PlayInfo> playInfoList = new List<PlayInfo>();
        foreach (var card in filteredCards)
        {
            if (IsHybridCard(card))
            {
                AddPlayInfoToList(playInfoList, card, "action");
                AddPlayInfoToList(playInfoList, card, "maneuver");
            }
            else
            {
                AddPlayInfoToList(playInfoList, card, card.Types[0]);
            }
        }
        return playInfoList;
    }

    private void AddPlayInfoToList(List<PlayInfo> playInfoList, Card card, string type)
    {
        PlayInfo playInfo = GetPlayInfo(card, type);
        playInfoList.Add(playInfo);
    }
    
    private bool IsHybridCard(Card card)
    {
        if (card.Types.Contains("Action") && card.Types.Contains("Maneuver"))
        {
            return true;
        }
        return false;
    }
    
    private PlayInfo GetPlayInfo(Card card, string type)
    {
        PlayInfo playInfo = new PlayInfo
        {
            CardInfo = card,
            PlayedAs = type.ToUpper(),
            Card = card
        };
        return playInfo;
    }

    private bool HaveArsenal(Player player)
    {
        if (player.Arsenal.Count > 0)
        {
            return true;
        }
        else return false;
    }

    private void FinishGame(Player winner)
    {
        _gameOver = true;
        _view.CongratulateWinner(winner.Name);
    }

    public void DiscardOpponentCard()
    {
        List<string> HandStringCards = GetHandStringCards();
        int CardIndex = _view.AskPlayerToSelectACardToDiscard(HandStringCards, WaitingPlayer.SuperstarCard.Name, 
            WaitingPlayer.SuperstarCard.Name, 1);
        WaitingPlayer.DiscardCardFromHand(CardIndex);
    }
    private List<string> GetHandStringCards()
    {
        List<Card> HandCards = GetHandCards();
        List<string> stringCards = new List<string>();
        foreach (var card in HandCards)
        {
            string cardString = Formatter.CardToString(card);
            stringCards.Add(cardString);
        }

        return stringCards;
    }
    private List<Card> GetHandCards()
    {
        List<Card> Hand = WaitingPlayer.Hand;
        return Hand;
    }
}
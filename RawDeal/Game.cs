using RawDealView;
using RawDealView.Formatters;
using RawDealView.Options;

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

    private void PlayCard(Card card, string cardString)
    {
        CurrentPlayer.PlayCard(card);
        _view.SayThatPlayerIsTryingToPlayThisCard(CurrentPlayer.Name, cardString);
        _view.SayThatPlayerSuccessfullyPlayedACard();
        if (CheckIfAttack(card))
        {
            int damage = GetCardDamage(card);
            if (damage > 0)
            {
                AttackOpponent(damage);
            }
        }
        else Turns();
    }

    private bool CheckIfAttack(Card card)
    {
        if (card.Types[0] == "Maneuver")
        {
            return true;
        }
        else return false;
    }

    private void AttackOpponent(int damage)
    {
        int currentDamage = 1;
        
        _view.SayThatSuperstarWillTakeSomeDamage(WaitingPlayer.Name, damage);
        
        while (currentDamage <= damage && !_gameOver)
        {
            DamageOpponent(currentDamage, damage);
            currentDamage++;
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
        if (HaveArsenal(WaitingPlayer))
        {
            Card removedCard = WaitingPlayer.LostCardForDamage();
            string removedCardString = Formatter.CardToString(removedCard);
            _view.ShowCardOverturnByTakingDamage(removedCardString, currentDamage, damage);
        }
        else FinishGame(CurrentPlayer);
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
        List<string> playInfoList = GetListOfPlayInfo(playableCards);
        int selection = _view.AskUserToSelectAPlay(playInfoList);
            
        if (selection == -1)
        {
            ShowGameInfo();
        }
        else PlayCard(playableCards[selection], playInfoList[selection]);
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

    private List<string> GetListOfPlayInfo(List<Card> filteredCards)
    {
        List<string> playInfoList = new List<string>();
        foreach (var card in filteredCards)
        {
            PlayInfo playInfo = GetPlayInfo(card);
            string playInfoString = Formatter.PlayToString(playInfo);
            playInfoList.Add(playInfoString);
        }

        return playInfoList;
    }

    private PlayInfo GetPlayInfo(Card card)
    {
        PlayInfo playInfo = new PlayInfo
        {
            CardInfo = card,
            PlayedAs = card.Types[0].ToUpper()
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
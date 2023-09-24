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
    private Player _currentPlayer;
    private Player _waitingPlayer;
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
        if (_deck2._deckSuperstarCard.SuperstarValue > _deck1._deckSuperstarCard.SuperstarValue)
        {
            return _deck2;
        }
        else return _deck1;
    }

    private void CreatePlayers()
    {
        Deck p1Deck = GetStarterDeck();
        Deck p2Deck = (p1Deck == _deck1) ? _deck2 : _deck1;

        _player1 = new Player(_view, p1Deck);
        _player2 = new Player(_view, p2Deck);
    }

    public void Play()
    {
        if (CheckStartRequirements())
        {
            CreatePlayers();
            StartGame();
        }
    }

    private void SetPlayers()
    {
        _currentPlayer = _player1;
        _waitingPlayer = _player2;
    }
    
    private void StartGame()
    {   
        SetPlayers();
        _player1.StartingDraw();
        _player2.StartingDraw();
        _view.SayThatATurnBegins(_currentPlayer.Name);
        _currentPlayer.DrawCard();
        ShowGameInfo();
    }

    private void ShowGameInfo()
    {
        _view.ShowGameInfo(_currentPlayer.PlayerInfo, _waitingPlayer.PlayerInfo);
        Turns();
    }

    private void SwitchPlayers()
    {
        (_currentPlayer, _waitingPlayer) = (_waitingPlayer, _currentPlayer);
    }

    private void NextTurn()
    {
        SwitchPlayers();
        if (HaveArsenal(_currentPlayer))
        {
            _currentPlayer.DrawCard();
        }
        else FinishGame(_waitingPlayer);

        if (!_gameOver)
        {
            _view.SayThatATurnBegins(_currentPlayer.Name);
            ShowGameInfo();
        }
    }

    private void Turns()
    {
        var selected = _currentPlayer.ShowPlayOptions();
        MainMenu(selected);
    }

    private void Surrender()
    {
        FinishGame(_waitingPlayer);
    }

    private void MainMenu(string selected)
    {
        int number = int.Parse(selected);

        switch (number)
        {
            case 1:
                SeePossibleCardssToSee();
                break;
            case 2:
                SeePossibleCards();
                break;
            case 3:
                NextTurn();
                break;
            case 4:
                Surrender();
                break;
        }
    }

    private void SeePossibleCardssToSee()
    {
        CardSet cardset = _view.AskUserWhatSetOfCardsHeWantsToSee();

        switch (cardset)
        {
            case CardSet.Hand:
                PrintCards(_currentPlayer.Hand);
                break;
            case CardSet.RingArea:
                PrintCards(_currentPlayer.RingArea);
                break;
            case CardSet.RingsidePile:
                PrintCards(_currentPlayer.RingSide);
                break;
            case CardSet.OpponentsRingArea:
                PrintCards(_waitingPlayer.RingArea);
                break;
            case CardSet.OpponentsRingsidePile:
                PrintCards(_waitingPlayer.RingSide);
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
        _currentPlayer.PlayCard(card);
        _view.SayThatPlayerIsTryingToPlayThisCard(_currentPlayer.Name, cardString);
        _view.SayThatPlayerSuccessfullyPlayedACard();
        if (CheckIfAttack(card))
        {
            AttackOpponent(card);
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

    private void AttackOpponent(Card card)
    {
        int damage = int.Parse(card.Damage);
        int currentDamage = 1;
        _view.SayThatSuperstarWillTakeSomeDamage(_waitingPlayer.Name, damage);

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

    private void DamageOpponent(int currentDamage, int damage)
    {
        if (HaveArsenal(_waitingPlayer))
        {
            Card removedCard = _waitingPlayer.LostCardForDamage();
            string removedCardString = Formatter.CardToString(removedCard);
            _view.ShowCardOverturnByTakingDamage(removedCardString, currentDamage, damage);
        }
        else FinishGame(_currentPlayer);
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
    
    private void SeePossibleCards()
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
        List<Card> filteredCards = _currentPlayer.Hand
            .Where(card => card.Types.Count > 0 && IsLowerFortitude(card) && IsManeuverOrAction(card)).ToList();
        return filteredCards;
    }

    private bool IsManeuverOrAction(Card card)
    {
        return (card.Types[0] == "Maneuver" || card.Types[0] == "Action");
    }

    private bool IsLowerFortitude(Card card)
    {
        return (int.Parse(card.Fortitude) <= _currentPlayer.Fortitude);
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
}
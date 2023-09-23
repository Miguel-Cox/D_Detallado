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

    private void StartGame()
    {
        _currentPlayer = _player1;
        _waitingPlayer = _player2;
        _player1.StartingDraw();
        _player2.StartingDraw();
        _view.SayThatATurnBegins(_currentPlayer._name);
        _currentPlayer.DrawCard();
        ShowGameInfo();
        Turns();
    }

    private void ShowGameInfo()
    {
        _view.ShowGameInfo(_player1.PlayerInfo, _player2.PlayerInfo);
    }

    private void SwitchPlayers()
    {
        (_currentPlayer, _waitingPlayer) = (_waitingPlayer, _currentPlayer);
    }

    private void NextTurn()
    {
        SwitchPlayers();
        _currentPlayer.DrawCard();
        _view.SayThatATurnBegins(_currentPlayer._name);
        ShowGameInfo();
        Turns();
    }

    private void Turns()
    {
        var selected = _currentPlayer.ShowPlayOptions();
        MainMenu(selected);
    }

    private void Surrender()
    {
        _view.CongratulateWinner(_waitingPlayer._name);
    }

    private void MainMenu(string selected)
    {
        int number = int.Parse(selected);

        switch (number)
        {
            case 1:
                ChooseCardsToSee();
                break;
            case 2:
                ChooseCard();
                break;
            case 3:
                NextTurn();
                break;
            case 4:
                Surrender();
                break;
            default:
                break;
        }
    }

    private void ChooseCardsToSee()
    {
        CardSet cardset = _view.AskUserWhatSetOfCardsHeWantsToSee();

        switch (cardset)
        {
            case CardSet.Hand:
                PrintCards(_currentPlayer._hand);
                break;
            case CardSet.RingArea:
                PrintCards(_currentPlayer._ringArea);
                break;
            case CardSet.RingsidePile:
                PrintCards(_currentPlayer._ringSide);
                break;
            case CardSet.OpponentsRingArea:
                PrintCards(_waitingPlayer._ringArea);
                break;
            case CardSet.OpponentsRingsidePile:
                PrintCards(_waitingPlayer._ringSide);
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
        Turns();
    }

    private void PlayCard(Card card, string cardString)
    {
        _currentPlayer.PlayCard(card);
        _view.SayThatPlayerIsTryingToPlayThisCard(_currentPlayer._name, cardString);
        _view.SayThatPlayerSuccessfullyPlayedACard();
        if (CheckIfAttack(card))
        {
            AttackOponent(card);
        }
        

    }

    private bool CheckIfAttack(Card card)
    {
        if (card.Types[0] == "Maneuver")
        {
            return true;
        }
        else return false;
    }

    private void AttackOponent(Card card)
    {
        int damage = int.Parse(card.Damage);
        int currentDamage = 1;
        _view.SayThatSuperstarWillTakeSomeDamage(_waitingPlayer._name, damage);
        
        while (currentDamage <= damage)
        {   
            DamageOponent(currentDamage, damage);
            currentDamage++;
        }

        if (!_gameOver)
        {
            ShowGameInfo();
            Turns();
        }
        
    }

    private void DamageOponent(int currentDamage, int damage)
    {   
        if (HaveArsenal(_waitingPlayer))
        {
            Card removedCard = _waitingPlayer.LostCardForDamage();
            PlayInfo removedCardPLayInfo = GetPlayInfo(removedCard);
            string removedCardString = Formatter.PlayToString(removedCardPLayInfo);
            _view.ShowCardOverturnByTakingDamage(removedCardString, currentDamage, damage);   
        }
        else FinishGame(_currentPlayer);
    }
    
    private void ChooseCard()
    {
        List<Card> playableCards = GetPlayableCards();
        List<string> playInfoList = GetListOfPlayInfo(playableCards);
        int selection = _view.AskUserToSelectAPlay(playInfoList);
        if (selection == -1)
        {
            Turns();
        }else PlayCard(playableCards[selection], playInfoList[selection]);

    }

    private List<Card> GetPlayableCards()
    {
        List<Card> filteredCards = _currentPlayer._hand
            .Where(card => card.Subtypes.Count > 0 && IsLowerFornitude(card) && IsManeuverOrStrike(card)).ToList();
        return filteredCards;
    }

    private bool IsManeuverOrStrike(Card card)
    {
        return (card.Types[0] == "Maneuver" || card.Types[0] == "Strike");
    }

    private bool IsLowerFornitude(Card card)
    {
        return (int.Parse(card.Fortitude) <= _currentPlayer.fortitude);
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
        if (player._arsenal.Count > 0)
        {
            return true;
        }
        else return false;
    }
    
    private void FinishGame(Player winner)
    {   
        _gameOver = true;
        _view.CongratulateWinner(winner._name);
    }
}